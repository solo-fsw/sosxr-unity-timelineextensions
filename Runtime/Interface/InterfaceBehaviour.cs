using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class InterfaceBehaviour : PlayableBehaviour
    {
        public ExposedReference<GameObject> InterfaceObjectReference;
        public GameObject InterfaceObject;
        public ITimeControl Interface;
        private PlayableDirector _director;

        private bool _hasStarted = false;


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
            if (!Application.isPlaying)
            {
                return;
            }

            Interface?.OnControlTimeStart();
            _hasStarted = true;
        }


        /// <summary>
        ///     OnBehaviourPause has the tendency to be called all the time: on pause, prior to starting, when scrubbing, etc.
        ///     Some checks are in place to ensure that the interface is only called when the clip has actually started.
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!_hasStarted)
            {
                return;
            }

            Interface?.OnControlTimeStop();
        }
    }
}