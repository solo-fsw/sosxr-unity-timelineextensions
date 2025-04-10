using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public abstract class Mixer : PlayableBehaviour
    {
        /// <summary>
        ///     Use this to get the object that the Track is bound to.
        ///     You usually want to cast it to the specific type of your binding.
        /// </summary>
        public object TrackBinding { get; set; }


        public override void OnGraphStart(Playable playable)
        {
            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var playableInput = (ScriptPlayable<Behaviour>) playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                behaviour.ClipStartedAction += ClipStarted;
                behaviour.ClipEaseInDoneOnceAction += ClipEaseInDoneOnce;
                behaviour.ClipEaseOutStartedOnceAction += ClipEaseOutStartedOnce;
                behaviour.ClipEndedAction += ClipEnd;
            }

            InitializeMixer(playable);
        }


        protected abstract void InitializeMixer(Playable playable);


        /// <summary>
        ///     This is called when the clip starts playing.
        /// </summary>
        /// <param name="activeBehaviour"></param>
        protected virtual void ClipStarted(Behaviour activeBehaviour)
        {
        }


        protected virtual void ClipEaseInDoneOnce(Behaviour activeBehaviour)
        {
        }


        /// <summary>
        ///     This is the main workhorse of the Mixer, where the active behaviour is processed.
        ///     It only gets called on any active Behaviour (when the playhead / time-scrubber is on the clip), so you don't need to check for that.
        ///     This is also the place where you can use the handy functions for checking when the easing is starting / done etc.
        ///     See the base Behaviour class for more info on that.
        /// </summary>
        /// <param name="activeBehaviour"></param>
        /// <param name="easeWeight"></param>
        protected virtual void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
        }


        protected virtual void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
        }


        /// <summary>
        ///     This is called when the clip ends playing.
        /// </summary>
        /// <param name="activeBehaviour"></param>
        protected virtual void ClipEnd(Behaviour activeBehaviour)
        {
        }


        public sealed override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            TrackBinding ??= playerData; // Here we set the TrackBinding, if it's not set yet.

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var playableInput = (ScriptPlayable<Behaviour>) playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (behaviour is {ClipActive: true})
                {
                    var easeWeight = playable.GetInputWeight(i); // Ranges from 0 to 1
                    ClipActive(behaviour, easeWeight);
                }
            }
        }
    }
}