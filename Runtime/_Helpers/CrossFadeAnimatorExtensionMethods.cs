using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations; // Needed for AnimatorController
#endif

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Extension methods on <see cref="Animator"/> for querying states, durations, and animation clips from the
    ///     underlying <c>AnimatorController</c>. Methods that access <c>AnimatorController</c> are guarded by
    ///     <c>#if UNITY_EDITOR</c> and return safe defaults at runtime.
    /// </summary>
    public static class CrossFadeAnimatorExtensionMethods
    {
        /// <summary>
        ///     Default duration (in seconds) returned when an animation state duration cannot be determined.
        ///     This provides a safe fallback for crossfade operations.
        /// </summary>
        private const float DefaultAnimationDuration = 5f;

        /// <summary>
        ///     Check if the Animator has a state with the given name (short name, or full path such as "Base Layer.Sub.Walk").
        ///     Works in builds and with AnimatorOverrideControllers, and finds states inside sub-state machines.
        ///     Only checks one layer (default: the first).
        /// </summary>
        public static bool HasState(this Animator animator, string stateName, int layerIndex = 0)
        {
            if (animator == null || string.IsNullOrEmpty(stateName) || animator.runtimeAnimatorController == null || layerIndex < 0)
            {
                return false;
            }

            if (animator.isInitialized && layerIndex < animator.layerCount)
            {
                if (animator.HasState(layerIndex, Animator.StringToHash(stateName)))
                {
                    return true;
                }

                if (animator.HasState(layerIndex, Animator.StringToHash(animator.GetLayerName(layerIndex) + "." + stateName)))
                {
                    return true;
                }
            }

#if UNITY_EDITOR
            // Not initialized (e.g. inactive GameObject in edit mode), or a nested state the runtime lookup missed:
            // fall back to reading the controller asset.
            var controller = GetAnimatorController(animator);

            if (controller == null || layerIndex >= controller.layers.Length)
            {
                return false;
            }

            foreach (var state in GetAllStates(controller.layers[layerIndex].stateMachine))
            {
                if (state.name == stateName)
                {
                    return true;
                }
            }
#endif

            return false;
        }

        /// <summary>
        ///     Checks whether the Animator is currently in the given state.
        ///     By default, it only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public static bool CurrentState(this Animator animator, string stateName, int layerIndex = 0)
        {
            if (animator == null)
            {
                Debug.LogWarning("Animator is null");

                return false;
            }

            if (!animator.HasState(stateName, layerIndex))
            {
                Debug.LogWarning("State: " + stateName + " not found, returning false");

                return false;
            }

            var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

            return stateInfo.IsName(stateName);
        }

        /// <summary>
        ///     Checks whether the animator can transition to the given state, by checking if the state exists and if the animator
        ///     is not already in that state.
        ///     By default, it only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public static bool CanTransitionTo(this Animator animator, string stateName, int layerIndex = 0)
        {
            if (!animator.HasState(stateName, layerIndex))
            {
                Debug.LogWarning("State: " + stateName + " not found, returning false");

                return false;
            }

            if (animator.CurrentState(stateName, layerIndex))
            {
                Debug.LogWarning("Already in state: " + stateName + ", returning false");

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Get the duration of the state with the given name.
        ///     By default, it only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static float GetStateDuration(this Animator animator, string stateName, int layerIndex = 0)
        {
#if UNITY_EDITOR
            if (!animator.HasState(stateName, layerIndex))
            {
                Debug.LogWarning("State : " + stateName + " not found, returning default duration of " + DefaultAnimationDuration);

                return DefaultAnimationDuration;
            }

            var controller = GetAnimatorController(animator);

            if (controller == null || layerIndex >= controller.layers.Length)
            {
                Debug.LogWarning("Animator controller is null or layer index is out of bounds, returning default duration of " + DefaultAnimationDuration);

                return DefaultAnimationDuration;
            }

            foreach (var state in GetAllStates(controller.layers[layerIndex].stateMachine))
            {
                if (state.name == stateName && state.motion != null)
                {
                    return state.motion.averageDuration;
                }
            }

            Debug.LogWarning("Returning default duration of " + DefaultAnimationDuration);
#endif

            return DefaultAnimationDuration;
        }

        /// <summary>
        ///     Get the names of all the states in the Animator.
        ///     By default, it only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static List<string> GetStateNames(this Animator animator, int layerIndex = 0)
        {
            List<string> stateNames = new()
            {
                ""
            };

#if UNITY_EDITOR
            if (animator == null)
            {
                Debug.LogWarning("Animator is null");

                return stateNames;
            }

            var controller = GetAnimatorController(animator);

            if (controller == null)
            {
                Debug.LogWarning("Animator controller is null");

                return stateNames;
            }

            if (layerIndex < 0 || layerIndex >= controller.layers.Length)
            {
                return stateNames;
            }

            foreach (var state in GetAllStates(controller.layers[layerIndex].stateMachine))
            {
                if (!stateNames.Contains(state.name))
                {
                    stateNames.Add(state.name);
                }
            }
#endif

            return stateNames;
        }

        /// <summary>
        ///     Get the AnimationClip for a state.
        ///     Returns null if not found or not an AnimationClip.
        /// </summary>
        public static AnimationClip GetStateAnimationClip(this Animator animator, string stateName, int layerIndex = 0)
        {
#if UNITY_EDITOR
            var controller = GetAnimatorController(animator);

            if (controller == null || layerIndex < 0 || layerIndex >= controller.layers.Length)
            {
                return null;
            }

            foreach (var state in GetAllStates(controller.layers[layerIndex].stateMachine))
            {
                if (state.name != stateName)
                {
                    continue;
                }

                AnimationClip clip = null;

                if (state.motion is AnimationClip stateClip)
                {
                    clip = stateClip;
                }
                else if (state.motion is BlendTree blendTree)
                {
                    var children = blendTree.children;

                    if (children.Length > 0 && children[0].motion is AnimationClip childClip)
                    {
                        clip = childClip;
                    }
                }

                // Respect clip overrides from an AnimatorOverrideController.
                if (clip != null && animator.runtimeAnimatorController is AnimatorOverrideController overrideController && overrideController[clip] != null)
                {
                    clip = overrideController[clip];
                }

                return clip;
            }
#endif
            return null;
        }

        /// <summary>
        ///     Check if a state loops its animation.
        ///     Returns true if loopTime is enabled or if state not found.
        /// </summary>
        public static bool DoesStateLoop(this Animator animator, string stateName, int layerIndex = 0)
        {
#if UNITY_EDITOR
            var clip = animator.GetStateAnimationClip(stateName, layerIndex);
            if (clip != null)
            {
                return clip.isLooping;
            }
#endif
            return true;
        }

        /// <summary>
        ///     Get the animation clip length for a state.
        ///     Returns 0 if not found.
        /// </summary>
        public static float GetStateAnimationLength(this Animator animator, string stateName, int layerIndex = 0)
        {
#if UNITY_EDITOR
            var clip = animator.GetStateAnimationClip(stateName, layerIndex);
            if (clip != null)
            {
                return clip.length;
            }
#endif
            return 0f;
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Which state is the one with the arrow from the Entry point in the Animator?
        ///     By default, it only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static AnimatorState GetDefaultEntryState(this Animator animator, int layerIndex = 0)
        {
            if (animator == null)
            {
                Debug.LogWarning("Animator is null");

                return null;
            }

            var controller = GetAnimatorController(animator);

            if (controller == null || layerIndex < 0 || layerIndex >= controller.layers.Length)
            {
                Debug.LogWarning("Animator controller is null or layer index is out of bounds");

                return null;
            }

            var stateMachine = controller.layers[layerIndex].stateMachine;

            return stateMachine.defaultState;
        }
#endif

        /// <summary>
        ///     Returns the name of the state with the arrow from the Entry point in the Animator.
        ///     By default, it only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static string GetDefaultEntryStateName(this Animator animator, int layerIndex = 0)
        {
            if (animator == null)
            {
                Debug.LogWarning("Animator is null");

                return "";
            }

            string stateName = "";

#if UNITY_EDITOR
            var state = animator.GetDefaultEntryState(layerIndex);
            stateName = state != null ? state.name : "";
#endif

            return stateName;
        }

#if UNITY_EDITOR
        /// <summary>
        ///     The AnimatorController behind the Animator, unwrapping any AnimatorOverrideControllers. Editor only.
        /// </summary>
        public static AnimatorController GetAnimatorController(Animator animator)
        {
            if (animator == null)
            {
                return null;
            }

            var runtimeController = animator.runtimeAnimatorController;

            while (runtimeController is AnimatorOverrideController overrideController)
            {
                runtimeController = overrideController.runtimeAnimatorController;
            }

            return runtimeController as AnimatorController;
        }

        /// <summary>
        ///     All states in a state machine, including those inside nested sub-state machines. Editor only.
        /// </summary>
        public static IEnumerable<AnimatorState> GetAllStates(AnimatorStateMachine stateMachine)
        {
            if (stateMachine == null)
            {
                yield break;
            }

            foreach (var childState in stateMachine.states)
            {
                if (childState.state != null)
                {
                    yield return childState.state;
                }
            }

            foreach (var childMachine in stateMachine.stateMachines)
            {
                foreach (var state in GetAllStates(childMachine.stateMachine))
                {
                    yield return state;
                }
            }
        }
#endif
    }
}
