using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     Integration tests for track workflows (Week 3+ - Nice to Have).
    /// </summary>
    public class IntegrationTests
    {
        private GameObject _trackBinding;
        private PlayableGraph _graph;

        [SetUp]
        public void Setup()
        {
            _trackBinding = new GameObject("TrackBinding");
            _graph = PlayableGraph.Create("TestGraph");
        }

        [TearDown]
        public void Teardown()
        {
            if (_graph.IsValid())
            {
                _graph.Destroy();
            }
            Object.DestroyImmediate(_trackBinding);
        }

        [Test]
        public void Integration_BehaviourMixer_EventFlow()
        {
            var behaviour = new TestBehaviour();
            bool started = false;
            bool ended = false;

            behaviour.ClipStartedAction += (b) => started = true;
            behaviour.ClipEndedAction += (b) => ended = true;

            behaviour.TriggerClipStarted();
            behaviour.SetClipIsActive(true);
            behaviour.SetClipIsActive(false);
            behaviour.TriggerClipEnded();

            Assert.IsTrue(started, "Started should fire");
            Assert.IsTrue(ended, "Ended should fire");
        }

        [Test]
        public void Integration_MixerLifecycle_ProperSetup()
        {
            var mixer = new TestMixer();
            var behaviour = new TestBehaviour();
            var binding = new GameObject("Binding");

            mixer.TrackBinding = binding;
            behaviour.SetClipIsActive(true);

            Assert.IsNotNull(mixer.TrackBinding);
            Assert.IsTrue(behaviour.ClipIsActive);

            Object.DestroyImmediate(binding);
        }

        [Test]
        public void Integration_EaseWeightPropagation()
        {
            var behaviour = new TestBehaviour();
            
            behaviour.SetClipIsActive(true);
            behaviour.EaseWeight = 0.75f;

            Assert.AreEqual(0.75f, behaviour.EaseWeight);
        }

        [Test]
        public void Integration_MultipleBehaviours_IndependentEvents()
        {
            var behaviour1 = new TestBehaviour();
            var behaviour2 = new TestBehaviour();
            int eventCount1 = 0;
            int eventCount2 = 0;

            behaviour1.ClipStartedAction += (b) => eventCount1++;
            behaviour2.ClipStartedAction += (b) => eventCount2++;

            behaviour1.TriggerClipStarted();

            Assert.AreEqual(1, eventCount1, "Behaviour 1 should fire");
            Assert.AreEqual(0, eventCount2, "Behaviour 2 should not fire");
        }

        [Test]
        public void Integration_CompleteLifecycle_AllEventsFire()
        {
            var behaviour = new TestBehaviour();
            var events = new List<string>();

            behaviour.ClipStartedAction += (b) => events.Add("Started");
            behaviour.ClipEaseInDoneOnceAction += (b) => events.Add("EaseInDone");
            behaviour.ClipEaseOutStartedOnceAction += (b) => events.Add("EaseOutStarted");
            behaviour.ClipEndedAction += (b) => events.Add("Ended");

            behaviour.TriggerClipStarted();
            behaviour.TriggerClipEaseInDoneOnce();
            behaviour.TriggerClipEaseOutStartedOnce();
            behaviour.TriggerClipEnded();

            Assert.AreEqual(4, events.Count);
            Assert.Contains("Started", events);
            Assert.Contains("EaseInDone", events);
            Assert.Contains("EaseOutStarted", events);
            Assert.Contains("Ended", events);
        }

        private class TestMixer : Mixer
        {
            protected override void InitializeMixer(Playable playable) { }
        }

        private class TestBehaviour : Behaviour
        {
            public void TriggerClipStarted() => ClipStartedAction?.Invoke(this);
            public void TriggerClipEaseInDoneOnce() => ClipEaseInDoneOnceAction?.Invoke(this);
            public void TriggerClipEaseOutStartedOnce() => ClipEaseOutStartedOnceAction?.Invoke(this);
            public void TriggerClipEnded() => ClipEndedAction?.Invoke(this);
            public void SetClipIsActive(bool active) => ClipIsActive = active;
        }
    }
}
