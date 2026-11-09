using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Rigidbody track.
    ///     <para>
    ///         On clip start: applies <c>isKinematic</c> / <c>useGravity</c> settings, and fires a one-shot force if
    ///         <see cref="ForceMode.Impulse" /> or <see cref="ForceMode.VelocityChange" /> is selected.
    ///     </para>
    ///     <para>
    ///         Every frame the clip is active: continuous force modes (<see cref="ForceMode.Force" />,
    ///         <see cref="ForceMode.Acceleration" />) apply force toward the target, scaled by the current <c>easeWeight</c>
    ///         and by the frame's share of a physics step, so the total push does not depend on the frame rate.
    ///     </para>
    ///     <para>
    ///         When the last active clip ends, <c>isKinematic</c> / <c>useGravity</c> are restored to the values they had
    ///         before the first clip started, unless <see cref="RigidbodyTrack.RestoreStateOnEnd" /> is off.
    ///     </para>
    /// </summary>
    public class RigidbodyMixer : Mixer
    {
        public Rigidbody Binding;

        /// <summary>Restore isKinematic / useGravity after the last clip. Set by the track.</summary>
        public bool RestoreStateOnEnd = true;

        private int _activeClipCount;
        private bool _hasStoredState;
        private bool _originalIsKinematic;
        private bool _originalUseGravity;

        protected override void InitializeMixer(Playable playable)
        {
            Binding ??= TrackBinding as Rigidbody;

            if (Binding == null)
            {
                Debug.LogWarning($"{TypeName}: There is nothing bound to this Track. Did you forget to set it??");
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

            if (_activeClipCount == 0 && !_hasStoredState)
            {
                _originalIsKinematic = Binding.isKinematic;
                _originalUseGravity = Binding.useGravity;
                _hasStoredState = true;
            }

            _activeClipCount++;

            Binding.isKinematic = behaviour.IsKinematic;
            Binding.useGravity = behaviour.UseGravity;

            if (behaviour.AddForce && behaviour.Target != null && IsOneShot(behaviour.ForceMode))
            {
                var displacement = CalculateDisplacement(Binding.transform, behaviour.Target);
                var direction = CalculateDirection(displacement);
                Binding.AddForce(direction * behaviour.Amount, behaviour.ForceMode);
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

            if (!behaviour.AddForce || behaviour.Target == null || IsOneShot(behaviour.ForceMode))
            {
                if (behaviour.Target != null)
                {
                    DrawRay(Binding.transform, CalculateDisplacement(Binding.transform, behaviour.Target));
                }

                return;
            }

            var disp = CalculateDisplacement(Binding.transform, behaviour.Target);
            var dir = CalculateDirection(disp);

            DrawRay(Binding.transform, disp);

            // Scale by easeWeight so the force ramps in / holds / ramps out with the clip's ease curves.
            // Scale by the frame's share of a physics step, because forces added per frame are only applied once per physics step.
            var frameScale = ContinuousForceScale(FrameDeltaTime, Time.fixedDeltaTime);
            Binding.AddForce(dir * (behaviour.Amount * easeWeight * frameScale), behaviour.ForceMode);
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not RigidbodyBehaviour)
            {
                return;
            }

            if (_activeClipCount > 0)
            {
                _activeClipCount--;
            }

            if (_activeClipCount > 0 || !_hasStoredState)
            {
                return;
            }

            _hasStoredState = false;

            if (!RestoreStateOnEnd || Binding == null)
            {
                return;
            }

            Binding.isKinematic = _originalIsKinematic;
            Binding.useGravity = _originalUseGravity;
        }

        /// <summary>Impulse and VelocityChange are instantaneous, so they fire once on clip start.</summary>
        public static bool IsOneShot(ForceMode mode)
        {
            return mode is ForceMode.Impulse or ForceMode.VelocityChange;
        }

        /// <summary>
        ///     Multiplier for a continuous force added once per rendered frame. A force added via AddForce acts for one
        ///     physics step, so scaling by deltaTime / fixedDeltaTime gives the same total impulse at any frame rate.
        /// </summary>
        public static float ContinuousForceScale(float frameDeltaTime, float fixedDeltaTime)
        {
            if (fixedDeltaTime <= 0f || frameDeltaTime <= 0f)
            {
                return 0f;
            }

            return frameDeltaTime / fixedDeltaTime;
        }

        /// <summary>How far and in what direction from <paramref name="originTrans"/> to <paramref name="targetTrans"/>.</summary>
        public static Vector3 CalculateDisplacement(Transform originTrans, Transform targetTrans) => targetTrans.position - originTrans.position;

        /// <summary>Returns the normalized direction of <paramref name="displacement"/> (magnitude clamped to 1).</summary>
        public static Vector3 CalculateDirection(Vector3 displacement) => displacement.normalized;

        private static void DrawRay(Transform originTrans, Vector3 displacement)
        {
            Debug.DrawRay(originTrans.position, displacement);
        }
    }
}
