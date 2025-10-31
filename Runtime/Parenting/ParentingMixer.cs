using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class ParentingMixer : Mixer
    {
        private Transform _parent;


        protected override void InitializeMixer(Playable playable)
        {
            _parent = (Transform) TrackBinding;

            if (_parent == null)
            {
                Debug.LogWarning("ParentingMixer: Parent is null, did you forget to set it?");
            }
        }


        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as ParentingBehaviour;


            behaviour.Child.SetParent(_parent, !behaviour.ZeroInOnParent);
        }


        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as ParentingBehaviour;
            behaviour.Child.SetParent(behaviour.OriginalParent, !behaviour.ZeroInOnParent);
        }
    }
}