using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Rigidbody track. Applies kinematic and gravity settings on clip start, optionally fires a force
    ///     impulse toward a target, and draws a debug ray toward the target each frame.
    /// </summary>
    public class RigidbodyMixer : Mixer
    {
        public Rigidbody Binding;

        protected override void InitializeMixer(Playable playable)
        {
            Binding ??= (Rigidbody)TrackBinding;

            if (Binding == null)
            {
                Debug.LogWarning($"{GetType().Name}: There is nothing bound to this Track. Did you forget to set it??");
            }
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not RigidbodyBehaviour behaviour)
            {
                return;
            }
            if (Binding == null)
            {
                return;
            }

            Binding.isKinematic = behaviour.IsKinematic;
            Binding.useGravity = behaviour.UseGravity;

            if (behaviour.AddForce && behaviour.Target != null)
            {
                var displacement = CalculateDisplacement(Binding.transform, behaviour.Target);
                var direction = CalculateDirection(displacement);

                Binding.AddForce(direction * behaviour.Amount, behaviour.ForceMode);
            }
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not RigidbodyBehaviour behaviour)
            {
                return;
            }
            if (Binding == null)
            {
                return;
            }
            if (behaviour.Target == null)
            {
                return;
            }

            var displacement = CalculateDisplacement(Binding.transform, behaviour.Target);
            DrawRay(Binding.transform, displacement);
        }

        /// <summary>
        ///     How far & in what direction do I need to go?
        /// </summary>
        /// <returns></returns>
        public static Vector3 CalculateDisplacement(Transform originTrans, Transform targetTrans) => targetTrans.position - originTrans.position;

        /// <summary>
        ///     Creates Vector with max 1
        /// </summary>
        /// <param name="displacement"></param>
        /// <returns></returns>
        public static Vector3 CalculateDirection(Vector3 displacement) => displacement.normalized;

        private static void DrawRay(Transform originTrans, Vector3 displacement) => Debug.DrawRay(originTrans.position, displacement);
    }
}
