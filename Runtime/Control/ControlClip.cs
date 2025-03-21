using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class ControlClip : Clip
    {
        private ControlBehaviour _template = new();
        private IControl _interfaceTrackBinding;
        private GameObject _gameObject;


        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (TrackBinding == null) // For when we forget to bind the track
            {
                return Playable.Null;
            }

            var playable = ScriptPlayable<ControlBehaviour>.Create(graph, _template); // Create a playable, using the constructor

            return playable;
        }


        public override void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);

            _gameObject = TrackBinding as GameObject;
            _interfaceTrackBinding = _gameObject?.GetComponent<IControl>();

            SetDisplayName();
        }


        private void SetDisplayName()
        {
            if (_interfaceTrackBinding == null)
            {
                TimelineClip.displayName = "No IPlayableControl found on " + (_gameObject?.name ?? "Unknown GameObject");

                return;
            }

            var typeName = _interfaceTrackBinding.GetType().Name;
            TimelineClip.displayName = "Bound to " + typeName + " on: " + _gameObject.name;
        }
    }
}