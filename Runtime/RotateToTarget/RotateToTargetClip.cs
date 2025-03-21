using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class RotateToTargetClip : Clip
    {
        public ExposedReference<GameObject> TargetRef;

        public RotateToTargetBehaviour Template = new();


        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<RotateToTargetBehaviour>.Create(graph, Template); // Create a playable using the constructor

            Template.Target = TargetRef.Resolve(Resolver);
            Template = playable.GetBehaviour(); // Get behaviour, and set as template

            return playable;
        }
    }
}