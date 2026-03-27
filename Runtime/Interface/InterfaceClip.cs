using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the Interface track. Reflects the type of the bound <see cref="IInterface"/> component as the clip
    ///     display name in the Timeline window so it is easy to identify at a glance.
    /// </summary>
    [Serializable]
    public class InterfaceClip : Clip
    {
        private InterfaceBehaviour _template = new();
        private IInterface _interfaceTrackBinding;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (TrackBinding == null)
            {
                return Playable.Null;
            }

            ScriptPlayable<InterfaceBehaviour> playable = ScriptPlayable<InterfaceBehaviour>.Create(graph, _template);
            var behaviour = playable.GetBehaviour();
            behaviour.InitializeBehaviour(TimelineClip, TrackBinding);

            return playable;
        }

        public override void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);

            _interfaceTrackBinding = TrackBinding as IInterface;

            SetDisplayName();
        }

        private void SetDisplayName()
        {
            if (_interfaceTrackBinding == null)
            {
                TimelineClip.displayName = $"No {nameof(IInterface)} bound";

                return;
            }

            string typeName = _interfaceTrackBinding.GetType().Name;
            string location = _interfaceTrackBinding is Component c ? $"on: {c.gameObject.name}" : "asset";
            TimelineClip.displayName = $"{typeName} ({location})";
        }
    }
}
