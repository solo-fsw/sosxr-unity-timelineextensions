using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Looper track. Reads the current <see cref="TimeState"/> from the active <see cref="LooperBehaviour"/>
    ///     each frame and adjusts the Director's playhead or speed accordingly (loop, pause, jump to start/end, or continue).
    /// </summary>
    public class LooperMixer : Mixer
    {
        private double _previousSpeed = -1;
        /// <summary>The PlayableDirector whose playhead and speed this mixer controls. Resolved from the graph when not set.</summary>
        public PlayableDirector Director { get; set; }
        /// <summary>The LooperControl that releases the loop. Taken from the track binding when not set.</summary>
        public LooperControl LooperControl { get; set; }


        /// <summary>Resolves the Director and LooperControl, and logs a warning for each one that is missing.</summary>
        protected override void InitializeMixer(Playable playable)
        {
            LooperControl ??= TrackBinding as LooperControl;

            if (Director == null)
            {
                Director = playable.GetGraph().GetResolver() as PlayableDirector;
            }

            if (Director == null)
            {
                Debug.LogWarning($"{TypeName}: Director is null, did you forget to set it? The Looper track will do nothing.");
            }
            else if (!Director.playableGraph.IsValid())
            {
                Debug.LogWarning($"{TypeName}: Playable Graph is not valid, yet you're trying to access it! Is this on the last clip in the Timeline by any chance?");
            }

            if (LooperControl == null)
            {
                Debug.LogWarning($"{TypeName}: We have no LooperControl bound to the Looper Track, thus no way of getting out of the loop..!");
            }
        }


        /// <summary>Sets the LooperControl's ClipInTimeline to the started clip, if a LooperControl is bound.</summary>
        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not LooperBehaviour behaviour)
            {
                return;
            }

            if (LooperControl == null)
            {
                return;
            }

            LooperControl.ClipInTimeline = behaviour;
        }


        /// <summary>
        ///     Applies the clip's time state: jumps the Director to the clip start or end when requested, then sets the timeline
        ///     speed to 0 for TimeScaleZero and to 1 otherwise.
        /// </summary>
        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not LooperBehaviour behaviour || Director == null)
            {
                return;
            }

            if (behaviour.CurrentState == TimeState.BreakAndGoToStart)
            {
                Director.time = behaviour.TimelineClip.start;
                behaviour.CurrentState = TimeState.BreakAndContinue;
            }
            else if (behaviour.CurrentState == TimeState.BreakAndGoToEnd)
            {
                Director.time = behaviour.TimelineClip.end;
                behaviour.CurrentState = TimeState.BreakAndContinue;
            }

            var timelineSpeed = behaviour.CurrentState == TimeState.TimeScaleZero ? 0 : 1;
            SetTimelineSpeed(timelineSpeed);
        }


        /// <summary>
        ///     Handles a stopped clip: unfreezes a paused timeline, restarts the Director when the clip must loop, and clears
        ///     the LooperControl's reference when the clip finished.
        /// </summary>
        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not LooperBehaviour behaviour)
            {
                return;
            }

            // A TimeScaleZero clip froze the timeline. The playhead left the clip (e.g. an external seek): unfreeze,
            // or the timeline stays at speed 0. When the clip restarts below, ClipActive sets the speed for its state again.
            if (_previousSpeed == 0)
            {
                SetTimelineSpeed(1);
            }

            if (ShouldRestartClip(behaviour.CurrentState, behaviour.ReachedEnd) && Director != null) // We need to do this clip again
            {
                Director.time = behaviour.TimelineClip.start;

                // If the Timeline reached its end and Unity stopped playback, restart the Director so looping wins.
                if (Director.state != PlayState.Playing)
                {
                    Director.Play();
                }

                behaviour.ClipIsDone = false;

                return;
            }

            if (!behaviour.ReachedEnd)
            {
                // Stopped or scrubbed away (a pause does not end the clip): keep the LooperControl pointed at this clip, so it can be resumed.
                return;
            }

            // We are done with this clip
            behaviour.ClipIsDone = true;

            if (LooperControl != null && LooperControl.ClipInTimeline == behaviour)
            {
                LooperControl.ClipInTimeline = null;
            }
        }


        /// <summary>
        ///     True when a clip that just stopped must jump back to its start.
        ///     Only a clip that played to its end loops. A pause, stop or seek away never restarts the Director.
        /// </summary>
        public static bool ShouldRestartClip(TimeState state, bool reachedEnd)
        {
            return reachedEnd && (state is TimeState.Looping or TimeState.BreakAndGoToStart);
        }


        /// <summary>
        ///     Sets the root playable speed on the Director's graph. Caches the previous speed and skips the call if unchanged.
        /// </summary>
        /// <param name="speed">0 to pause, 1 for normal playback.</param>
        public void SetTimelineSpeed(double speed)
        {
            if (Math.Abs(_previousSpeed - speed) < 0.001)
            {
                return;
            }

            if (Director == null || !Director.playableGraph.IsValid() || Director.playableGraph.GetRootPlayableCount() == 0)
            {
                return;
            }

            var root = Director.playableGraph.GetRootPlayable(0);

            if (!root.IsValid())
            {
                return;
            }

            root.SetSpeed(speed);

            _previousSpeed = speed;
        }
    }
}
