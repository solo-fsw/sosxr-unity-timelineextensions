using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Parenting track. Reparents the child Transform to the bound parent Transform when the clip starts,
    ///     and restores the original parent when the clip ends.
    /// </summary>
    public class ParentingMixer : Mixer
    {
        private Transform _parent;

        protected override void InitializeMixer(Playable playable)
        {
            _parent = (Transform)TrackBinding;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (TrackBinding == null)
            {
                Debug.LogError(
                    $"{GetType().Name}: There is nothing bound to this Track. Did you forget to set it?"
                );
                return;
            }

            ParentingBehaviour behaviour = activeBehaviour as ParentingBehaviour;
            behaviour.OriginalPosition = behaviour.Child.position;

            if (behaviour.KeepPosition == KeepPosition.Yes)
            {
                behaviour.Child.SetParent(_parent, true);
            }
            else if (behaviour.KeepPosition == KeepPosition.KeepWorldPosition)
            {
                behaviour.Child.SetParent(_parent, false);
            }
            else if (behaviour.KeepPosition == KeepPosition.ZeroOutCompletely)
            {
                behaviour.Child.position = Vector3.zero;
                behaviour.Child.SetParent(_parent, false);
            }
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (TrackBinding == null)
            {
                return;
            }

            ParentingBehaviour behaviour = activeBehaviour as ParentingBehaviour;
            behaviour.Child.position = behaviour.OriginalPosition;
            behaviour.Child.SetParent(behaviour.OriginalParent, true);
        }
    }
}
