using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Rigidbody track.
    ///     <para>
    ///         On clip start: applies <c>isKinematic</c> / <c>useGravity</c> settings, and fires a one-shot force if
    ///         <see cref="ForceMode.Impulse" /> is selected.
    ///     </para>
    ///     <para>
    ///         Every frame the clip is active: continuous force modes (<see cref="ForceMode.Force" />,
    ///         <see cref="ForceMode.Acceleration" />, <see cref="ForceMode.VelocityChange" />) apply force toward the target,
    ///         scaled by the current <c>easeWeight</c> so the force ramps in/out with the clip's ease curves and blends
    ///         correctly when two clips overlap. The direction is recalculated each frame so moving targets are tracked.
    ///     </para>
    /// </summary>
    public class RigidbodyMixer : Mixer
    {
        public Rigidbody Binding;

        protected override void InitializeMixer(Playable playable)
        {
            Binding ??= (Rigidbody)TrackBinding;

            if (Binding == null)
            {
                Debug.LogWarning(
                    $"{GetType().Name}: There is nothing bound to this Track. Did you forget to set it??"
                );
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

            if (
                behaviour.AddForce
                && behaviour.Target != null
                && behaviour.ForceMode == ForceMode.Impulse
            )
            {
                var displacement = CalculateDisplacement(Binding.transform, behaviour.Target);
                var direction = CalculateDirection(displacement);
                Binding.AddForce(direction * behaviour.Amount, ForceMode.Impulse);
            }
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (Binding == null)
            {
                return;
            }

            if (activeBehaviour is not RigidbodyBehaviour behaviour)
            {
                return;
            }

            if (
                !behaviour.AddForce
                || behaviour.Target == null
                || behaviour.ForceMode == ForceMode.Impulse
            )
            {
                if (behaviour.Target != null)
                {
                    DrawRay(
                        Binding.transform,
                        CalculateDisplacement(Binding.transform, behaviour.Target)
                    );
                }

                return;
            }

            var disp = CalculateDisplacement(Binding.transform, behaviour.Target);
            var dir = CalculateDirection(disp);

            DrawRay(Binding.transform, disp);

            // Scale by easeWeight so the force:
            //   • ramps in smoothly during ease-in  (0 → 1)
            //   • holds at full strength during the clip body  (≈ 1)
            //   • ramps out during ease-out  (1 → 0)
            // When two clips overlap, Timeline drives each clip's easeWeight independently
            // and ClipActive is called once per clip, so blending is automatic.
            Binding.AddForce(dir * behaviour.Amount * easeWeight, behaviour.ForceMode);
        }

        /// <summary>How far and in what direction from <paramref name="originTrans"/> to <paramref name="targetTrans"/>.</summary>
        public static Vector3 CalculateDisplacement(Transform originTrans, Transform targetTrans) =>
            targetTrans.position - originTrans.position;

        /// <summary>Returns the normalized direction of <paramref name="displacement"/> (magnitude clamped to 1).</summary>
        public static Vector3 CalculateDirection(Vector3 displacement) => displacement.normalized;

        private static void DrawRay(Transform originTrans, Vector3 displacement)
        {
            Debug.DrawRay(originTrans.position, displacement);
        }
    }
}
