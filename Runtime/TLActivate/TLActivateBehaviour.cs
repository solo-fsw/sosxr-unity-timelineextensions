using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class TLActivateBehaviour : PlayableBehaviour
    {
        [FormerlySerializedAs("activateReference")] public ExposedReference<GameObject> ActivateReference;

        [FormerlySerializedAs("activateAtStart")] public bool ActivateAtStart = true;
        [FormerlySerializedAs("activateAtEnd")] public bool ActivateAtEnd;
        private bool _activatedAtEnd;
        private bool _activatedAtStart;

        private bool _clipHasPlayed;
        public ITLActivate Activate;


        public override void OnPlayableCreate(Playable playable)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;

            if (director == null)
            {
                return;
            }

            if (ActivateReference.Resolve(director) == null)
            {
                return;
            }

            if (ActivateReference.Resolve(director).GetComponent<ITLActivate>() == null)
            {
                Debug.LogWarningFormat("Selected GameObject {0} does not have an ITLActivate component, cannot proceed",
                    ActivateReference.Resolve(director).name);

                return;
            }

            Activate = ActivateReference.Resolve(director).GetComponent<ITLActivate>();
        }


        public override void OnGraphStart(Playable playable)
        {
            _activatedAtStart = false;
            _activatedAtEnd = false;
        }


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _clipHasPlayed = true;

            if (ActivateAtStart == false)
            {
                return;
            }

            if (_activatedAtStart == false)
            {
                Activate?.TLActivate();
                _activatedAtStart = true;
            }
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_clipHasPlayed == false)
            {
                return;
            }

            if (ActivateAtEnd == false)
            {
                return;
            }

            if (_activatedAtEnd == false)
            {
                Activate?.TLActivate();
                _activatedAtEnd = true;
            }
        }
    }
}