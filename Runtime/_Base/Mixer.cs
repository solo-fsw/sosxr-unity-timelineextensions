using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Base playable behaviour mixer for all SOSXR Timeline Extension tracks.
    ///     Subscribes to lifecycle events from each <see cref="Behaviour"/> input and routes them to the appropriate virtual
    ///     methods (<see cref="ClipStarted"/>, <see cref="ClipActive"/>, <see cref="ClipEnd"/>, etc.).
    ///     Extend this class and override those virtual methods to implement your track's per-frame logic.
    /// </summary>
    public abstract class Mixer : PlayableBehaviour
    {
        /// <summary>
        ///     Use this to get the object that the Track is bound to.
        ///     You usually want to cast it to the specific type of your binding.
        /// </summary>
        public object TrackBinding { get; set; }

        protected List<Behaviour> Behaviours = new List<Behaviour>();

        public bool IsLast(Behaviour current)
        {
            if (Behaviours[^1] == current)
            {
                return true;
            }

            return false;
        }

        public override void OnGraphStart(Playable playable)
        {
            int inputCount = playable.GetInputCount();

            Behaviours.Clear();

            for (int i = 0; i < inputCount; i++)
            {
                ScriptPlayable<Behaviour> playableInput =
                    (ScriptPlayable<Behaviour>)playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (behaviour == null)
                {
                    return;
                }

                behaviour.ClipStartedAction += ClipStarted;
                behaviour.ClipEaseInDoneOnceAction += ClipEaseInDoneOnce;
                behaviour.ClipEaseOutStartedOnceAction += ClipEaseOutStartedOnce;
                behaviour.ClipEndedAction += ClipEnd;

                Behaviours.Add(behaviour);
            }

            InitializeMixer(playable);
        }

        /// <summary>Called once after all input behaviours have been connected. Use this to cache the track binding and do one-time setup.</summary>
        /// <param name="playable">The mixer playable.</param>
        protected abstract void InitializeMixer(Playable playable);

        /// <summary>
        ///     This is called when the clip starts playing.
        /// </summary>
        /// <param name="activeBehaviour"></param>
        protected virtual void ClipStarted(Behaviour activeBehaviour) { }

        /// <summary>Called once when ease-in completes for the active clip.</summary>
        /// <param name="activeBehaviour">The behaviour whose ease-in just finished.</param>
        protected virtual void ClipEaseInDoneOnce(Behaviour activeBehaviour) { }

        /// <summary>
        ///     This is the main workhorse of the Mixer, where the active behaviour is processed.
        ///     It only gets called on any active Behaviour (when the playhead / time-scrubber is on the clip), so you don't need to check for that.
        ///     This is also the place where you can use the handy functions for checking when the easing is starting / done etc.
        ///     See the base Behaviour class for more info on that.
        /// </summary>
        /// <param name="activeBehaviour"></param>
        /// <param name="easeWeight"></param>
        protected virtual void ClipActive(Behaviour activeBehaviour, float easeWeight) { }

        /// <summary>Called once when ease-out begins for the active clip. This does _not_ get called when two clips overlap</summary>
        /// <param name="activeBehaviour">The behaviour whose ease-out just started.</param>
        protected virtual void ClipEaseOutStartedOnce(Behaviour activeBehaviour) { }

        /// <summary>
        ///     This is called when the clip ends playing. This happens when the end of the clip is overlapped by another clip, and also when the clip simply ends without overlap with another clip.
        /// </summary>
        /// <param name="activeBehaviour"></param>
        protected virtual void ClipEnd(Behaviour activeBehaviour) { }

        /// <summary>
        ///     Sealed ProcessFrame implementation. Iterates active input behaviours and dispatches to <see cref="ClipActive"/>.
        ///     Do not override — override <see cref="ClipActive"/> instead.
        /// </summary>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            TrackBinding ??= playerData; // Here we set the TrackBinding, if it's not set yet.

            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                ScriptPlayable<Behaviour> playableInput =
                    (ScriptPlayable<Behaviour>)playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (behaviour is { ClipIsActive: true })
                {
                    float easeWeight = playable.GetInputWeight(i);
                    behaviour.EaseWeight = easeWeight;
                    ClipActive(behaviour, easeWeight);
                }
            }
        }
    }
}
