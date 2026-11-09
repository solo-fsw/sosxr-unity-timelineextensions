using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Reflection;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     Basic tests for the core 4-class architecture (Track, Clip, Behaviour, Mixer).
    /// </summary>
    public class CoreArchitectureTests
    {
        [Test]
        public void Mixer_TrackBinding_CanBeSet()
        {
            // Arrange
            var mixer = new TestMixer();
            var trackBinding = new GameObject("TestBinding");

            // Act
            mixer.TrackBinding = trackBinding;

            // Assert
            Assert.IsNotNull(mixer.TrackBinding);
            Assert.AreEqual(trackBinding, mixer.TrackBinding);

            // Cleanup
            Object.DestroyImmediate(trackBinding);
        }

        [Test]
        public void Mixer_IsLast_ReturnsFalse_WhenEmpty()
        {
            // Arrange
            var mixer = new TestMixer();

            // Act
            var result = mixer.IsLast(new TestBehaviour());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Mixer_IsLast_ReturnsTrue_WhenLast()
        {
            // Arrange
            var mixer = new TestMixer();
            var behaviour1 = new TestBehaviour();
            var behaviour2 = new TestBehaviour();
            
            // Add behaviours via reflection for testing
            var behavioursField = typeof(Mixer).GetField("Behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
            behavioursField?.SetValue(mixer, new List<Behaviour> { behaviour1, behaviour2 });

            // Act
            var result = mixer.IsLast(behaviour2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Mixer_IsLast_ReturnsFalse_WhenNotLast()
        {
            // Arrange
            var mixer = new TestMixer();
            var behaviour1 = new TestBehaviour();
            var behaviour2 = new TestBehaviour();
            
            // Add behaviours via reflection for testing
            var behavioursField = typeof(Mixer).GetField("Behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
            behavioursField?.SetValue(mixer, new List<Behaviour> { behaviour1, behaviour2 });

            // Act
            var result = mixer.IsLast(behaviour1);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Behaviour_CanSetEaseWeight()
        {
            // Arrange
            var behaviour = new TestBehaviour();
            
            // Act
            behaviour.EaseWeight = 0.5f;

            // Assert
            Assert.AreEqual(0.5f, behaviour.EaseWeight);
        }

        [Test]
        public void Behaviour_ClipIsActive_DefaultsToFalse()
        {
            // Arrange & Act
            var behaviour = new TestBehaviour();

            // Assert
            Assert.IsFalse(behaviour.ClipIsActive);
        }
    }

    /// <summary>
    ///     Test implementation of Behaviour for unit testing.
    /// </summary>
    public class TestBehaviour : Behaviour
    {
    }

    /// <summary>
    ///     Test implementation of Mixer for unit testing.
    /// </summary>
    public class TestMixer : Mixer
    {
        protected override void InitializeMixer(Playable playable)
        {
            // Test implementation - no-op
        }
    }
}
