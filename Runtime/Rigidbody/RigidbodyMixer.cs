using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class RigidbodyMixer : Mixer
    {
        private Rigidbody _rigidbody;


        protected override void InitializeMixer(Playable playable)
        {
            _rigidbody ??= (Rigidbody) TrackBinding;

            if (_rigidbody == null)
            {
                Debug.LogWarning("RigidbodyMixer: TrackBinding is not a Rigidbody, did you forget to set it?");
            }
        }


        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as RigidbodyBehaviour;

            _rigidbody.isKinematic = behaviour.isKinematic;
            _rigidbody.useGravity = behaviour.useGravity;

            if (behaviour.addForce)
            {
                var displacement = CalculateDisplacement(_rigidbody.transform, behaviour.target);
                var direction = CalculateDirection(displacement);

                _rigidbody.AddForce(direction * behaviour.amount, behaviour.forceMode);
            }
        }


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            var behaviour = activeBehaviour as RigidbodyBehaviour;

            var displacement = CalculateDisplacement(_rigidbody.transform, behaviour.target);
            DrawRay(_rigidbody.transform, displacement);
        }


        /// <summary>
        ///     How far & in what direction do I need to go?
        /// </summary>
        /// <returns></returns>
        public static Vector3 CalculateDisplacement(Transform originTrans, Transform targetTrans)
        {
            return targetTrans.position - originTrans.position;
        }


        /// <summary>
        ///     Creates Vector with max 1
        /// </summary>
        /// <param name="displacement"></param>
        /// <returns></returns>
        public static Vector3 CalculateDirection(Vector3 displacement)
        {
            return displacement.normalized;
        }


        private static void DrawRay(Transform originTrans, Vector3 displacement)
        {
            Debug.DrawRay(originTrans.position, displacement);
        }
    }
}