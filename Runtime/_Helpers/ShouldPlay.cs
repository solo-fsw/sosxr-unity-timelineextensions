using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    public class ShouldPlay : MonoBehaviour
    {
        public PlayableDirector Task;
        public PlayableDirector DNF;
        public ExecutiveDirector Executive;


        [ContextMenu(nameof(DoTask))]
        public void DoTask()
        {
            Executive.SetShouldPlay(Task, true);
            Executive.SetShouldPlay(DNF, false);
        }


        [ContextMenu(nameof(DidNotFinish))]
        public void DidNotFinish()
        {
            Executive.SetShouldPlay(Task, false);
            Executive.SetShouldPlay(DNF, true);
        }
    }
}
