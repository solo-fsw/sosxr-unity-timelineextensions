using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class PostProcessingMixer : Mixer
    {
        protected override void InitializeMixer(Playable playable)
        {
            //
        }


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not PostProcessingBehaviour behaviour)
            {
                return;
            }

            if (behaviour.Volume == null)
            {
                return;
            }

            behaviour.Volume.weight = easeWeight * behaviour.MaxWeight;
        }
    }
}