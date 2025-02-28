using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class EnhancedAudioClip : PlayableAsset
    {
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume = 1f;
        [Range(-3f, 3f)] public float Pitch = 1f;

        public bool Mute = false;
        [Tooltip("0 is 2D, 1 is 3D")]
        [Range(0f, 1f)] public float SpatialBlend = 1f;

        public Vector2 Distance = new(0.25f, 10f);
        public AnimationCurve VolumeOverDistance = AnimationCurve.EaseInOut(0, 1, 1, 0.1f);


        public EnhancedAudioBehaviour Template { get; private set; }
        public TimelineClip TimelineClip { get; set; }


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EnhancedAudioBehaviour>.Create(graph, Template);

            var behaviour = playable.GetBehaviour();

            Template = behaviour;

            behaviour.Initialize(this);

            return playable;
        }
    }
}