using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class Behaviour  : PlayableBehaviour
    {
        public Transform Example; // Data is stored here
        
        public override void OnGraphStart(Playable playable)
        {
            
        }
    }
}