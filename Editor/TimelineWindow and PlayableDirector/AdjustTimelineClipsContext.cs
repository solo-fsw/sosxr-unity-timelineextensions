using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions.Editor
{
    public static class AdjustTimelineClipsWindow
    {
        private const double SmallStep = 0.1; // sec
        private const double LargeStep = 0.5; // sec


        // Left Edge with Option / Alt
        [MenuItem("SOSXR/Timeline Extensions/Move Left Edge - Left _&LEFT")] // Option / Alt
        private static void MoveLeftEdgeLeft()
        {
            AdjustEdges(SmallStep, true, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Left Edge - Left (Large) _&#LEFT")] // Option+Shift / Alt+Shift
        private static void MoveLeftEdgeLeftLarge()
        {
            AdjustEdges(LargeStep, true, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Left Edge - Right _&RIGHT")] // Option / Alt
        private static void MoveLeftEdgeRight()
        {
            AdjustEdges(-SmallStep, true, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Left Edge - Right (Large) _&#RIGHT")] // Option+Shift / Alt+Shift
        private static void MoveLeftEdgeRightLarge()
        {
            AdjustEdges(-LargeStep, true, true);
        }


        // Right Edge with Cmd / Control
        [MenuItem("SOSXR/Timeline Extensions/Move Right Edge - Left _%LEFT")] // Cmd / Control
        private static void MoveRightEdgeLeft()
        {
            AdjustEdges(-SmallStep, true, false);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Right Edge - Left (Large) _%#LEFT")] // Cmd+Shift / Control+Shift
        private static void MoveRightEdgeLeftLarge()
        {
            AdjustEdges(-LargeStep, true, false);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Right Edge - Right _%RIGHT")] // Cmd / Control
        private static void MoveRightEdgeRight()
        {
            AdjustEdges(SmallStep, true, false);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Right Edge - Right (Large) _%#RIGHT")] // Cmd+Shift / Control+Shift
        private static void MoveRightEdgeRightLarge()
        {
            AdjustEdges(LargeStep, true, false);
        }


        // Both edges with Option + Cmd / Alt + Control
        [MenuItem("SOSXR/Timeline Extensions/Move Both Edges - Left _%&LEFT")] // Option+Cmd / Alt+Control
        private static void MoveBothEdgesLeft()
        {
            AdjustEdges(SmallStep, false, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Both Edges - Left (Large) _%&#LEFT")] // Option+Cmd+Shift / Alt+Control+Shift
        private static void MoveBothEdgesLeftLarge()
        {
            AdjustEdges(LargeStep, false, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Both Edges - Right _%&RIGHT")] // Option+Cmd / Alt+Control
        private static void MoveBothEdgesRight()
        {
            AdjustEdges(-SmallStep, false, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Both Edges - Right (Large) _%&#RIGHT")] // Option+Cmd+Shift / Alt+Control+Shift
        private static void MoveBothEdgesRightLarge()
        {
            AdjustEdges(-LargeStep, false, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Left Ease - Left _&-")] // Option / Alt
        private static void MoveLeftEaseLeft()
        {
            AdjustEase(-SmallStep, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Left Ease - Right _&=")] // Option / Alt
        private static void MoveLeftEaseRight()
        {
            AdjustEase(SmallStep, true);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Right Ease - Left _%-")] // Cmd / Control
        private static void MoveRightEaseLeft()
        {
            AdjustEase(SmallStep, false);
        }


        [MenuItem("SOSXR/Timeline Extensions/Move Right Ease - Right _%=")] // Cmd / Control
        private static void MoveRightEaseRight()
        {
            AdjustEase(-SmallStep, false);
        }


        private static void AdjustEdges(double seconds, bool adjustDuration, bool leftEdge)
        {
            var clips = TimelineEditor.selectedClips;

            if (clips == null || clips.Length == 0)
            {
                Debug.LogWarning("No Timeline clips selected.");

                return;
            }

            var tracks = CreateUndo(clips);
            Undo.RecordObjects(tracks.ToArray<Object>(), "Adjust Clip Edges");

            foreach (var clip in clips)
            {
                if (leftEdge)
                {
                    clip.start = Mathf.Max((float) (clip.start - seconds), 0); // Prevent start going below 0s. We don't need to do this for clip end since the Timeline will automatically adjust the duration of the entire graph.
                }

                if (adjustDuration)
                {
                    clip.duration = Mathf.Max((float) (clip.duration + seconds), 0.1f); // Prevent duration going below 0.1s
                }
            }

            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }


        private static void AdjustEase(double seconds, bool leftEase)
        {
            var clips = TimelineEditor.selectedClips;

            if (clips == null || clips.Length == 0)
            {
                Debug.LogWarning("No Timeline clips selected.");

                return;
            }

            var tracks = CreateUndo(clips);
            Undo.RecordObjects(tracks.ToArray<Object>(), "Adjust Clip Ease");

            foreach (var clip in clips)
            {
                if (leftEase)
                {
                    clip.easeInDuration = Mathf.Max((float) (clip.easeInDuration + seconds), 0); // Prevent ease in going below 0s
                }
                else
                {
                    clip.easeOutDuration = Mathf.Max((float) (clip.easeOutDuration + seconds), 0); // Prevent ease out going below 0s
                }
            }

            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }


        private static HashSet<TrackAsset> CreateUndo(TimelineClip[] clips)
        {
            var tracks = new HashSet<TrackAsset>();

            foreach (var clip in clips)
            {
                if (clip.GetParentTrack() != null)
                {
                    tracks.Add(clip.GetParentTrack());
                }
            }

            return tracks;
        }
    }
}