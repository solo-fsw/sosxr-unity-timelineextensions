using UnityEditor.Animations;
using UnityEngine;


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
        public static bool HasState(this Animator animator, string stateName)
        {
            var controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller == null)
            {
                return false;
            }

            foreach (var state in controller.layers[0].stateMachine.states)
            {
                if (state.state.name == stateName)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///     Checks whether the Animator is currently in the given state.
        ///     Only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public static bool CurrentState(this Animator animator, string stateName)
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

            foreach (var state in controller.layers[0].stateMachine.states)
            {
                if (state.state.name == stateName)
                {
                    return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == state.state.nameHash;
                }
            }

            return false;
        }


        /// <summary>
        ///     Checks whether the animator can transition to the given state, by checking if the state exists and if the animator
        ///     is not already in that state.
        ///     It only checks the first layer!
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public static bool CanTransitionTo(this Animator animator, string stateName)
        {
            if (!animator.HasState(stateName))
            {
                return false;
            }

            if (animator.CurrentState(stateName))
            {
                return false;
            }

            return true;
        }
    }
}