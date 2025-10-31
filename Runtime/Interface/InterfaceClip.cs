using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class InterfaceClip : Clip
    {
        private InterfaceBehaviour _template = new();
        private IInterface _interfaceTrackBinding;
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

            var playable = ScriptPlayable<InterfaceBehaviour>.Create(graph, _template); // Create a playable, using the constructor
            var behaviour = playable.GetBehaviour(); // Get the behaviour from the playable
            behaviour.InitializeBehaviour(TimelineClip, TrackBinding); // Initialize the behaviour

            return playable;
        }


        public override void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);

            _gameObject = TrackBinding as GameObject;
            _interfaceTrackBinding = _gameObject?.GetComponent<IInterface>();

            SetDisplayName();
        }


        private void SetDisplayName()
        {
            if (_interfaceTrackBinding == null)
            {
                TimelineClip.displayName = "No IControl found on " + (_gameObject?.name ?? "Unknown GameObject");

                return;
            }

            var typeName = _interfaceTrackBinding.GetType().Name;
            TimelineClip.displayName = "Bound to " + typeName + " on: " + _gameObject.name;
        }
    }
}