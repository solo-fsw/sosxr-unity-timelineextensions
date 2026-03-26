using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the ToTarget track. Accumulates ease-weighted delta time each frame to drive a normalised 0–1 position
    ///     along the path, then lerps the bound object toward the effective target and slerps its rotation toward the travel
    ///     direction. Resets accumulation state when the clip ends.
    /// </summary>
    public class ToTargetMixer : Mixer
    {
        private GameObject _binding;

        protected override void InitializeMixer(Playable playable) { }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                var inputPlayable = (ScriptPlayable<ToTargetBehaviour>)playable.GetInput(i);
                var b = inputPlayable.GetBehaviour();

                if (b is not { ClipIsActive: true } || b.TotalEaseWeightIntegral <= 0)
                {
                    continue;
                }

                var easeWeight = playable.GetInputWeight(i);
                b.AccumulatedEaseTime += easeWeight * info.deltaTime;
                b.NormalizedPosition = Mathf.Clamp01(
                    b.AccumulatedEaseTime / b.TotalEaseWeightIntegral
                );
            }

            base.ProcessFrame(playable, info, playerData);
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            _binding ??= TrackBinding as GameObject;

            if (_binding == null || activeBehaviour is not ToTargetBehaviour b)
            {
                return;
            }

            if (!b.StartPositionCaptured)
            {
                b.StartPosition = _binding.transform.position;
                b.StartPositionCaptured = true;
            }

            var displacement = b.TargetPosition - b.StartPosition;

            if (b.AxisToUse.x == 0)
            {
                displacement.x = 0;
            }

            if (b.AxisToUse.y == 0)
            {
                displacement.y = 0;
            }

            if (b.AxisToUse.z == 0)
            {
                displacement.z = 0;
            }

            var effectiveTarget = b.StartPosition + displacement;

            _binding.transform.position = Vector3.Lerp(
                b.StartPosition,
                effectiveTarget,
                b.NormalizedPosition
            );

            if (displacement != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(displacement.normalized);
                _binding.transform.rotation = Quaternion.Slerp(
                    _binding.transform.rotation,
                    targetRotation,
                    b.RotateSpeed * Time.deltaTime
                );
            }

#if UNITY_EDITOR
            Debug.DrawRay(
                _binding.transform.position,
                effectiveTarget - _binding.transform.position,
                Color.cyan
            );
#endif
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not ToTargetBehaviour b)
            {
                return;
            }

            b.StartPositionCaptured = false;
            b.AccumulatedEaseTime = 0f;
            b.NormalizedPosition = 0f;
        }
    }
}
