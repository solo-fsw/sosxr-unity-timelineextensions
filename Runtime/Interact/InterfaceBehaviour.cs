using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Acts as our data for the clip to write to
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [Serializable]
    public class InterfaceBehaviour : PlayableBehaviour
    {
        public ExposedReference<GameObject> InterfaceObjectReference;
        public GameObject InterfaceObject;
        public ITimeControl Interface;

        private PlayableDirector _director;


        public override void OnPlayableCreate(Playable playable)
        {
            _director = playable.GetGraph().GetResolver() as PlayableDirector;

            if (_director == null)
            {
                Debug.LogWarning("No director found");

                return;
            }

            InterfaceObject = InterfaceObjectReference.Resolve(_director);

            Interface = InterfaceObject?.GetComponent<ITimeControl>();

            if (Interface != null)
            {
                return;
            }

            if (InterfaceObject != null && Interface == null)
            {
                Debug.LogWarning("No ITimeControl found on " + InterfaceObject?.name);
            }
        }


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Interface?.OnControlTimeStart();
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Interface?.OnControlTimeStop();
        }
    }
}