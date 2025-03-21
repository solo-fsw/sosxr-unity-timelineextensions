using System;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class TimeControlBehaviour : Behaviour
    {
        public TimeState InitialState; // This is what you set in the inspector for what this clip initially needs to do
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
                displayName = "|| pausing";
            }
            else if (CurrentState == TimeState.Continue)
            {
                displayName = "● do not loop";
            }
            else if (CurrentState == TimeState.Looping)
            {
                displayName = "↩︎ loop clip";
            }
            else if (CurrentState == TimeState.GoToStart)
            {
                displayName = "← go to clip start";
            }
            else if (CurrentState == TimeState.GoToEnd)
            {
                displayName = "→ go to clip end";
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