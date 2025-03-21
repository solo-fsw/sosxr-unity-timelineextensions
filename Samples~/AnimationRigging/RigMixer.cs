using UnityEngine.Animations.Rigging;


namespace SOSXR.TimelineExtensions
{
    public class RigMixer : Mixer
    {
        private Rig _rig;


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not RigBehaviour behaviour)
            {
                return;
            }

            _rig ??= behaviour.TrackBinding as Rig;

            if (_rig != null && behaviour.WeightType == WeightType.Rig)
            {
                if (behaviour.ClipStartedOnce && behaviour.MatchWeightOnClipStart)
                {
                    _rig.weight = behaviour.RigWeight;
                }

                _rig.weight = behaviour.RigWeight * easeWeight;
            }

            if (behaviour.Constraint != null && behaviour.WeightType == WeightType.Constraint)
            {
                if (behaviour.ClipStartedOnce && behaviour.MatchWeightOnClipStart)
                {
                    behaviour.Constraint.weight = behaviour.ConstraintWeight;
                }

                behaviour.Constraint.weight = behaviour.ConstraintWeight * easeWeight;
            }
        }
    }
}