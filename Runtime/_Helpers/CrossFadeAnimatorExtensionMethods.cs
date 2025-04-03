using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations; // Needed for AnimatorController
#endif


namespace SOSXR.TimelineExtensions
{
    public static class CrossFadeAnimatorExtensionMethods
    {
        private const float _defaultDuration = 5;


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
                Debug.LogWarning("Animator controller is null");

                return false;
            }

            foreach (var state in controller.layers[layerIndex].stateMachine.states)
            {
                if (state.state.name == stateName)
                {
                    return true;
                }
            }

            return false;
            #else
            return false;
            #endif
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

            if (!animator.HasState(stateName))
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
            if (!animator.HasState(stateName))
            {
                Debug.LogWarning("State : " + stateName + " not found, returning default duration of " + _defaultDuration);

                return _defaultDuration;
            }

            var controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller == null || layerIndex >= controller.layers.Length)
            {
                Debug.LogWarning("Animator controller is null or layer index is out of bounds, returning default duration of " + _defaultDuration);

                return _defaultDuration;
            }

            foreach (var state in controller.layers[layerIndex].stateMachine.states)
            {
                if (state.state.name == stateName && state.state.motion != null)
                {
                    return state.state.motion.averageDuration;
                }
            }

            Debug.LogWarning("Returning default duration of " + _defaultDuration);
            #endif
            
            return _defaultDuration;
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
            var stateNames = new List<string>();
            stateNames.Add("");
            
            #if UNITY_EDITOR
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
            #endif

            return stateNames;
        }


        /*/// <summary>
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
                    Debug.LogWarning("We found state " + stateName + " and it is looping: " + state.state.motion.isLooping);

                    return state.state.motion.isLooping;
                }
            }

            return false;
        }
        */

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

            if (animator.runtimeAnimatorController is not AnimatorController controller)
            {
                Debug.LogWarning("Animator controller is null");

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

            var stateName = "";

            #if UNITY_EDITOR
            var state = animator.GetDefaultEntryState(layerIndex);
            stateName = state.name;
            #endif

            return stateName;
        }
    }
}