using UnityEngine;
using SOSXR.TimelineExtensions;

namespace SOSXR
{
    public class InformedConsentManager : LooperControl
    {
        public void Agree()
        {
            BreakAndContinue();
        }


        public void Disagree()
        {
            BreakAndGoToEnd();
        }
    }
}
