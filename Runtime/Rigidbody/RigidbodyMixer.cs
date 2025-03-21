using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class RigidbodyMixer : Mixer
    {
        private Rigidbody _rigidbody;
        /*public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            TrackBinding ??= playerData; // Here we set the TrackBinding, if it's not set yet.
            _rigidbody??= (Rigidbody) TrackBinding;

            ProcessingFrame(info.weight);

            var inputCount = playable.GetInputCount();

            var highestWeight = 0f;

            for (var i = 0; i < inputCount; i++)
            {
                var playableInput = (ScriptPlayable<Behaviour>) playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour() as RigidbodyBehaviour;

                if (behaviour is not {ClipActive: true})
                {
                    continue;
                }

                var easeWeight = playable.GetInputWeight(i); // Ranges from 0 to 1

                ActiveBehaviour(behaviour, easeWeight);

                if (easeWeight > highestWeight)
                {
                    _rigidbody.isKinematic = behaviour.isKinematic;
                    _rigidbody.useGravity = behaviour.useGravity;

                    behaviour.displacement = RigidbodyBehaviour.CalculateDisplacement(_rigidbody.transform, behaviour.target);
                    behaviour.direction = RigidbodyBehaviour.CalculateDirection(behaviour.displacement);

                    if (behaviour.addForce)
                    {
                        _rigidbody.AddForce(behaviour.direction * behaviour.amount, behaviour.forceMode);
                    }

                    highestWeight = easeWeight;
                }
            }


        }*/


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not RigidbodyBehaviour behaviour)
            {
                return;
            }

            _rigidbody ??= (Rigidbody) TrackBinding;

            if (behaviour.ClipStartedOnce)
            {
                _rigidbody.isKinematic = behaviour.isKinematic;
                _rigidbody.useGravity = behaviour.useGravity;

                if (behaviour.addForce)
                {
                    var displacement = CalculateDisplacement(_rigidbody.transform, behaviour.target);
                    var direction = CalculateDirection(displacement);
                    DrawRay(_rigidbody.transform, displacement);
                    _rigidbody.AddForce(direction * behaviour.amount, behaviour.forceMode);
                }
            }
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