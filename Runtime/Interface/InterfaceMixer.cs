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
        /// <summary>The track binding cast to <see cref="IInterface"/>. Null when the binding does not implement it.</summary>
        protected IInterface Interface { get; private set; }

        /// <summary>Casts the track binding to <see cref="IInterface"/> and logs a warning when it does not implement it.</summary>
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

        /// <summary>Forwards the clip start to <see cref="IInterface.OnClipStart(float)"/> with the clip's mix-in duration.</summary>
        protected override void ClipStarted(Behaviour activeBehaviour) => Interface?.OnClipStart(activeBehaviour.MixInDuration);

        /// <summary>Forwards the ease-in completion to <see cref="IInterface.OnEaseInDone()"/>.</summary>
        protected override void ClipEaseInDoneOnce(Behaviour activeBehaviour) =>
            Interface?.OnEaseInDone();

        /// <summary>Forwards the per-frame ease weight to <see cref="IInterface.ClipActive(float)"/>.</summary>
        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            Interface?.ClipActive(easeWeight);
        }

        /// <summary>Forwards the ease-out start to <see cref="IInterface.OnEaseOutStart(float)"/> with the clip's mix-out duration.</summary>
        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour) =>
            Interface?.OnEaseOutStart(activeBehaviour.MixOutDuration);

        /// <summary>Forwards the clip end to <see cref="IInterface.OnClipEnd(bool)"/>, passing whether the clip reached its end.</summary>
        protected override void ClipEnd(Behaviour activeBehaviour) => Interface?.OnClipEnd(activeBehaviour.ReachedEnd);
    }
}
