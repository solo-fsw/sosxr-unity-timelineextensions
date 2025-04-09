using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public abstract class Mixer : PlayableBehaviour
    {
        private bool _registered = false;
        /// <summary>
        ///     Use this to get the object that the Track is bound to.
        ///     You usually want to cast it to the specific type of your binding.
        /// </summary>
        protected object TrackBinding { get; set; }

        #region Other Things

        /// <summary>
        ///     I'm hoping on that this doesn't need to get overriden in the actual implementation, and that I've covered most
        ///     use-cases in the ProcessingFrame and ActiveBehaviour methods.
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        /// <param name="playerData"></param>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            TrackBinding ??= playerData; // Here we set the TrackBinding, if it's not set yet. 

            ProcessingFrame(info.weight);

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var playableInput = (ScriptPlayable<Behaviour>) playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (_registered == false)
                {
                    behaviour.ClipStartedAction += ActiveBehaviourClipStart;
                    behaviour.ClipEndedAction += ActiveBehaviourClipEnd;
                }

                if (behaviour is {ClipActive: false})
                {
                    continue;
                }

                var easeWeight = playable.GetInputWeight(i); // Ranges from 0 to 1

                ActiveBehaviour(behaviour, easeWeight);
            }

            _registered = true;
        }

        #endregion


        /// <summary>
        ///     This gets called every frame, before the active Behaviour is found, and it's used for more 'general' processing
        ///     that is not specific for the Active Behaviour.
        ///     This gets called before the ActiveBehaviour method is called for each active Behaviour.
        /// </summary>
        /// <param name="easeWeight"></param>
        /// <param name="inputCount"></param>
        protected virtual void ProcessingFrame(float easeWeight)
        {
        }


        protected virtual void ActiveBehaviourClipStart(Behaviour activeBehaviour)
        {
        }


        /// <summary>
        ///     This is the main workhorse of the Mixer, where the active behaviour is processed.
        ///     It only gets called on any active Behaviour (when the time-scrubber is on the clip), so you don't need to check for
        ///     that.
        ///     This is also the place where you can use the handy functions for checking when the easing is starting / done etc.
        ///     See the base Behaviour class for more info on that.
        /// </summary>
        /// <param name="activeBehaviour"></param>
        /// <param name="easeWeight"></param>
        protected virtual void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
        }

        protected virtual void ActiveBehaviourClipEnd(Behaviour activeBehaviour)
        {
        }
    }
}