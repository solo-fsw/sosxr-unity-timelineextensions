using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class RigClip : Clip
    {
        public WeightType WeightType = WeightType.Rig;

        public bool MatchWeightOnClipStart = true;

        [HideIf(nameof(WeightType), WeightType.Constraint)]
        [Range(0f, 1f)] public float RigWeight = 1f;

        [HideIf(nameof(WeightType), WeightType.Rig)]
        public ExposedReference<GameObject> Constraint;

        [HideIf(nameof(WeightType), WeightType.Rig)]
        [Range(0f, 1f)] public float ConstraintWeight = 1f;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<RigBehaviour>.Create(graph);
            var clone = playable.GetBehaviour();

            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            clone.RigWeight = RigWeight;
            clone.ConstraintObject = Constraint.Resolve(Resolver);
            clone.Constraint = clone.ConstraintObject?.GetComponent<IRigConstraint>();
            clone.ConstraintWeight = ConstraintWeight;
            clone.WeightType = WeightType;
            clone.MatchWeightOnClipStart = MatchWeightOnClipStart;

            return playable;
        }
    }


    public enum WeightType
    {
        Rig,
        Constraint
    }
}