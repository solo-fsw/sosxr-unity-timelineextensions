using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace SOSXR.TimelineExtensions
{
    public class RigMixer : Mixer
    {
        private Rig _rig;


        protected override void ActiveBehaviourClipStart(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not RigBehaviour behaviour)
            {
                return;
            }

            _rig ??= behaviour.TrackBinding as Rig;

            if (!behaviour.MatchWeightOnClipStart)
            {
                return;
            }

            if (_rig != null && behaviour.WeightType == WeightType.Rig)
            {
                Debug.LogWarning("This is not working as expected.");
                _rig.weight = behaviour.RigWeight;
            }

            if (behaviour.Constraint != null && behaviour.WeightType == WeightType.Constraint)
            {
                Debug.LogWarning("This is not working as expected.");
                behaviour.ConstraintWeight = behaviour.Constraint.weight;
            }
        }


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not RigBehaviour behaviour)
            {
                return;
            }

            _rig ??= behaviour.TrackBinding as Rig;

            if (_rig != null && behaviour.WeightType == WeightType.Rig)
            {
                _rig.weight = behaviour.RigWeight * easeWeight;
            }

            if (behaviour.Constraint != null && behaviour.WeightType == WeightType.Constraint)
            {
                behaviour.Constraint.weight = behaviour.ConstraintWeight * easeWeight;
            }
        }
    }
}