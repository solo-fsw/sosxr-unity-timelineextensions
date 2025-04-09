using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class RigidbodyMixer : Mixer
    {
        private Rigidbody _rigidbody;


        protected override void ActiveBehaviourClipStart(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not RigidbodyBehaviour behaviour)
            {
                return;
            }

            _rigidbody ??= (Rigidbody) TrackBinding;

            _rigidbody.isKinematic = behaviour.isKinematic;
            _rigidbody.useGravity = behaviour.useGravity;

            if (behaviour.addForce)
            {
                var displacement = CalculateDisplacement(_rigidbody.transform, behaviour.target);
                var direction = CalculateDirection(displacement);

                _rigidbody.AddForce(direction * behaviour.amount, behaviour.forceMode);
            }
        }


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not RigidbodyBehaviour behaviour)
            {
                return;
            }

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