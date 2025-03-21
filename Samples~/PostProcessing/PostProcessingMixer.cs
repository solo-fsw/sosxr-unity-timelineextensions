namespace SOSXR.TimelineExtensions
{
    public class PostProcessingMixer : Mixer
    {
        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
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