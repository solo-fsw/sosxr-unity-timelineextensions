using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Stub mixer for the ToTarget track. Movement processing is handled by <see cref="ToTargetBehaviour.ProcessFrame"/>
    ///     directly, so this mixer has no additional logic.
    /// </summary>
    public class ToTargetMixer : PlayableBehaviour
    {
    }
}