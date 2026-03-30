using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     Tests for clip factory methods (Week 2 - Important).
    /// </summary>
    public class ClipFactoryTests
    {
        private PlayableGraph _graph;
        private GameObject _owner;

        [SetUp]
        public void Setup()
        {
            _graph = PlayableGraph.Create("TestGraph");
            _owner = new GameObject("TestOwner");
        }

        [TearDown]
        public void Teardown()
        {
            if (_graph.IsValid())
            {
                _graph.Destroy();
            }
            Object.DestroyImmediate(_owner);
        }

        [Test]
        public void ToTargetClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<ToTargetClip>();
            clip.Template = new ToTargetBehaviour();
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void RigidbodyClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<RigidbodyClip>();
            clip.Template = new RigidbodyBehaviour();
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void RotateToTargetClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<RotateToTargetClip>();
            clip.Template = new RotateToTargetBehaviour();
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void AnimatorClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<AnimatorClip>();
            clip.Template = new AnimatorBehaviour();
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void ParentingClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<ParentingClip>();
            clip.Template = new ParentingBehaviour();
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void LightsClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<LightsClip>();
            clip.Template = new LightsBehaviour();
            
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void InterfaceClip_CreatePlayable_WithGameObjectBinding_ReturnsNull()
        {
            var clip = ScriptableObject.CreateInstance<InterfaceClip>();
            var bindingObject = new GameObject("TestBinding");
            
            // Use reflection to set TrackBinding since it's protected
            var trackBindingField = typeof(Clip).GetField("TrackBinding", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            trackBindingField?.SetValue(clip, bindingObject);
            
            var playable = clip.CreatePlayable(_graph, _owner);

            // GameObject doesn't implement IInterface, so it returns Playable.Null
            Assert.IsFalse(playable.IsValid(), "Playable should be invalid when binding doesn't implement IInterface");

            Object.DestroyImmediate(clip);
            Object.DestroyImmediate(bindingObject);
        }

        [Test]
        public void InterfaceClip_CreatePlayable_WithoutBinding_ReturnsNull()
        {
            var clip = ScriptableObject.CreateInstance<InterfaceClip>();
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsFalse(playable.IsValid(), "Playable should be invalid when TrackBinding is null");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void LooperClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<LooperClip>();
            clip.Template = new LooperBehaviour();
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void EnhancedAudioClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<EnhancedAudioClip>();
            clip.Template = new EnhancedAudioBehaviour();
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void TMProClip_CreatePlayable_ReturnsValidPlayable()
        {
            var clip = ScriptableObject.CreateInstance<TMProClip>();
            // TMProClip doesn't use a Template - it creates playable directly
            var playable = clip.CreatePlayable(_graph, _owner);

            Assert.IsTrue(playable.IsValid(), "Playable should be valid");

            Object.DestroyImmediate(clip);
        }
    }
}
