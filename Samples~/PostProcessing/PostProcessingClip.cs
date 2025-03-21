using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     These variables allow us to set the value in the editor.
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    public class PostProcessingClip : Clip
    {
        public ExposedReference<Volume> Volume;
        [Range(0f, 1f)] public float MaxWeight;
        public PostProcessingBehaviour Template;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<PostProcessingBehaviour>.Create(graph, Template);

            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            clone.Volume = Volume.Resolve(Resolver);
            clone.MaxWeight = MaxWeight;

            return playable;
        }
    }
}