namespace SOSXR.TimelineExtensions
{
    public class RotateToTargetMixer : Mixer
    {
        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not RotateToTargetBehaviour behaviour)
            {
                return;
            }

            behaviour.CalculateValues();

            behaviour.DrawRayInRotationDirection();

            behaviour.HandleSmoothRotation(behaviour.DirectionToTarget);
        }
    }
}