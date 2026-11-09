using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    /// Tests for AnimatorMixer overlap handling and default state fade behavior.
    /// These tests verify that the AnimatorMixer correctly handles:
    /// 1. Overlapping clips (blend between states without default state fade)
    /// 2. Non-overlapping clips (fade back to default state after last clip)
    ///
    /// The key insight is that _activeBehaviour != behaviour check in ClipEaseOutStartedOnce
    /// is the actual mechanism that filters out overlapped clips.
    /// </summary>
    public class AnimatorMixerTests
    {
        private TestAnimatorMixer _mixer;
        private MockAnimatorBehaviour _behaviourA;
        private MockAnimatorBehaviour _behaviourB;
        private GameObject _trackBinding;

        [SetUp]
        public void Setup()
        {
            _trackBinding = new GameObject("TestBinding");
            _mixer = new TestAnimatorMixer(_trackBinding);

            _behaviourA = new MockAnimatorBehaviour("StateA");
            _behaviourB = new MockAnimatorBehaviour("StateB");
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_trackBinding);
            _mixer = null;
            _behaviourA = null;
            _behaviourB = null;
        }

        [Test]
        public void AnimatorMixer_ClipStarted_SetsActiveBehaviour()
        {
            _mixer.InvokeClipStarted(_behaviourA);

            Assert.AreEqual(
                _behaviourA,
                _mixer.GetActiveBehaviour(),
                "ActiveBehaviour should be set to the started clip"
            );
        }

        [Test]
        public void AnimatorMixer_SecondClipStarted_ReplacesActiveBehaviour()
        {
            _mixer.InvokeClipStarted(_behaviourA);
            _mixer.InvokeClipStarted(_behaviourB);

            Assert.AreEqual(
                _behaviourB,
                _mixer.GetActiveBehaviour(),
                "ActiveBehaviour should be replaced by second clip"
            );
        }

        [Test]
        public void AnimatorMixer_ClipEaseOutStartedOnce_ForActiveClip_ClearsState()
        {
            _mixer.InvokeClipStarted(_behaviourA);

            // Simulate ease-out for the SAME behaviour (simulates non-overlapping end)
            _mixer.InvokeClipEaseOutStartedOnce(_behaviourA);

            Assert.IsNull(
                _mixer.GetActiveBehaviour(),
                "ActiveBehaviour should be null after active clip ease-out"
            );
        }

        [Test]
        public void AnimatorMixer_ClipEaseOutStartedOnce_ForNonActiveClip_IsIgnored()
        {
            _mixer.InvokeClipStarted(_behaviourA);
            _mixer.InvokeClipStarted(_behaviourB);

            // Simulate ease-out for the PREVIOUS behaviour (simulates overlapping scenario)
            _mixer.InvokeClipEaseOutStartedOnce(_behaviourA);

            // State should NOT be cleared because behaviourA is no longer active
            Assert.AreEqual(
                _behaviourB,
                _mixer.GetActiveBehaviour(),
                "ActiveBehaviour should still be behaviourB"
            );
        }

        [Test]
        public void AnimatorMixer_OverlapScenario_NoDefaultFadeWhenOverlapping()
        {
            // Setup: Clip A starts
            _mixer.InvokeClipStarted(_behaviourA);
            Assert.AreEqual(1, _mixer.CrossFadeCallCount, "Should crossfade to StateA");
            Assert.AreEqual("StateA", _mixer.LastCrossFadeState, "Should fade to StateA");

            // Clip B starts while A is still active (overlap)
            _mixer.InvokeClipStarted(_behaviourB);
            Assert.AreEqual(2, _mixer.CrossFadeCallCount, "Should crossfade to StateB");
            Assert.AreEqual("StateB", _mixer.LastCrossFadeState, "Should fade to StateB");

            // Clip A ease-out should NOT trigger default state fade (it's not the active clip)
            _mixer.InvokeClipEaseOutStartedOnce(_behaviourA);
            Assert.AreEqual(
                2,
                _mixer.CrossFadeCallCount,
                "Should NOT crossfade to default (overlap scenario)"
            );
        }

        [Test]
        public void AnimatorMixer_NonOverlapScenario_DefaultFadeAtEnd()
        {
            // Setup a track with a default state
            _mixer.SetDefaultState("Idle");

            // Setup: Clip A starts and ends without overlap
            _mixer.InvokeClipStarted(_behaviourA);
            Assert.AreEqual(1, _mixer.CrossFadeCallCount, "Should crossfade to StateA");

            // Clip A ease-out (no successor clip)
            _mixer.InvokeClipEaseOutStartedOnce(_behaviourA);

            // Should fade to default state
            Assert.AreEqual(2, _mixer.CrossFadeCallCount, "Should crossfade to default state");
            Assert.AreEqual(
                "Idle",
                _mixer.LastCrossFadeState,
                "Should fade to DefaultState (Idle)"
            );
        }

        [Test]
        public void AnimatorMixer_ActiveBehaviourGuard_IsPrimaryOverlapFilter()
        {
            // This test verifies that the _activeBehaviour != behaviour check
            // is the actual mechanism preventing default fades during overlap

            _mixer.InvokeClipStarted(_behaviourA);
            var firstBehaviour = _mixer.GetActiveBehaviour();

            // Second clip starts
            _mixer.InvokeClipStarted(_behaviourB);
            var secondBehaviour = _mixer.GetActiveBehaviour();

            // Verify they are different
            Assert.AreNotEqual(
                firstBehaviour,
                secondBehaviour,
                "ActiveBehaviour should change when new clip starts"
            );

            // First clip's ease-out should be ignored
            _mixer.InvokeClipEaseOutStartedOnce(firstBehaviour);
            Assert.AreEqual(
                secondBehaviour,
                _mixer.GetActiveBehaviour(),
                "ActiveBehaviour should remain unchanged"
            );
        }

        [Test]
        public void AnimatorMixer_CurrentState_TracksActiveState()
        {
            _mixer.SetDefaultState("Idle");

            _mixer.InvokeClipStarted(_behaviourA);
            Assert.AreEqual("StateA", _mixer.GetCurrentState(), "CurrentState should track StateA");

            _mixer.InvokeClipStarted(_behaviourB);
            Assert.AreEqual("StateB", _mixer.GetCurrentState(), "CurrentState should track StateB");

            _mixer.InvokeClipEaseOutStartedOnce(_behaviourB);
            Assert.AreEqual(
                "Idle",
                _mixer.GetCurrentState(),
                "CurrentState should be DefaultState after final clip"
            );
        }

        [Test]
        public void AnimatorMixer_NoDefaultState_NoFadeOnClipEnd()
        {
            // Don't set a default state
            _mixer.SetDefaultState(null);

            _mixer.InvokeClipStarted(_behaviourA);
            Assert.AreEqual(1, _mixer.CrossFadeCallCount, "Should crossfade to StateA");

            // Clip ends
            _mixer.InvokeClipEaseOutStartedOnce(_behaviourA);

            // Should NOT fade to anything (no default state)
            Assert.AreEqual(
                1,
                _mixer.CrossFadeCallCount,
                "Should NOT crossfade (no default state)"
            );
        }

        [Test]
        public void AnimatorMixer_AlreadyInDefaultState_NoFade()
        {
            _mixer.SetDefaultState("Idle");

            // Start in default state
            _mixer.SetCurrentState("Idle");

            // Clip with default state name starts
            var defaultBehaviour = new MockAnimatorBehaviour("Idle");
            _mixer.InvokeClipStarted(defaultBehaviour);

            // Should still fade to Idle (state change)
            Assert.AreEqual(1, _mixer.CrossFadeCallCount);

            // Clip ends
            _mixer.InvokeClipEaseOutStartedOnce(defaultBehaviour);

            // Should NOT fade - already in default state
            Assert.AreEqual(
                1,
                _mixer.CrossFadeCallCount,
                "Should NOT crossfade when already in default state"
            );
        }

        #region Test Classes

        /// <summary>
        /// Mock AnimatorBehaviour for testing that doesn't require real TimelineClip
        /// </summary>
        public class MockAnimatorBehaviour : AnimatorBehaviour
        {
            private string _stateName;
            private float _easeInDuration = 0.1f;
            private float _easeOutDuration = 0.1f;

            public MockAnimatorBehaviour(string stateName)
            {
                _stateName = stateName;
                // Use reflection to set the private StateName field in base class
                SetStateName(stateName);
            }

            private void SetStateName(string stateName)
            {
                var field = typeof(AnimatorBehaviour).GetField(
                    "StateName",
                    BindingFlags.Public | BindingFlags.Instance
                );
                if (field != null)
                {
                    field.SetValue(this, stateName);
                }
            }

            public new string StateName => _stateName;
            public float EaseInDuration => _easeInDuration;
            public float EaseOutDuration => _easeOutDuration;

            public void SetEaseInDuration(float duration) => _easeInDuration = duration;

            public void SetEaseOutDuration(float duration) => _easeOutDuration = duration;
        }

        /// <summary>
        /// Testable AnimatorMixer that exposes internals via reflection
        /// </summary>
        public class TestAnimatorMixer : AnimatorMixer
        {
            public int CrossFadeCallCount { get; private set; }
            public string LastCrossFadeState { get; private set; }
            public float LastCrossFadeDuration { get; private set; }

            public TestAnimatorMixer(GameObject trackBinding)
            {
                // Set the TrackBinding via reflection
                var trackBindingField = typeof(Mixer).GetField(
                    "<TrackBinding>k__BackingField",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                trackBindingField?.SetValue(this, trackBinding);
            }

            public void SetDefaultState(string defaultState)
            {
                // AnimatorTrack is a ScriptableObject, not a Component
                // Use CreateInstance instead of AddComponent
                var track = ScriptableObject.CreateInstance<AnimatorTrack>();

                // Set DefaultState via reflection since ScriptableObject fields are serialized
                var defaultStateField = typeof(AnimatorTrack).GetField(
                    "DefaultState",
                    BindingFlags.Public | BindingFlags.Instance
                );
                defaultStateField?.SetValue(track, defaultState ?? "");

                var trackField = typeof(AnimatorMixer).GetField(
                    "Track",
                    BindingFlags.Public | BindingFlags.Instance
                );
                trackField?.SetValue(this, track);
            }

            public void SetCurrentState(string state)
            {
                var field = typeof(AnimatorMixer).GetField(
                    "_currentState",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                field?.SetValue(this, state);
            }

            public AnimatorBehaviour GetActiveBehaviour()
            {
                var field = typeof(AnimatorMixer).GetField(
                    "_activeBehaviour",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                return field?.GetValue(this) as AnimatorBehaviour;
            }

            public string GetCurrentState()
            {
                var field = typeof(AnimatorMixer).GetField(
                    "_currentState",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                return field?.GetValue(this) as string;
            }

            public void InvokeClipStarted(AnimatorBehaviour behaviour)
            {
                var method = typeof(AnimatorMixer).GetMethod(
                    "ClipStarted",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                method?.Invoke(this, new object[] { behaviour });
            }

            public void InvokeClipEaseOutStartedOnce(AnimatorBehaviour behaviour)
            {
                var method = typeof(AnimatorMixer).GetMethod(
                    "ClipEaseOutStartedOnce",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                method?.Invoke(this, new object[] { behaviour });
            }

            protected override void InitializeMixer(Playable playable)
            {
                // Skip base initialization
                var bindingField = typeof(AnimatorMixer).GetField(
                    "Binding",
                    BindingFlags.Public | BindingFlags.Instance
                );
                bindingField?.SetValue(this, null);
            }

            // Override CrossFadeToState to track calls instead of calling Unity API
            protected override void CrossFadeToState(string stateName, float duration)
            {
                CrossFadeCallCount++;
                LastCrossFadeState = stateName;
                LastCrossFadeDuration = duration;
            }
        }

        #endregion
    }
}
