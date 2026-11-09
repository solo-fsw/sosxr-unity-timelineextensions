using NUnit.Framework;
using UnityEngine;
using System;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     Tests for Behaviour lifecycle events (Week 1 - Critical Path).
    /// </summary>
    public class BehaviourLifecycleTests
    {
        private TestBehaviour _behaviour;

        [SetUp]
        public void Setup()
        {
            _behaviour = new TestBehaviour();
        }

        [TearDown]
        public void Teardown()
        {
            _behaviour = null;
        }

        [Test]
        public void Behaviour_ClipStartedAction_FiresOnce()
        {
            bool fired = false;
            _behaviour.ClipStartedAction += (b) => fired = true;

            _behaviour.TriggerClipStarted();

            Assert.IsTrue(fired, "ClipStartedAction should fire");
        }

        [Test]
        public void Behaviour_ClipEaseInDoneOnceAction_FiresOnce()
        {
            bool fired = false;
            _behaviour.ClipEaseInDoneOnceAction += (b) => fired = true;

            _behaviour.TriggerClipEaseInDoneOnce();

            Assert.IsTrue(fired, "ClipEaseInDoneOnceAction should fire");
        }

        [Test]
        public void Behaviour_ClipEaseOutStartedOnceAction_FiresOnce()
        {
            bool fired = false;
            _behaviour.ClipEaseOutStartedOnceAction += (b) => fired = true;

            _behaviour.TriggerClipEaseOutStartedOnce();

            Assert.IsTrue(fired, "ClipEaseOutStartedOnceAction should fire");
        }

        [Test]
        public void Behaviour_ClipEndedAction_FiresOnce()
        {
            bool fired = false;
            _behaviour.ClipEndedAction += (b) => fired = true;

            _behaviour.TriggerClipEnded();

            Assert.IsTrue(fired, "ClipEndedAction should fire");
        }

        [Test]
        public void Behaviour_AllActions_CanFireIndependently()
        {
            int firedCount = 0;
            _behaviour.ClipStartedAction += (b) => firedCount++;
            _behaviour.ClipEaseInDoneOnceAction += (b) => firedCount++;
            _behaviour.ClipEaseOutStartedOnceAction += (b) => firedCount++;
            _behaviour.ClipEndedAction += (b) => firedCount++;

            _behaviour.TriggerClipStarted();
            _behaviour.TriggerClipEaseInDoneOnce();
            _behaviour.TriggerClipEaseOutStartedOnce();
            _behaviour.TriggerClipEnded();

            Assert.AreEqual(4, firedCount, "All 4 actions should fire independently");
        }

        [Test]
        public void Behaviour_EaseWeight_CanBeReadAndWritten()
        {
            _behaviour.EaseWeight = 0.5f;

            Assert.AreEqual(0.5f, _behaviour.EaseWeight);
        }

        [Test]
        public void Behaviour_ClipIsActive_CanBeToggled()
        {
            Assert.IsFalse(_behaviour.ClipIsActive, "Should default to false");

            _behaviour.SetClipIsActive(true);

            Assert.IsTrue(_behaviour.ClipIsActive);
        }

        [Test]
        public void Behaviour_MultipleSubscribers_AllReceiveEvents()
        {
            int count1 = 0;
            int count2 = 0;
            _behaviour.ClipStartedAction += (b) => count1++;
            _behaviour.ClipStartedAction += (b) => count2++;

            _behaviour.TriggerClipStarted();

            Assert.AreEqual(1, count1, "First subscriber should receive event");
            Assert.AreEqual(1, count2, "Second subscriber should receive event");
        }

        /// <summary>
        ///     Test behaviour with public methods to trigger events for testing.
        /// </summary>
        public class TestBehaviour : Behaviour
        {
            public void TriggerClipStarted() => ClipStartedAction?.Invoke(this);
            public void TriggerClipEaseInDoneOnce() => ClipEaseInDoneOnceAction?.Invoke(this);
            public void TriggerClipEaseOutStartedOnce() => ClipEaseOutStartedOnceAction?.Invoke(this);
            public void TriggerClipEnded() => ClipEndedAction?.Invoke(this);
            public void SetClipIsActive(bool active) => ClipIsActive = active;
        }
    }
}
