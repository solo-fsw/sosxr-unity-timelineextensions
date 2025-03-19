using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class Mixer : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var trackBinding = playerData as Transform; // Transform is an example here

            if (trackBinding == null)
            {
                return;
            }

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var playableInput = (ScriptPlayable<Behaviour>) playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (behaviour is not {ClipStarted: true})
                {
                    continue;
                }
                
                if (behaviour.ClipStartedOnce)
                {
                    Debug.Log("Started");
                }
                
                if (behaviour.EaseInHasFinishedOnce)
                {
                    Debug.Log("Ease in finished");
                }
                
                if (behaviour.EaseOutHasStartedOnce)
                {
                    Debug.Log("Ease out has started");
                }
                
                if (behaviour.ClipIsDoneOnce)
                {
                    Debug.Log("Clip done");
                    
                    behaviour.ClipStarted = false;
                }
                
                Debug.LogWarning("Clippy");
            }
        }
    }
}