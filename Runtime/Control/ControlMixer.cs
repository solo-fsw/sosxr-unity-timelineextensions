namespace SOSXR.TimelineExtensions
{
    public class ControlMixer : Mixer
    {
        protected IControl Interface { get; private set; }


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not ControlBehaviour behaviour)
            {
                return;
            }

            if (Interface == null)
            {
                return;
            }

            if (behaviour.ClipStartedOnce)
            {
                Interface.OnClipStart();
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

            if (behaviour.ClipEnd)
            {
                Interface.OnClipEnd();
            }
        }


        public void SetInterfaceTrackBinding(IControl interfaceTrackBinding)
        {
            Interface = interfaceTrackBinding;
        }
    }
}