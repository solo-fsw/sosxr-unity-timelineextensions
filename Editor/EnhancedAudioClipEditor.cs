using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;


[CustomTimelineEditor(typeof(EnhancedAudioClip))]
public class EnhancedAudioClipEditor : ClipEditor
{
    public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
    {
        base.OnCreate(clip, track, clonedFrom);

        // Set the initial duration based on the audio clip
        if (clip.asset is EnhancedAudioClip enhancedAudioClip && enhancedAudioClip.Clip != null)
        {
            clip.duration = enhancedAudioClip.Clip.length;
        }
    }


    public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
    {
        base.DrawBackground(clip, region);

        if (clip.asset is EnhancedAudioClip enhancedAudioClip && enhancedAudioClip.Clip != null)
        {
            DrawLoopIndicator(region.position, clip.duration, enhancedAudioClip.Clip.length);
        }
    }


    private void DrawLoopIndicator(Rect rect, double clipDuration, float audioClipLength)
    {
        if (clipDuration <= audioClipLength)
        {
            return;
        }


        var loopCount = Mathf.CeilToInt((float) (clipDuration / audioClipLength));
        var loopWidth = rect.width / loopCount;

        for (var i = 0; i < loopCount; i++)
        {
            var loopRect = new Rect(rect.x + i * loopWidth, rect.y, loopWidth, rect.height);
            GUI.Label(loopRect, "Loop", EditorStyles.centeredGreyMiniLabel);
        }
    }
}