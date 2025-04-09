using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ParentingMixer : Mixer
    {
        private Transform _parent;

        private ParentingBehaviour _behaviour;


        protected override void ActiveBehaviourClipStart(Behaviour activeBehaviour)
        {
            _behaviour = activeBehaviour as ParentingBehaviour;

            _parent ??= _behaviour.TrackBinding as Transform;

            _behaviour.Child.SetParent(_parent, !_behaviour.ZeroInOnParent);
        }


        protected override void ActiveBehaviourClipEnd(Behaviour activeBehaviour)
        {
            _behaviour.Child.SetParent(_behaviour.OriginalParent, !_behaviour.ZeroInOnParent);
        }
    }
}