using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ParentingMixer : Mixer
    {
        private Transform _parent;


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not ParentingBehaviour behaviour)
            {
                return;
            }

            _parent ??= behaviour.TrackBinding as Transform;

            if (behaviour.ClipStartedOnce)
            {
                behaviour.Child.SetParent(_parent, !behaviour.ZeroInOnParent);
            }

            if (behaviour.ClipEnd)
            {
                behaviour.Child.SetParent(behaviour.OriginalParent, !behaviour.ZeroInOnParent);
            }
        }
    }
}