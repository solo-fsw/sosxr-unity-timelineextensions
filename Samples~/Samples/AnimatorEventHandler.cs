using UnityEngine;

namespace SOSXR
{
    /// <summary>
    ///     Sample MonoBehaviour that responds to Animation Events fired from an Animator. Used in the demo scene to show
    ///     how Timeline-tracked animations can trigger script callbacks (e.g. footstep sounds).
    /// </summary>
    public class AnimatorEventHandler : MonoBehaviour
    {
        public void OnFootstep()
        {
            // Debug.Log("Step on you");
        }
    }
}
