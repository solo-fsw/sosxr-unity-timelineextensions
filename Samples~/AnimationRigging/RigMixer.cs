using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class RigMixer : Mixer
    {
        private Rig _rig;


        protected override void InitializeMixer(Playable playable)
        {
            _rig ??= TrackBinding as Rig;

            if (_rig == null)
            {
                Debug.LogWarning("RigMixer: TrackBinding is not a Rig, did you forget to set it?");
            }
        }


        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as RigBehaviour;


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


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            var behaviour = activeBehaviour as RigBehaviour;

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