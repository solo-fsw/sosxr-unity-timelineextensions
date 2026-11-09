using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Reflection;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     Tests for edge cases and error handling (Week 2 - Important).
    /// </summary>
    public class EdgeCaseTests
    {
        [Test]
        public void Mixer_IsLast_WithNullBehaviourInList_HandlesGracefully()
        {
            var mixer = new TestMixer();
            var realBehaviour = new TestBehaviour();
            var behaviours = new List<Behaviour> { realBehaviour, null };
            
            var field = typeof(Mixer).GetField("Behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(mixer, behaviours);

            // Last item is null, so IsLast(null) returns true
            bool result = mixer.IsLast(null);

            Assert.IsTrue(result, "IsLast should return true when last behaviour in list is null");
        }

        [Test]
        public void Mixer_IsLast_SingleBehaviour_ReturnsTrue()
        {
            var mixer = new TestMixer();
            var behaviour = new TestBehaviour();
            var behaviours = new List<Behaviour> { behaviour };
            
            var field = typeof(Mixer).GetField("Behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(mixer, behaviours);

            bool result = mixer.IsLast(behaviour);

            Assert.IsTrue(result, "Single behaviour should be last");
        }

        [Test]
        public void Behaviour_EaseWeight_NegativeValue_SetSuccessfully()
        {
            var behaviour = new TestBehaviour();
            
            behaviour.EaseWeight = -0.5f;

            Assert.AreEqual(-0.5f, behaviour.EaseWeight, "Negative ease weight should be allowed");
        }

        [Test]
        public void Behaviour_EaseWeight_GreaterThanOne_SetSuccessfully()
        {
            var behaviour = new TestBehaviour();
            
            behaviour.EaseWeight = 1.5f;

            Assert.AreEqual(1.5f, behaviour.EaseWeight, "Ease weight > 1 should be allowed");
        }

        [Test]
        public void Behaviour_EaseWeight_Zero_SetSuccessfully()
        {
            var behaviour = new TestBehaviour();
            
            behaviour.EaseWeight = 0f;

            Assert.AreEqual(0f, behaviour.EaseWeight);
        }

        [Test]
        public void Behaviour_EaseWeight_One_SetSuccessfully()
        {
            var behaviour = new TestBehaviour();
            
            behaviour.EaseWeight = 1f;

            Assert.AreEqual(1f, behaviour.EaseWeight);
        }

        [Test]
        public void Mixer_WithEmptyBehavioursList_IsLastReturnsFalse()
        {
            var mixer = new TestMixer();
            var behaviours = new List<Behaviour>();
            
            var field = typeof(Mixer).GetField("Behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(mixer, behaviours);

            bool result = mixer.IsLast(new TestBehaviour());

            Assert.IsFalse(result, "Empty list should return false");
        }

        [Test]
        public void Behaviour_NotInMixerList_IsLastReturnsFalse()
        {
            var mixer = new TestMixer();
            var behaviourInList = new TestBehaviour();
            var behaviourNotInList = new TestBehaviour();
            var behaviours = new List<Behaviour> { behaviourInList };
            
            var field = typeof(Mixer).GetField("Behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(mixer, behaviours);

            bool result = mixer.IsLast(behaviourNotInList);

            Assert.IsFalse(result, "Behaviour not in list should return false");
        }

        public class TestMixer : Mixer
        {
            protected override void InitializeMixer(Playable playable) { }
        }

        public class TestBehaviour : Behaviour { }
    }
}
