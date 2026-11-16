using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the ToTarget track. At edit time, resolves scene references, masks axes, pre-computes the
    ///     ease-weight integral that determines clip duration, and chains start positions between consecutive clips on the
    ///     same track so multi-hop paths work correctly.
    /// </summary>
    [Serializable]
    public class ToTargetClip : Clip
    {
        /// <summary>The scene GameObject the bound object moves toward. Resolved through the Director's exposed reference table.</summary>
        public ExposedReference<GameObject> Target;

        /// <summary>Movement speed in units per second. Clip duration is derived from the travel distance divided by this value.</summary>
        [Range(0.1f, 20f)]
        public float MoveSpeed = 2f;
        /// <summary>Rate at which the object turns toward its direction of travel.</summary>
        public float RotateSpeed = 3f;

        /// <summary>Per-axis switch (1 = use, 0 = ignore). Axes set to 0 are removed from the displacement before the end position and duration are calculated.</summary>
        [Tooltip("Which axes to use for movement calculations. 0 = ignore, 1 = use.")]
        public Vector3Int AxisToUse = new(1, 0, 1);

        /// <summary>Behaviour copied into each playable created from this clip. Hidden in the Inspector.</summary>
        [HideInInspector]
        public ToTargetBehaviour Template = new();

        /// <summary>World position of the target, captured in InitializeClip. Zero when the target does not resolve.</summary>
        public Vector3 ResolvedTargetPosition { get; private set; }

        /// <summary>Axis-masked end position — where the object actually arrives. Used to chain subsequent clips.</summary>
        public Vector3 ResolvedEffectiveEndPosition { get; private set; }

        private float _totalEaseWeightIntegral;

        /// <summary>Overrides the base to also resolve the target, compute the axis-masked end position, and update the clip duration and display name in the editor.</summary>
        public override void InitializeClip(
            object trackBinding,
            TimelineClip timelineClip,
            IExposedPropertyTable resolver
        )
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);

            var targetGO = Target.Resolve(resolver);
            ResolvedTargetPosition = targetGO != null ? targetGO.transform.position : Vector3.zero;

            var startPosition = ResolveStartPosition(timelineClip);

            var displacement = ResolvedTargetPosition - startPosition;

            if (AxisToUse.x == 0)
            {
                displacement.x = 0;
            }

            if (AxisToUse.y == 0)
            {
                displacement.y = 0;
            }

            if (AxisToUse.z == 0)
            {
                displacement.z = 0;
            }

            ResolvedEffectiveEndPosition = startPosition + displacement;

            var distance = displacement.magnitude;
            _totalEaseWeightIntegral = MoveSpeed > 0 ? distance / MoveSpeed : 0f;

            UpdateClipDuration(timelineClip);
            UpdateDisplayName(timelineClip, targetGO);
        }

        private Vector3 ResolveStartPosition(TimelineClip timelineClip)
        {
            var previousClip = GetPreviousClip(timelineClip);

            if (previousClip != null)
            {
                return previousClip.ResolvedEffectiveEndPosition;
            }

            var binding = TrackBinding as GameObject;

            return binding != null ? binding.transform.position : Vector3.zero;
        }

        private static ToTargetClip GetPreviousClip(TimelineClip timelineClip)
        {
            if (timelineClip.GetParentTrack() == null)
            {
                return null;
            }

            ToTargetClip result = null;
            double latestStart = double.MinValue;

            foreach (var c in timelineClip.GetParentTrack().GetClips())
            {
                if (c.asset is ToTargetClip toTargetClip && c.start < timelineClip.start && c.start > latestStart)
                {
                    latestStart = c.start;
                    result = toTargetClip;
                }
            }

            return result;
        }

        private void UpdateClipDuration(TimelineClip timelineClip)
        {
            if (Application.isPlaying || _totalEaseWeightIntegral <= 0)
            {
                return;
            }

            var easeInDuration = (float)timelineClip.easeInDuration;
            var easeOutDuration = (float)timelineClip.easeOutDuration;

            // Guard against null curves to prevent NullReferenceException
            var easeInCurve = timelineClip.mixInCurve ?? new AnimationCurve();
            var easeOutCurve = timelineClip.mixOutCurve ?? new AnimationCurve();

            var easeInArea = AreaUnderCurve(easeInCurve);
            var easeOutArea = AreaUnderCurve(easeOutCurve);

            var duration =
                _totalEaseWeightIntegral
                + easeInDuration * (1f - easeInArea)
                + easeOutDuration * (1f - easeOutArea);

            // InitializeClip runs on every graph rebuild: only touch the clip when the computed duration changed.
            if (Math.Abs(timelineClip.duration - duration) < DurationTolerance)
            {
                return;
            }

            timelineClip.duration = duration;

#if UNITY_EDITOR
            // No Undo record: the duration follows from the clip's inputs, so undoing those recomputes it. Recording here
            // could run inside an undo-triggered rebuild and clear the redo stack. Mark dirty so the new duration is saved.
            var track = timelineClip.GetParentTrack();

            if (track != null)
            {
                UnityEditor.EditorUtility.SetDirty(track);
            }
#endif
        }

        /// <summary>Smallest duration change (seconds) that <see cref="UpdateClipDuration"/> writes to the clip.</summary>
        private const double DurationTolerance = 0.0001;

        /// <summary>
        ///     Number of steps used for trapezoidal rule integration when calculating area under animation curves.
        ///     Higher values increase accuracy but also computation time.
        /// </summary>
        private const int AreaUnderCurveSteps = 100;

        /// <summary>
        ///     Approximates the area under an <see cref="AnimationCurve"/> via the trapezoidal rule.
        ///     Used to determine how much of the ease duration is already "covered" by the ease curve shape when computing total clip duration.
        /// </summary>
        private static float AreaUnderCurve(AnimationCurve curve)
        {
            const int steps = AreaUnderCurveSteps;
            var sum = 0f;

            for (var i = 0; i < steps; i++)
            {
                var t0 = (float)i / steps;
                var t1 = (float)(i + 1) / steps;
                sum += (curve.Evaluate(t0) + curve.Evaluate(t1)) * 0.5f * (t1 - t0);
            }

            return sum;
        }

        private static void UpdateDisplayName(TimelineClip timelineClip, GameObject targetGO)
        {
            if (timelineClip == null)
            {
                return;
            }

            timelineClip.displayName =
                targetGO != null ? $"To: {targetGO.name}" : "To: (no target)";
        }

        /// <summary>
        ///     Creates the ToTarget playable for this clip and wires up initial data.
        /// </summary>
        /// <param name="graph">PlayableGraph to which the playable belongs.</param>
        /// <param name="owner">Owner GameObject (usually the clip's host).</param>
        /// <returns>A ScriptPlayable of ToTargetBehaviour configured for this clip.</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ToTargetBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();

            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            clone.MoveSpeed = MoveSpeed;
            clone.RotateSpeed = RotateSpeed;
            clone.AxisToUse = AxisToUse;
            clone.TotalEaseWeightIntegral = _totalEaseWeightIntegral;

            // Validate and resolve ExposedReference
            var targetGO = Target.Resolve(Resolver);
            if (targetGO == null)
            {
                Debug.LogWarning($"{TypeName}: Target could not be resolved. Make sure the target GameObject is assigned in the clip.");
            }
            clone.TargetPosition = targetGO != null ? targetGO.transform.position : Vector3.zero;

            return playable;
        }
    }
}
