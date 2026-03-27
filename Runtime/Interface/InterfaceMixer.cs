using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Interface track. Casts the bound <see cref="Component"/> directly to <see cref="IInterface"/>
    ///     and forwards all clip lifecycle events to the corresponding interface methods.
    /// </summary>
    public class InterfaceMixer : Mixer
    {
        protected IInterface Interface { get; private set; }

        protected override void InitializeMixer(Playable playable)
        {
            Interface = TrackBinding as IInterface;

            if (Interface == null)
            {
                Debug.LogWarning(
                    $"TrackBinding does not implement {nameof(IInterface)}, did you forget to add it?"
                );
            }
        }

        protected override void ClipStarted(Behaviour activeBehaviour) => Interface?.OnClipStart();

        protected override void ClipEaseInDoneOnce(Behaviour activeBehaviour) =>
            Interface?.OnEaseInDone();

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            Interface?.ClipActive(easeWeight);
        }

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour) =>
            Interface?.OnEaseOutStart();

        protected override void ClipEnd(Behaviour activeBehaviour) => Interface?.OnClipEnd();
    }
}
