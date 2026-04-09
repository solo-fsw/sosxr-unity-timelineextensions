using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Plays a list of <see cref="PlayableDirector"/> components sequentially, waiting for each one's duration before
    ///     starting the next. Useful for chaining multiple Timelines end-to-end without manual coordination.
    ///     Supports auto-play on Awake, Start, or OnEnable, and can be re-triggered at runtime via <see cref="PlayAllDirectors"/>.
    /// </summary>
    public class ExecutiveDirector : LooperControl
    {
        /// <summary>Controls when the director sequence starts automatically.</summary>
        public enum AutoPlay
        {
            Never,
            OnAwake,
            OnStart,
            OnEnable
        }

        [SerializeField] private AutoPlay m_autoPlay = AutoPlay.Never;

        [SerializeField] private List<DurationDirector> m_durationDirectors = new List<DurationDirector>();
        [SerializeField, DisableEditing] private float m_totalDuration;


        private Coroutine _playCoroutine;

        private void OnValidate()
        {
            m_totalDuration = 0;

            foreach (var dd in m_durationDirectors)
            {
                if (dd == null || dd.Director == null)
                {
                    dd.Duration = 0;
                    dd.IsPlaying = false;

                    continue;
                }

                dd.Director.playOnAwake = false;
                dd.Duration = (float)Math.Round(dd.Director.duration, 2);
                m_totalDuration += dd.Duration;
            }

            if (AllowBuffering)
            {
                AllowBuffering = false;
                Debug.Log($"Buffering is not allowed on the {nameof(ExecutiveDirector)}, because otherwise this can only be used once as a LooperControl");
            }
        }

        private void Awake()
        {
            if (this.enabled == false)
            {
                return;
            }
            if (m_autoPlay == AutoPlay.OnAwake)
            {
                PlayAllDirectors();
            }
        }

        private void Start()
        {
            if (this.enabled == false)
            {
                return;
            }
            if (m_autoPlay == AutoPlay.OnStart)
            {
                PlayAllDirectors();
            }
        }

        private void OnEnable()
        {
            if (this.enabled == false)
            {
                return;
            }
            if (m_autoPlay == AutoPlay.OnEnable)
            {
                PlayAllDirectors();
            }
        }


        public void SetShouldPlay(PlayableDirector director, bool should)
        {
            foreach (var dd in m_durationDirectors)
            {
                if (dd.Director != director)
                {
                    continue;
                }

                dd.ShouldPlay = should;

                break;
            }
        }


        public void ShouldPlay(int index)
        {
            SetShouldPlay(m_durationDirectors[index].Director, true);
        }


        public void ShouldNotPlay(int index)
        {
            SetShouldPlay(m_durationDirectors[index].Director, false);
        }


        [ContextMenu(nameof(PlayAllDirectors))]
        /// <summary>
        ///     Starts the sequential playback coroutine. If already running, stops all directors and restarts from the beginning.
        /// </summary>
        public void PlayAllDirectors()
        {
            if (_playCoroutine != null)
            {
                foreach (var dd in m_durationDirectors)
                {
                    dd.Director.Stop();
                    dd.IsPlaying = false;
                }

                StopCoroutine(_playCoroutine);
                _playCoroutine = null;

                Debug.Log("Stopping all directors");
            }

            _playCoroutine = StartCoroutine(PlayAllDirectorsCR());
        }

        private IEnumerator PlayAllDirectorsCR()
        {
            foreach (var dd in m_durationDirectors)
            {
                if (dd.ShouldPlay == false)
                {
                    Debug.Log($"We will not play {dd.Director.name}");
                    continue;
                }

                dd.Director.Play();
                dd.IsPlaying = true;

                Debug.Log("Playing director: " + dd.Director.name + " for approximately " + dd.Duration + " seconds (may be longer or shorter due to Loopers and other control mechanisms).");

                while (dd.Director.state == PlayState.Playing)
                {
                    yield return new WaitForSeconds(0.25f);
                }

                dd.IsPlaying = false;
            }

            Debug.Log("Playing directors finished");

            _playCoroutine = null;
        }

        private void OnDisable() => StopAllCoroutines();

        /// <summary>Pairs a <see cref="PlayableDirector"/> with its runtime duration and playing state for sequential playback.</summary>
        [Serializable]
        public class DurationDirector
        {
            public PlayableDirector Director;

            [DisableEditing] public bool IsPlaying = false;
            [DisableEditing] public bool ShouldPlay = true;

            [Tooltip("This is the duration of the Timeline. Beware that when using Loopers or other control measures, the Runtime duration will be different than listed here.")]
            [DisableEditing] public float Duration;
        }
    }
}
