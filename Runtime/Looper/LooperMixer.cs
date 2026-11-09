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
        public PlayableDirector Director { get; set; }
        public LooperControl LooperControl { get; set; }


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


        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not LooperBehaviour behaviour)
            {
                return;
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
                // Paused, stopped or scrubbed away: keep the LooperControl pointed at this clip, so it can be resumed.
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

            Director.playableGraph.GetRootPlayable(0).SetSpeed(speed);

            _previousSpeed = speed;
        }
    }
}
