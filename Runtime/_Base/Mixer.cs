using System;
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

        /// <summary>
        ///     Delta time (in seconds) of the frame currently being processed, as reported by Timeline.
        ///     Valid inside <see cref="ClipActive"/>.
        /// </summary>
        protected float FrameDeltaTime { get; private set; }

        // Cache the type name to avoid reflection overhead in error messages
        private string _cachedTypeName;

        /// <summary>
        ///     Returns the cached type name for this mixer to use in debug messages.
        /// </summary>
        protected string TypeName => _cachedTypeName ??= GetType().Name;

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
            var inputCount = playable.GetInputCount();
            var inputs = new List<Behaviour>(inputCount);

            for (var i = 0; i < inputCount; i++)
            {
                var input = playable.GetInput(i);

                if (!input.IsValid() || !typeof(Behaviour).IsAssignableFrom(input.GetPlayableType()))
                {
                    inputs.Add(null);

                    continue;
                }

                inputs.Add(((ScriptPlayable<Behaviour>)input).GetBehaviour());
            }

            RegisterBehaviours(inputs);

            InitializeMixer(playable);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            UnsubscribeAll();
            Behaviours.Clear();
        }

        /// <summary>
        ///     Replaces the current set of input behaviours. Unsubscribes from the previous set first, so calling this
        ///     repeatedly (OnGraphStart runs again after every Pause / Play) never stacks event handlers.
        ///     Null entries are kept so list indices keep matching the mixer's input ports.
        /// </summary>
        internal void RegisterBehaviours(IList<Behaviour> inputs)
        {
            UnsubscribeAll();
            Behaviours.Clear();

            foreach (var behaviour in inputs)
            {
                Behaviours.Add(behaviour);

                if (behaviour == null)
                {
                    continue;
                }

                behaviour.ClipStartedAction += ClipStarted;
                behaviour.ClipEaseInDoneOnceAction += ClipEaseInDoneOnce;
                behaviour.ClipEaseOutStartedOnceAction += ClipEaseOutStartedOnce;
                behaviour.ClipEndedAction += ClipEnd;
            }

            UpdateOverlapFlags();
        }

        private void UnsubscribeAll()
        {
            foreach (var behaviour in Behaviours)
            {
                if (behaviour == null)
                {
                    continue;
                }

                behaviour.ClipStartedAction -= ClipStarted;
                behaviour.ClipEaseInDoneOnceAction -= ClipEaseInDoneOnce;
                behaviour.ClipEaseOutStartedOnceAction -= ClipEaseOutStartedOnce;
                behaviour.ClipEndedAction -= ClipEnd;
            }
        }

        private void UpdateOverlapFlags()
        {
            foreach (var behaviour in Behaviours)
            {
                if (behaviour?.TimelineClip == null)
                {
                    continue;
                }

                var clipStart = behaviour.TimelineClip.start;
                var clipEnd = clipStart + behaviour.TimelineClip.duration;

                var hasOverlap = false;

                foreach (var other in Behaviours)
                {
                    if (other != null && other != behaviour && other.TimelineClip != null && other.TimelineClip.start > clipStart && other.TimelineClip.start < clipEnd)
                    {
                        hasOverlap = true;

                        break;
                    }
                }

                behaviour.AnotherClipOverlapsWithMe = hasOverlap;
            }
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
        ///     This is called when the clip stops playing: the playhead left the clip, another clip took over, or the
        ///     Director was paused / stopped. Check <see cref="Behaviour.ReachedEnd"/> to tell a natural end apart from an interruption.
        /// </summary>
        /// <param name="activeBehaviour"></param>
        protected virtual void ClipEnd(Behaviour activeBehaviour) { }

        /// <summary>
        ///     ProcessFrame implementation. Iterates active input behaviours and dispatches to <see cref="ClipActive"/>.
        ///     Prefer overriding <see cref="ClipActive"/>. If you do override this, call the base method.
        /// </summary>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            DispatchFrame(playerData, playable.GetInputCount(), i => playable.GetInputWeight(i), (float)info.deltaTime);
        }

        /// <summary>
        ///     Engine-independent part of <see cref="ProcessFrame"/>: sets the binding, stores the frame delta and calls
        ///     <see cref="ClipActive"/> for every active behaviour with its input weight.
        /// </summary>
        internal void DispatchFrame(object playerData, int inputCount, Func<int, float> getInputWeight, float deltaTime)
        {
            TrackBinding ??= playerData;

            FrameDeltaTime = deltaTime;

            // Use cached Behaviours list instead of calling GetInput each frame for better performance
            for (var i = 0; i < Behaviours.Count && i < inputCount; i++)
            {
                var behaviour = Behaviours[i];

                if (behaviour != null && behaviour.ClipIsActive)
                {
                    var easeWeight = getInputWeight(i);
                    behaviour.EaseWeight = easeWeight;
                    ClipActive(behaviour, easeWeight);
                }
            }
        }
    }
}
