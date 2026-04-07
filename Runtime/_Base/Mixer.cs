using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

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

        /// <summary>
        ///     Returns true if the supplied behaviour is the last one in the mixer's input list.
        /// </summary>
        /// <param name="current">Behaviour to test.</param>
        /// <returns>True if current is the last behaviour; otherwise false.</returns>
        public bool IsLast(Behaviour current)
        {
            if (Behaviours.Count == 0)
            {
                return false;
            }

            if (Behaviours[Behaviours.Count - 1] == current)
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
                ScriptPlayable<Behaviour> playableInput = (ScriptPlayable<Behaviour>)playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (behaviour == null)
                {
                    continue;
                }

                behaviour.ClipStartedAction += ClipStarted;
                behaviour.ClipEaseInDoneOnceAction += ClipEaseInDoneOnce;
                behaviour.ClipEaseOutStartedOnceAction += ClipEaseOutStartedOnce;
                behaviour.ClipEndedAction += ClipEnd;

                Behaviours.Add(behaviour);
            }

            foreach (var behaviour in Behaviours)
            {
                double clipStart = behaviour.TimelineClip.start;
                double clipEnd = clipStart + behaviour.TimelineClip.duration;

                behaviour.AnotherClipOverlapsWithMe = Behaviours.Any(other =>
                    other != behaviour &&
                    other.TimelineClip != null &&
                    other.TimelineClip.start > clipStart &&
                    other.TimelineClip.start < clipEnd);
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

        /// <summary>
        /// Called once when ease-out begins for a clip (when it reaches ease-out time or ends).
        /// Note: The Behaviour fires this event for all clips when they reach ease-out or end.
        /// Individual mixers should filter based on which clip is currently driving output
        /// (typically by checking if activeBehaviour == _activeBehaviour).
        /// </summary>
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

            // Set TrackBinding if not already set (compatible with older C# versions)
            if (TrackBinding == null)
            {
                TrackBinding = playerData;
            }

            int inputCount = playable.GetInputCount();

            // Use cached Behaviours list instead of calling GetInput each frame for better performance
            for (int i = 0; i < Behaviours.Count && i < inputCount; i++)
            {
                var behaviour = Behaviours[i];

                if (behaviour != null && behaviour.ClipIsActive)
                {
                    float easeWeight = playable.GetInputWeight(i);
                    behaviour.EaseWeight = easeWeight;
                    ClipActive(behaviour, easeWeight);
                }
            }
        }
    }
}
