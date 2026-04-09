using SOSXR.TimelineExtensions;
using UnityEngine;

namespace SOSXR
{
    public class Wave : LooperControl
    {
        [SerializeField] private Transform m_head;
        [SerializeField] private Transform m_leftHand;
        [SerializeField] private Transform m_rightHand;
        [SerializeField] private TimeState m_timeState = TimeState.BreakAndGoToEnd;

        private Animation _animation;


        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }


        [ContextMenu(nameof(AnimateWave))]
        private void AnimateWave()
        {
            if (_animation == null)
            {
                return;
            }

            _animation.Play();
        }

        

        private void Update()
        {
            if (m_leftHand.position.y > m_head.position.y || m_rightHand.position.y > m_head.position.y)
            {
                if (m_timeState == TimeState.BreakAndGoToEnd)
                {
                    BreakAndGoToEnd();
                }
                else if (m_timeState == TimeState.BreakAndContinue)
                {
                    BreakAndContinue();
                }
                else if (m_timeState == TimeState.BreakAndGoToStart)
                {
                    BreakAndGoToStart();
                }
            }
        }
    }
}
