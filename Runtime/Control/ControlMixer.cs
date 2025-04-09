using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ControlMixer : Mixer
    {
        protected IControl Interface { get; private set; }

        protected override void ActiveBehaviourClipStart(Behaviour activeBehaviour)
        {
            Interface.OnClipStart();
        }


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not ControlBehaviour behaviour)
            {
                return;
            }

            if (Interface == null)
            {
                Debug.LogWarning("Interface is null, did you forget to set it?");
                return;
            }

            if (behaviour.EaseInDoneOnce)
            {
                Interface.OnEaseInDone();
            }

            if (behaviour.ClipActive)
            {
                Interface.ClipActive();
            }

            if (behaviour.EaseOutStartedOnce)
            {
                Interface.OnEaseOutStart();
            }
        }


        public void SetInterfaceTrackBinding(IControl interfaceTrackBinding)
        {
            Interface = interfaceTrackBinding;
        }


        protected override void ActiveBehaviourClipEnd(Behaviour activeBehaviour)
        {
            Interface.OnClipEnd();
        }
    }
}