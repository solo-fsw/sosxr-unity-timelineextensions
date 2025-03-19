using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public abstract class Mixer : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var playableInput = (ScriptPlayable<ExampleBehaviour>) playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (behaviour is not {ClipIsActive: true})
                {
                    continue;
                }

                var easeWeight = playable.GetInputWeight(i); // Ranges from 0 to 1

                ActiveBehaviour(playerData, behaviour, easeWeight);
            }
        }
        
        /// <summary>
        /// Cast trackBinding to the correct type 
        /// </summary>
        /// <param name="trackBinding"></param>
        /// <param name="activeBehaviour"></param>
        /// <param name="easeWeight"></param>
        /// <typeparam name="T"></typeparam>
        protected abstract void ActiveBehaviour<T>(T trackBinding, Behaviour activeBehaviour, float easeWeight);
    }
}