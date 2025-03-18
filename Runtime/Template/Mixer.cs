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

                if (behaviour == null)
                {
                    continue;
                }

                var weight = playable.GetInputWeight(i);

                if (weight <= 0) // If the weight is 0, then the clip is not active
                {
                    continue;
                }

                var activeBehaviour = behaviour; // Rename this variable to something more meaningful

                // Do something with the active behaviour
            }
        }
    }
}