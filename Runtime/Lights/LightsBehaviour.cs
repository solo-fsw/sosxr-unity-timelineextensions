using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class LightsBehaviour : PlayableBehaviour
    {
        public Light TrackBinding { get; set; }
        public LightsClip Clip { get; set; }


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!TrackBinding)
            {
                return;
            }

            TrackBinding.enabled = true;
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!TrackBinding)
            {
                return;
            }

            TrackBinding.intensity = Clip.Intensity * info.weight;
            TrackBinding.range = Clip.Range * info.weight;
            TrackBinding.color = Clip.Color;
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!TrackBinding)
            {
                return;
            }

            TrackBinding.enabled = false;
        }
    }
}