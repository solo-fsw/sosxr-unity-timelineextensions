using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class InterfaceMixer : Mixer
    {
        protected IInterface Interface { get; private set; }


        protected override void InitializeMixer(Playable playable)
        {
            var go = TrackBinding as GameObject;

            if (go == null)
            {
                Debug.LogWarning("TrackBinding is not a GameObject, did you forget to set it?");

                return;
            }

            if (go.TryGetComponent(out IInterface interfaceTB))
            {
                Interface = interfaceTB;
            }
            else
            {
                Debug.LogWarning("TrackBinding does not implement IInterface, did you forget to set it?");
            }
        }


        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            Interface.OnClipStart();
        }


        protected override void ClipEaseInDoneOnce(Behaviour activeBehaviour)
        {
            Interface.OnEaseInDone();
        }


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            Interface.ClipActive();
        }


        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            Interface.OnEaseOutStart();
        }


        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            Interface.OnClipEnd();
        }
    }
}