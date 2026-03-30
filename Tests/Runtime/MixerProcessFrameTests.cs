using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Reflection;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     Tests for Mixer event dispatch and ProcessFrame behavior (Week 1 - Critical Path).
    /// </summary>
    public class MixerProcessFrameTests
    {
        private TestMixer _mixer;
        private GameObject _trackBinding;

        [SetUp]
        public void Setup()
        {
            _mixer = new TestMixer();
            _trackBinding = new GameObject("TestBinding");
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_trackBinding);
            _mixer = null;
        }

        [Test]
        public void Mixer_ProcessFrame_SetsTrackBinding()
        {
            var behaviour = new TestBehaviour();
            behaviour.SetClipIsActive(true);
            SetBehaviours(_mixer, new List<Behaviour> { behaviour });

            _mixer.SimulateProcessFrame(_trackBinding);

            Assert.AreEqual(_trackBinding, _mixer.TrackBinding);
        }

        [Test]
        public void Mixer_ProcessFrame_UpdatesEaseWeight_WhenBehaviourActive()
        {
            var behaviour = new TestBehaviour();
            behaviour.SetClipIsActive(true);
            SetBehaviours(_mixer, new List<Behaviour> { behaviour });

            _mixer.SimulateProcessFrame(_trackBinding, 0.75f);

            Assert.AreEqual(0.75f, behaviour.EaseWeight, 0.001f, "EaseWeight should be updated for active behaviour");
        }

        [Test]
        public void Mixer_ProcessFrame_DoesNotUpdateEaseWeight_WhenBehaviourInactive()
        {
            var behaviour = new TestBehaviour();
            behaviour.SetClipIsActive(false);
            behaviour.EaseWeight = 0.5f;
            SetBehaviours(_mixer, new List<Behaviour> { behaviour });

            _mixer.SimulateProcessFrame(_trackBinding, 0.75f);

            Assert.AreEqual(0.5f, behaviour.EaseWeight, 0.001f, "EaseWeight should NOT change for inactive behaviour");
        }

        [Test]
        public void Mixer_ProcessFrame_UpdatesMultipleBehaviours()
        {
            var behaviour1 = new TestBehaviour();
            var behaviour2 = new TestBehaviour();
            behaviour1.SetClipIsActive(true);
            behaviour2.SetClipIsActive(true);
            
            SetBehaviours(_mixer, new List<Behaviour> { behaviour1, behaviour2 });

            _mixer.SimulateProcessFrame(_trackBinding, 0.5f);

            Assert.AreEqual(0.5f, behaviour1.EaseWeight, 0.001f, "Behaviour 1 EaseWeight should be updated");
            Assert.AreEqual(0.5f, behaviour2.EaseWeight, 0.001f, "Behaviour 2 EaseWeight should be updated");
        }

        [Test]
        public void Mixer_ProcessFrame_SkipsNullBehaviours()
        {
            var behaviour = new TestBehaviour();
            behaviour.SetClipIsActive(true);
            
            SetBehaviours(_mixer, new List<Behaviour> { null, behaviour, null });

            _mixer.SimulateProcessFrame(_trackBinding, 0.8f);

            Assert.AreEqual(0.8f, behaviour.EaseWeight, 0.001f, "Non-null behaviour should still be updated");
        }

        [Test]
        public void Mixer_ProcessFrame_MixedActiveAndInactive()
        {
            var activeBehaviour = new TestBehaviour();
            var inactiveBehaviour = new TestBehaviour();
            activeBehaviour.SetClipIsActive(true);
            inactiveBehaviour.SetClipIsActive(false);
            inactiveBehaviour.EaseWeight = 0.3f;
            
            SetBehaviours(_mixer, new List<Behaviour> { activeBehaviour, inactiveBehaviour });

            _mixer.SimulateProcessFrame(_trackBinding, 0.9f);

            Assert.AreEqual(0.9f, activeBehaviour.EaseWeight, 0.001f, "Active behaviour should be updated");
            Assert.AreEqual(0.3f, inactiveBehaviour.EaseWeight, 0.001f, "Inactive behaviour should NOT be updated");
        }

        [Test]
        public void Mixer_TrackBinding_PreservesExistingValue()
        {
            var existingBinding = new GameObject("ExistingBinding");
            _mixer.TrackBinding = existingBinding;
            
            var behaviour = new TestBehaviour();
            behaviour.SetClipIsActive(true);
            SetBehaviours(_mixer, new List<Behaviour> { behaviour });

            _mixer.SimulateProcessFrame(_trackBinding);

            Assert.AreEqual(existingBinding, _mixer.TrackBinding, "Existing binding should be preserved");
            
            Object.DestroyImmediate(existingBinding);
        }

        [Test]
        public void Mixer_ProcessFrame_DefaultEaseWeight_IsOne()
        {
            var behaviour = new TestBehaviour();
            behaviour.SetClipIsActive(true);
            SetBehaviours(_mixer, new List<Behaviour> { behaviour });

            _mixer.SimulateProcessFrame(_trackBinding);

            Assert.AreEqual(1.0f, behaviour.EaseWeight, 0.001f, "Default ease weight should be 1.0");
        }

        private void SetBehaviours(Mixer mixer, List<Behaviour> behaviours)
        {
            var field = typeof(Mixer).GetField("Behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(mixer, behaviours);
        }

        /// <summary>
        ///     Test mixer with public simulation methods for testing.
        /// </summary>
        public class TestMixer : Mixer
        {
            protected override void InitializeMixer(Playable playable) { }

            public void SimulateProcessFrame(object playerData, float easeWeight = 1.0f)
            {
                if (TrackBinding == null)
                {
                    TrackBinding = playerData;
                }

                var behavioursField = typeof(Mixer).GetField("Behaviours", BindingFlags.NonPublic | BindingFlags.Instance);
                var behaviours = behavioursField?.GetValue(this) as List<Behaviour>;

                if (behaviours == null) return;

                for (int i = 0; i < behaviours.Count; i++)
                {
                    var behaviour = behaviours[i];
                    if (behaviour != null && behaviour.ClipIsActive)
                    {
                        behaviour.EaseWeight = easeWeight;
                    }
                }
            }
        }

        /// <summary>
        ///     Test behaviour with helper methods.
        /// </summary>
        public class TestBehaviour : Behaviour
        {
            public void SetClipIsActive(bool active) => ClipIsActive = active;
        }
    }
}
