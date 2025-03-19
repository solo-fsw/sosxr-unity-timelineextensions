using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     These variables allow us to set the value in the editor.
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    public class LightsClip : PlayableAsset
    {
        public float Intensity;
        public Color Color;
        public float Range;

        public LightsBehaviour Template;

        public TimelineClip TimelineClip { get; set; }


        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LightsBehaviour>.Create(graph); // Create a playable using the constructor

            var behaviour = playable.GetBehaviour(); // Get behaviour

            SetDisplayName();

            return playable;
        }


        /// <summary>
        ///     The displayname of the clip in Timeline will be set using this method.
        ///     Name is only set if a light is set to != 0;
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        private void SetDisplayName()
        {
            var displayName = "";

            if (Intensity != 0)
            {
                displayName += "I:" + Intensity + " R:" + Range + " (" + Color.r + "," + Color.g + "," + Color.b + ")";
            }

            displayName = SetDisplayNameIfStillEmpty(displayName);

            if (TimelineClip != null)
            {
                TimelineClip.displayName = displayName;
            }
        }


        private static string SetDisplayNameIfStillEmpty(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = "New Lights Clip";
            }

            return displayName;
        }
    }
}