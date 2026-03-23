using System;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the Looper track. Holds the desired <see cref="TimeState"/> configured in the Inspector
    ///     (<see cref="InitialState"/>) and the runtime-mutable <see cref="CurrentState"/> that the mixer reads each frame.
    /// </summary>
    [Serializable]
    public class LooperBehaviour : Behaviour
    {
        /// <summary>The playback state chosen in the Inspector. Used to reset CurrentState when replaying.</summary>
        public TimeState InitialState; // This is what you set in the inspector for what this clip initially needs to do
        /// <summary>The actively read state. Can be changed at runtime by <see cref="LooperControl"/> to override looping behaviour.</summary>
        public TimeState CurrentState; // This allows us to revert back to choice made in inspector: otherwise this ScriptableObject will store the changes made in PlayMode


        /// <summary>
        ///     The displayName of the clip in Timeline will be set using this method.
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        public void SetDisplayName()
        {
            var displayName = "";

            if (CurrentState == TimeState.TimeScaleZero)
            {
                displayName = "|| stop time";
            }
            else if (CurrentState == TimeState.BreakAndContinue)
            {
                displayName = "● do not loop";
            }
            else if (CurrentState == TimeState.Looping)
            {
                displayName = "↩︎ loop clip";
            }
            else if (CurrentState == TimeState.BreakAndGoToStart)
            {
                displayName = "← break from start";
            }
            else if (CurrentState == TimeState.BreakAndGoToEnd)
            {
                displayName = "→ break from end";
            }

            displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Looper Clip");

            if (TimelineClip == null)
            {
                return;
            }

            TimelineClip.displayName = displayName;
        }
    }
}
