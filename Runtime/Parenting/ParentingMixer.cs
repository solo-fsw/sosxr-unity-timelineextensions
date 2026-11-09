using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Parenting track. Reparents the child Transform to the bound parent Transform when the clip starts,
    ///     and restores the original parent, world position and world rotation when the clip ends.
    /// </summary>
    public class ParentingMixer : Mixer
    {
        private Transform _parent;

        protected override void InitializeMixer(Playable playable)
        {
            _parent = TrackBinding as Transform;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (_parent == null)
            {
                Debug.LogError($"{TypeName}: There is nothing bound to this Track. Did you forget to set it?");

                return;
            }

            if (activeBehaviour is not ParentingBehaviour behaviour)
            {
                return;
            }

            if (behaviour.Child == null)
            {
                Debug.LogWarning($"{TypeName}: The clip has no Child Transform assigned. Nothing to reparent.");

                return;
            }

            behaviour.OriginalParent = behaviour.Child.parent;
            behaviour.OriginalPosition = behaviour.Child.position;
            behaviour.OriginalRotation = behaviour.Child.rotation;
            behaviour.IsAttached = true;

            Attach(behaviour.Child, _parent, behaviour.KeepPosition);
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not ParentingBehaviour behaviour || !behaviour.IsAttached)
            {
                return;
            }

            behaviour.IsAttached = false;

            if (behaviour.Child == null)
            {
                return;
            }

            Detach(behaviour.Child, behaviour.OriginalParent, behaviour.OriginalPosition, behaviour.OriginalRotation);
        }

        /// <summary>Parents <paramref name="child"/> to <paramref name="parent"/> using the chosen <see cref="KeepPosition"/> mode.</summary>
        public static void Attach(Transform child, Transform parent, KeepPosition mode)
        {
            switch (mode)
            {
                case KeepPosition.Yes:
                case KeepPosition.KeepWorldPosition:
                    child.SetParent(parent, true);

                    break;
                case KeepPosition.KeepLocalPosition:
                    child.SetParent(parent, false);

                    break;
                case KeepPosition.ZeroOutCompletely:
                    child.SetParent(parent, false);
                    child.localPosition = Vector3.zero;
                    child.localRotation = Quaternion.identity;

                    break;
            }
        }

        /// <summary>Puts <paramref name="child"/> back under its original parent, at its original world position and rotation.</summary>
        public static void Detach(Transform child, Transform originalParent, Vector3 originalPosition, Quaternion originalRotation)
        {
            child.SetParent(originalParent, true);
            child.SetPositionAndRotation(originalPosition, originalRotation);
        }
    }
}
