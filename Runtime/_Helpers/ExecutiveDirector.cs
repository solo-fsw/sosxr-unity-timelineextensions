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
    public class ExecutiveDirector : MonoBehaviour
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

        [SerializeField] private List<DurationDirector> m_durationDirectors;
        [DisableEditing][SerializeField] private float m_totalDuration;

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
        }

        private void Awake()
        {
            if (m_autoPlay == AutoPlay.OnAwake)
            {
                PlayAllDirectors();
            }
        }

        private void Start()
        {
            if (m_autoPlay == AutoPlay.OnStart)
            {
                PlayAllDirectors();
            }
        }

        private void OnEnable()
        {
            if (m_autoPlay == AutoPlay.OnEnable)
            {
                PlayAllDirectors();
            }
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
                dd.Director.Play();
                dd.IsPlaying = true;

                Debug.Log("Playing director: " + dd.Director.name + " for " + dd.Duration + " seconds");

                yield return new WaitForSeconds(dd.Duration);
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
            [DisableEditing] public bool IsPlaying;
            [DisableEditing] public float Duration;
        }
    }
}
