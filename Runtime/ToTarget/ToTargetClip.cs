using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the ToTarget track. Holds references to the starting point and destination GameObjects (both via
    ///     <see cref="ExposedReference{T}"/>). When <c>forceClipLength</c> is enabled, the clip's duration is automatically
    ///     calculated to exactly cover the move, accounting for ease curves, move speed, and stopping distance.
    /// </summary>
    [Serializable]
    public class ToTargetClip : PlayableAsset
    {
        public ExposedReference<GameObject> StartingPoint;
        public ExposedReference<GameObject> Target;

        public ToTargetBehaviour Template = new();

        private TimelineClip _timelineClip;
        private const string _divider = " - ";

        public GameObject StartingPointGO { get; set; }
        public GameObject TargetGO { get; set; }

        public TimelineClip TimelineClip
        {
            get => _timelineClip;
            set => _timelineClip = value;
        }

        public ToTargetBehaviour Behaviour { get; set; }

        public override double duration
        {
            get
            {
                if (Template == null)
                {
                    return base.duration;
                }
                if (!Template.ForceClipLength)
                {
                    return base.duration;
                }

                if (Behaviour == null || Behaviour?.DurationToTarget == 0)
                {
                    return base.duration;
                }

                return TimelineClip.duration = Behaviour.DurationToTarget;
            }
        }

        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<ToTargetBehaviour> playable = ScriptPlayable<ToTargetBehaviour>.Create(graph, Template); // Create a playable using the constructor

            Behaviour = playable.GetBehaviour(); // Get behaviour

            if (StartingPointGO == null)
            {
                StartingPointGO = StartingPoint.Resolve(graph.GetResolver());
            }

            if (TargetGO == null)
            {
                TargetGO = Target.Resolve(graph.GetResolver());
            }

            SetValuesOnBehaviourFromClip(Behaviour);

            SetDisplayName(Behaviour, TimelineClip);

            return playable;
        }

        private void SetValuesOnBehaviourFromClip(ToTargetBehaviour behaviour)
        {
            behaviour.ToTargetClip = this;
            behaviour.Target = TargetGO;
            behaviour.StartingPoint = StartingPointGO;
        }

        /// <summary>
        ///     The displayname of the clip in Timeline will be set using this method.
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        private void SetDisplayName(ToTargetBehaviour behaviour, TimelineClip clip)
        {
            var displayName = "";

            if (behaviour.Target == null || behaviour.StartingPoint == null)
            {
                return;
            }

            displayName += "To: " + behaviour.Target.name;

            displayName += _divider + behaviour.StartingPoint.name; // TODO: fix naming

            displayName = RemoveTrailingDivider(displayName);
            displayName = SetDisplayNameIfStillEmpty(displayName);

            if (clip == null)
            {
                return;
            }

            clip.displayName = displayName;
        }

        private static string RemoveTrailingDivider(string dispName)
        {
            if (string.IsNullOrEmpty(dispName))
            {
                return dispName;
            }

            var removeLast = dispName.LastIndexOf(_divider, StringComparison.Ordinal);

            if (removeLast < 0)
            {
                return dispName;
            }

            dispName = dispName[..removeLast];

            return dispName;
        }

        private static string SetDisplayNameIfStillEmpty(string dispName)
        {
            if (string.IsNullOrEmpty(dispName))
            {
                dispName = "New ToTarget Clip";
            }

            return dispName;
        }
    }
}
