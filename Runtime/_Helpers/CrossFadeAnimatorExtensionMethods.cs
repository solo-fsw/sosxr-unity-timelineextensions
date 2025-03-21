using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations; // Needed for AnimatorController
#endif


namespace SOSXR.TimelineExtensions
{
    public static class CrossFadeAnimatorExtensionMethods
    {
        /// <summary>
        ///     Check if the Animator has a state with the given name.
        ///     Only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public static bool HasState(this Animator animator, string stateName, int layerIndex = 0)
        {
            #if UNITY_EDITOR
            var controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller == null)
            {
                return false;
            }

            foreach (var state in controller.layers[layerIndex].stateMachine.states)
            {
                if (state.state.name == stateName)
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
            #if UNITY_EDITOR
            if (!animator.HasState(stateName))
            {
                return false;
            }

            var controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller == null)
            {
                return false;
            }

            foreach (var state in controller.layers[layerIndex].stateMachine.states)
            {
                if (state.state.name == stateName)
                {
                    return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == state.state.nameHash;
                }
            }
            #endif
            return false;
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
                return false;
            }

            if (animator.CurrentState(stateName, layerIndex))
            {
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
            if (!animator.HasState(stateName))
            {
                return 0;
            }

            var controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller == null || layerIndex >= controller.layers.Length)
            {
                return 0;
            }

            foreach (var state in controller.layers[layerIndex].stateMachine.states)
            {
                if (state.state.name == stateName && state.state.motion != null)
                {
                    return state.state.motion.averageDuration;
                }
            }
            #endif
            return 0;
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
            #if UNITY_EDITOR
            var stateNames = new List<string>();
            stateNames.Add("NONE");

            if (animator == null)
            {
                Debug.LogWarning("Animator is null");

                return stateNames;
            }

            if (animator.runtimeAnimatorController is not AnimatorController controller)
            {
                Debug.LogWarning("Animator controller is null");

                return stateNames;
            }


            foreach (var state in controller.layers[layerIndex].stateMachine.states)
            {
                stateNames.Add(state.state.name);
            }

            return stateNames;
            #endif

            return null;
        }


        /// <summary>
        ///     Which state is the one with the arrow from the Entry point in the Animator?
        ///     By default, it only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static AnimatorState GetDefaultEntryState(this Animator animator, int layerIndex = 0)
        {
            #if UNITY_EDITOR
            if (animator == null)
            {
                Debug.LogWarning("Animator is null");

                return null;
            }

            if (animator.runtimeAnimatorController is not AnimatorController controller)
            {
                Debug.LogWarning("Animator controller is null");

                return null;
            }

            var stateMachine = controller.layers[layerIndex].stateMachine;

            return stateMachine.defaultState;
            #endif

            return null;
        }


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

            var state = animator.GetDefaultEntryState(layerIndex);

            return state.name;
        }


        /// <summary>
        ///     Checks whether the state with the given name is looping.
        ///     This is useful in combination with the StateDuration method.
        ///     By default, it only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static bool IsLooping(this Animator animator, string stateName, int layerIndex = 0)
        {
            #if UNITY_EDITOR
            if (!animator.HasState(stateName))
            {
                return false;
            }

            var controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller == null)
            {
                return false;
            }

            foreach (var state in controller.layers[layerIndex].stateMachine.states)
            {
                if (state.state.name == stateName)
                {
                    return state.state.motion.isLooping;
                }
            }
            #endif
            return false;
        }
    }
}