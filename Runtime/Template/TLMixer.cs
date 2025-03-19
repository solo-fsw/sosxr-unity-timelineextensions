using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class TLMixer : PlayableBehaviour
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


        protected virtual void ActiveBehaviour<T>(T trackBinding, TLBehaviour activeBehaviour, float easeWeight)
        {
            
        }
    }
}