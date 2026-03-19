using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Interface track. Resolves the <see cref="IInterface"/> component on the bound <see cref="GameObject"/>
    ///     and forwards all clip lifecycle events to the corresponding interface methods.
    /// </summary>
    public class InterfaceMixer : Mixer
    {
        protected IInterface Interface { get; private set; }

        protected override void InitializeMixer(Playable playable)
        {
            GameObject go = TrackBinding as GameObject;

            if (go == null)
            {
                Debug.LogWarning("TrackBinding is not a GameObject, did you forget to set it?");

                return;
            }

            if (go.TryGetComponent(out IInterface interf))
            {
                Interface = interf;
            }
            else
            {
                Debug.LogWarning($"TrackBinding does not implement {nameof(IInterface)}, did you forget to add it?");
            }
        }

        protected override void ClipStarted(Behaviour activeBehaviour) => Interface?.OnClipStart();

        protected override void ClipEaseInDoneOnce(Behaviour activeBehaviour) => Interface?.OnEaseInDone();

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight) => Interface?.ClipActive();

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour) => Interface?.OnEaseOutStart();

        protected override void ClipEnd(Behaviour activeBehaviour) => Interface?.OnClipEnd();
    }
}
