using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Use this base class if you want to give another object the power to break out of the loop / pause in Timeline in
    ///     some fashion.
    /// </summary>
    public abstract class TimeControlBase : MonoBehaviour, ITimeControl
    {
        [SerializeField] [HideInInspector] private TimeControlBehaviour m_timeControl;

        [SerializeField] [HideInInspector] private PlayableDirector m_director;

        public TimeControlBehaviour TimeControl
        {
            get => m_timeControl;
            set => m_timeControl = value;
        }

        public PlayableDirector Director
        {
            get => m_director;
            set => m_director = value;
        }


        public virtual void SetTime(double time)
        {
            if (Director == null || Director.state != PlayState.Playing)
            {
                return;
            }

            Debug.Log("SetTime: " + time);
        }


        public virtual void OnControlTimeStart()
        {
            if (Director == null || Director.state != PlayState.Playing)
            {
                return;
            }

            Debug.Log("OnControlTimeStart");
        }


        public virtual void OnControlTimeStop()
        {
            if (Director == null || Director.state != PlayState.Playing)
            {
                return;
            }

            Debug.Log("OnControlTimeStop");
        }


        [ContextMenu(nameof(TimeScaleZero))]
        protected virtual void TimeScaleZero()
        {
            if (Director == null || Director.state != PlayState.Playing)
            {
                return;
            }

            TimeControl.CurrentState = TimeState.TimeScaleZero;
        }


        [ContextMenu(nameof(Looping))]
        protected virtual void Looping()
        {
            if (Director == null || Director.state != PlayState.Playing)
            {
                return;
            }

            TimeControl.CurrentState = TimeState.Looping;
        }


        [ContextMenu(nameof(Continue))]
        protected virtual void Continue()
        {
            if (Director == null || Director.state != PlayState.Playing)
            {
                return;
            }

            TimeControl.CurrentState = TimeState.Continue;
        }


        [ContextMenu(nameof(BreakAndGoToStart))]
        protected virtual void BreakAndGoToStart()
        {
            if (Director == null || Director.state != PlayState.Playing)
            {
                return;
            }

            TimeControl.CurrentState = TimeState.GoToStart;
        }


        [ContextMenu(nameof(BreakAndGoToEnd))]
        protected virtual void BreakAndGoToEnd()
        {
            TimeControl.CurrentState = TimeState.GoToEnd;
        }
    }
}