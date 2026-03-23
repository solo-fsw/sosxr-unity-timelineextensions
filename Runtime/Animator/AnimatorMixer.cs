using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Binding track. Calls <c>Binding.CrossFadeInFixedTime</c> to the start state when a clip begins,
    ///     and to the end state once ease-out starts.
    /// </summary>
    public class AnimatorMixer : Mixer
    {
        public Animator Binding;

        public static int ActiveClips { get; private set; }
        protected override void InitializeMixer(Playable playable)
        {
            Binding = (Animator)TrackBinding;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            ActiveClips++;

            if (!Binding.HasState(behaviour.StartClipStateName))
            {
                Debug.LogWarning($"Our bound Animator does not have state named '{behaviour.StartClipStateName}' in it's Animator Cotroller. Is it properly set, and is it on the first (0) layer?");
                return;
            }

            // if (ActiveClips > 1)
            // {
            //     Debug.Log("HOT START Too hot for me, not running right now");
            //     return;
            // }

            Binding?.CrossFadeInFixedTime(behaviour.StartClipStateName, behaviour.EaseInDuration, 0);
            Debug.Log("Start cross at clip start");

        }

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            if (behaviour.EaseOutDuration <= 0)
            {
                return;
            }
            if (!Binding.HasState(behaviour.EndClipStateName))
            {
                Debug.LogWarning($"Our bound Animator does not have state named '{behaviour.EndClipStateName}' in it's Animator Controller. Is it properly set, and is it on the first (0) layer?");
                return;
            }

            if (ActiveClips > 1)
            {
                Debug.Log("Too hot for me, not running right now");
                return;
            }

            Binding?.CrossFadeInFixedTime(behaviour.EndClipStateName, behaviour.EaseOutDuration, 0);
            Debug.Log("Cross at fade");
        }


        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            ActiveClips--;

            if (behaviour.EaseOutDuration > 0)
            {
                return;
            }

            if (!Binding.HasState(behaviour.EndClipStateName))
            {
                Debug.LogWarning($"Our bound Animator does not have state named '{behaviour.EndClipStateName}' in it's Animator Controller. Is it properly set, and is it on the first (0) layer?");
                return;
            }

            if (ActiveClips > 1)
            {
                Debug.Log("Too hot for me, not running right now");
                return;
            }

            Binding?.CrossFadeInFixedTime(behaviour.EndClipStateName, behaviour.EaseOutDuration, 0);
            Debug.Log("Cross at end");
        }
    }
}
