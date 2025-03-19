using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class Mixer : PlayableBehaviour
    {
        protected object TrackBinding { get; set; }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            TrackBinding ??= playerData;

            ProcessingFrame();

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var playableInput = (ScriptPlayable<Behaviour>) playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (behaviour is not {ClipIsActive: true})
                {
                    continue;
                }

                var easeWeight = playable.GetInputWeight(i); // Ranges from 0 to 1

                ActiveBehaviour(behaviour, easeWeight);
            }
        }


        protected virtual void ProcessingFrame()
        {
            
        }


        protected virtual void ActiveBehaviour(Behaviour genericActiveBehaviour, float easeWeight)
        {
            
        }
    }
}