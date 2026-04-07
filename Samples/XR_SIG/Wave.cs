using SOSXR.TimelineExtensions;
using UnityEngine;

namespace SOSXR
{
    public class Wave : LooperControl
    {
        [SerializeField] private Transform m_head;
        [SerializeField] private Transform m_leftHand;
        [SerializeField] private Transform m_rightHand;


        private void Update()
        {
            if (m_leftHand.position.y > m_head.position.y || m_rightHand.position.y > m_head.position.y)
            {
                BreakAndGoToEnd();
            }
        }
    }
}
