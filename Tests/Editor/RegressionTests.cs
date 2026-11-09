using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SOSXR.TimelineExtensions.Editor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     EditMode regression tests for the issues fixed in 0.6.0 (code review 2026-11-09).
    /// </summary>
    public class RegressionTests
    {
        private readonly List<Object> _cleanup = new();

        [TearDown]
        public void Teardown()
        {
            foreach (var obj in _cleanup)
            {
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }

            _cleanup.Clear();
        }

        private T Register<T>(T obj) where T : Object
        {
            _cleanup.Add(obj);

            return obj;
        }

        private GameObject NewGameObject(string name)
        {
            return Register(new GameObject(name));
        }

        private static void InitializeMixer(Mixer mixer, object binding)
        {
            mixer.TrackBinding = binding;
            typeof(Mixer).GetMethod("InitializeMixer", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(mixer, new object[] { Playable.Null });
        }

        #region Mixer subscriptions

        private class CountingMixer : Mixer
        {
            public int Started;
            public int Ended;

            protected override void InitializeMixer(Playable playable) { }

            protected override void ClipStarted(Behaviour activeBehaviour)
            {
                Started++;
            }

            protected override void ClipEnd(Behaviour activeBehaviour)
            {
                Ended++;
            }
        }

        private class FiringBehaviour : Behaviour
        {
            public void FireStarted() => ClipStartedAction?.Invoke(this);
            public void FireEnded() => ClipEndedAction?.Invoke(this);
        }

        [Test]
        public void Mixer_RegisterBehavioursTwice_DoesNotStackHandlers()
        {
            var mixer = new CountingMixer();
            var behaviour = new FiringBehaviour();

            mixer.RegisterBehaviours(new List<Behaviour> { behaviour });
            mixer.RegisterBehaviours(new List<Behaviour> { behaviour });
            mixer.RegisterBehaviours(new List<Behaviour> { behaviour });

            behaviour.FireStarted();
            behaviour.FireEnded();

            Assert.AreEqual(1, mixer.Started);
            Assert.AreEqual(1, mixer.Ended);
        }

        [Test]
        public void Mixer_RegisterBehaviours_ReplacingSet_UnsubscribesOldBehaviours()
        {
            var mixer = new CountingMixer();
            var oldBehaviour = new FiringBehaviour();
            var newBehaviour = new FiringBehaviour();

            mixer.RegisterBehaviours(new List<Behaviour> { oldBehaviour });
            mixer.RegisterBehaviours(new List<Behaviour> { newBehaviour });

            oldBehaviour.FireStarted();

            Assert.AreEqual(0, mixer.Started);
        }

        [Test]
        public void Mixer_RegisterBehaviours_WithoutTimelineClip_DoesNotThrow()
        {
            var mixer = new CountingMixer();

            Assert.DoesNotThrow(() => mixer.RegisterBehaviours(new List<Behaviour> { new FiringBehaviour(), null, new FiringBehaviour() }));
            Assert.IsTrue(mixer.IsLast(null) == false);
        }

        [Test]
        public void Mixer_OnGraphStartTwice_OnRealGraph_FiresOnce()
        {
            var graph = PlayableGraph.Create("RegressionTests");

            try
            {
                var mixerPlayable = ScriptPlayable<CountingMixer>.Create(graph, 1);
                var inputPlayable = ScriptPlayable<FiringBehaviour>.Create(graph);
                graph.Connect(inputPlayable, 0, mixerPlayable, 0);

                var mixer = mixerPlayable.GetBehaviour();
                mixer.OnGraphStart(mixerPlayable);
                mixer.OnGraphStart(mixerPlayable); // Happens after Director.Pause() + Play()

                inputPlayable.GetBehaviour().FireStarted();

                Assert.AreEqual(1, mixer.Started);
            }
            finally
            {
                graph.Destroy();
            }
        }

        #endregion

        #region Behaviour

        [TestCase(0.98, 0.02, 1.0, ExpectedResult = true)]
        [TestCase(1.0, 0.0, 1.0, ExpectedResult = true)]
        [TestCase(0.5, 0.016, 1.0, ExpectedResult = false)]
        [TestCase(0.0, 0.0, 2.0, ExpectedResult = false)]
        [TestCase(0.5, -1.0, 1.0, ExpectedResult = false)]
        [TestCase(0.0, 0.0, 0.0, ExpectedResult = true)]
        public bool Behaviour_IsNaturalEnd(double time, double delta, double duration)
        {
            return Behaviour.IsNaturalEnd(time, delta, duration);
        }

        #endregion

        #region Looper

        [TestCase(TimeState.Looping, true, ExpectedResult = true)]
        [TestCase(TimeState.BreakAndGoToStart, true, ExpectedResult = true)]
        [TestCase(TimeState.Looping, false, ExpectedResult = false)]
        [TestCase(TimeState.BreakAndGoToStart, false, ExpectedResult = false)]
        [TestCase(TimeState.BreakAndContinue, true, ExpectedResult = false)]
        [TestCase(TimeState.TimeScaleZero, true, ExpectedResult = false)]
        public bool Looper_ShouldRestartClip_OnlyAfterNaturalEnd(TimeState state, bool reachedEnd)
        {
            return LooperMixer.ShouldRestartClip(state, reachedEnd);
        }

        private class TestLooperMixer : LooperMixer
        {
            public void Start(Behaviour b) => ClipStarted(b);
            public void Active(Behaviour b) => ClipActive(b, 1f);
            public void End(Behaviour b) => ClipEnd(b);
        }

        [Test]
        public void Looper_WithoutDirector_DoesNotThrow()
        {
            var control = NewGameObject("Looper").AddComponent<LooperControl>();
            var mixer = new TestLooperMixer { LooperControl = control };
            var behaviour = new LooperBehaviour { CurrentState = TimeState.Looping, ReachedEnd = true };

            Assert.DoesNotThrow(() =>
            {
                mixer.Start(behaviour);
                mixer.Active(behaviour);
                mixer.End(behaviour);
                mixer.SetTimelineSpeed(0);
            });
        }

        [Test]
        public void Looper_InterruptedClip_KeepsLooperControlPointedAtClip()
        {
            var control = NewGameObject("Looper").AddComponent<LooperControl>();
            var mixer = new TestLooperMixer { LooperControl = control };
            var behaviour = new LooperBehaviour { CurrentState = TimeState.BreakAndContinue };

            mixer.Start(behaviour);
            behaviour.ReachedEnd = false; // e.g. Director.Pause()
            mixer.End(behaviour);

            Assert.AreSame(behaviour, control.ClipInTimeline);
        }

        [Test]
        public void Looper_FinishedClip_ClearsLooperControl()
        {
            var control = NewGameObject("Looper").AddComponent<LooperControl>();
            var mixer = new TestLooperMixer { LooperControl = control };
            var behaviour = new LooperBehaviour { CurrentState = TimeState.BreakAndContinue };

            mixer.Start(behaviour);
            behaviour.ReachedEnd = true;
            mixer.End(behaviour);

            Assert.IsNull(control.ClipInTimeline);
            Assert.IsTrue(behaviour.ClipIsDone);
        }

        [Test]
        public void LooperControl_SetNullWithBufferedState_DoesNotThrow()
        {
            var control = NewGameObject("Looper").AddComponent<LooperControl>();
            control.BufferedState = TimeState.Looping;

            Assert.DoesNotThrow(() => control.ClipInTimeline = null);
            Assert.AreEqual(TimeState.Looping, control.BufferedState, "Buffered state must wait for a real clip");
        }

        #endregion

        #region Enhanced Audio

        [TestCase(0.0, 2f, false, ExpectedResult = 0f)]
        [TestCase(1.5, 2f, false, ExpectedResult = 1.5f)]
        [TestCase(2.5, 2f, false, ExpectedResult = -1f)]
        [TestCase(2.5, 2f, true, ExpectedResult = 0.5f)]
        [TestCase(-1.0, 2f, true, ExpectedResult = 0f)]
        [TestCase(1.0, 0f, true, ExpectedResult = -1f)]
        public float EnhancedAudio_ComputeAudioStartTime(double localTime, float length, bool loop)
        {
            return EnhancedAudioMixer.ComputeAudioStartTime(localTime, length, loop);
        }

        private class TestAudioMixer : EnhancedAudioMixer
        {
            public void Start(Behaviour b) => ClipStarted(b);
            public void End(Behaviour b) => ClipEnd(b);

            public Behaviour Current => (Behaviour)typeof(EnhancedAudioMixer).GetField("_currentBehaviour", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(this);
        }

        [Test]
        public void EnhancedAudio_OlderClipEnding_DoesNotReleaseNewerClip()
        {
            var source = NewGameObject("Audio").AddComponent<AudioSource>();
            var mixer = new TestAudioMixer();
            InitializeMixer(mixer, source);

            var a = new EnhancedAudioBehaviour();
            var b = new EnhancedAudioBehaviour();

            mixer.Start(a);
            mixer.Start(b);
            mixer.End(a);

            Assert.AreSame(b, mixer.Current, "Clip B must keep the AudioSource after overlapping clip A ends");
            Assert.IsFalse(EnhancedAudioMixer.ShouldStopOnEnd(a, b));

            mixer.End(b);

            Assert.IsNull(mixer.Current);
            Assert.IsTrue(EnhancedAudioMixer.ShouldStopOnEnd(b, b));
        }

        [Test]
        public void EnhancedAudioClip_MatchDurationToClip_SetsDurationToAudioLength()
        {
            var timeline = Register(ScriptableObject.CreateInstance<TimelineAsset>());
            var track = Register(timeline.CreateTrack<EnhancedAudioTrack>(null, "Audio"));
            var timelineClip = track.CreateClip<EnhancedAudioClip>();
            var asset = Register((EnhancedAudioClip)timelineClip.asset);
            asset.Audio = Register(AudioClip.Create("TwoSeconds", 44100 * 2, 1, 44100, false));

            asset.InitializeClip(null, timelineClip, null);
            Assert.AreEqual(2.0, timelineClip.duration, 0.01, "New audio snaps the duration");

            timelineClip.duration = 5.0;
            asset.InitializeClip(null, timelineClip, null);
            Assert.AreEqual(5.0, timelineClip.duration, 0.01, "Extended duration (looping) is kept");

            typeof(EnhancedAudioClip).GetMethod("MatchDurationToClip", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(asset, null);
            Assert.AreEqual(2.0, timelineClip.duration, 0.01);
        }

        [Test]
        public void EnhancedAudioClip_MatchDurationToClip_WithoutAudio_DoesNotThrow()
        {
            var asset = Register(ScriptableObject.CreateInstance<EnhancedAudioClip>());
            var method = typeof(EnhancedAudioClip).GetMethod("MatchDurationToClip", BindingFlags.NonPublic | BindingFlags.Instance)!;

            Assert.DoesNotThrow(() => method.Invoke(asset, null));
        }

        [Test]
        public void EnhancedAudioClip_PreviousAudio_IsSerialized()
        {
            var field = typeof(EnhancedAudioClip).GetField("_previousAudio", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(field);
            Assert.IsNotNull(field.GetCustomAttribute<SerializeField>(), "_previousAudio must survive a domain reload");
        }

        #endregion

        #region Rotate To Target

        [Test]
        public void RotateToTarget_ComputeLookRotation_ZeroDirection_ReturnsFallback()
        {
            var fallback = Quaternion.Euler(0, 45, 0);

            var result = RotateToTargetMixer.ComputeLookRotation(Vector3.zero, new Vector3(0, 5, 0), new Vector3Int(1, 0, 1), fallback);

            Assert.AreEqual(fallback, result);
        }

        [Test]
        public void RotateToTarget_ComputeLookRotation_IgnoresMaskedAxis()
        {
            var result = RotateToTargetMixer.ComputeLookRotation(Vector3.zero, new Vector3(0, 10, 10), new Vector3Int(1, 0, 1), Quaternion.identity);

            var forward = result * Vector3.forward;
            Assert.AreEqual(0f, forward.y, 0.0001f);
            Assert.AreEqual(1f, forward.z, 0.0001f);
        }

        private class TestRotateMixer : RotateToTargetMixer
        {
            public void Start(Behaviour b) => ClipStarted(b);
            public void Active(Behaviour b, float w) => ClipActive(b, w);
            public void End(Behaviour b) => ClipEnd(b);
        }

        [Test]
        public void RotateToTarget_ClipEnd_RestoresRotatingTransform_NotTarget()
        {
            var rotating = NewGameObject("Rotating").transform;
            var target = NewGameObject("Target").transform;
            target.position = new Vector3(10, 0, 0);
            target.rotation = Quaternion.Euler(0, 0, 30);

            var mixer = new TestRotateMixer();
            InitializeMixer(mixer, rotating);
            var behaviour = new RotateToTargetBehaviour { Rotator = target };

            mixer.Start(behaviour);
            mixer.Active(behaviour, 1f);
            Assert.AreNotEqual(Quaternion.identity, rotating.rotation, "Bound transform should turn toward the target");

            mixer.End(behaviour);

            Assert.That(Quaternion.Angle(Quaternion.identity, rotating.rotation), Is.LessThan(0.01f));
            Assert.That(Quaternion.Angle(Quaternion.Euler(0, 0, 30), target.rotation), Is.LessThan(0.01f), "Target must not be touched");
        }

        [Test]
        public void RotateToTarget_NoBinding_DoesNotThrow()
        {
            var target = NewGameObject("Target").transform;
            var mixer = new TestRotateMixer();
            var behaviour = new RotateToTargetBehaviour { Rotator = target };

            Assert.DoesNotThrow(() =>
            {
                mixer.Start(behaviour);
                mixer.Active(behaviour, 1f);
                mixer.End(behaviour);
            });
        }

        #endregion

        #region Animator

        private class TestAnimatorMixer : AnimatorMixer
        {
            public readonly List<string> CrossFades = new();
            public void Start(Behaviour b) => ClipStarted(b);

            protected override void CrossFadeToState(string stateName, float duration)
            {
                CrossFades.Add(stateName);
            }
        }

        [Test]
        public void AnimatorMixer_ClipStarted_WithoutDefaultState_StillCrossfades()
        {
            var mixer = new TestAnimatorMixer { Track = Register(ScriptableObject.CreateInstance<AnimatorTrack>()) };
            mixer.Track.DefaultState = "";

            mixer.Start(new AnimatorBehaviour { StateName = "Wave" });

            CollectionAssert.AreEqual(new[] { "Wave" }, mixer.CrossFades);
        }

        private AnimatorController CreateControllerWithNestedState()
        {
            var controller = Register(new AnimatorController());
            controller.AddLayer("Base Layer");
            var root = controller.layers[0].stateMachine;
            root.AddState("Idle");
            root.AddStateMachine("Sub").AddState("Nested");

            return controller;
        }

        [Test]
        public void HasState_FindsStateInSubStateMachine()
        {
            var animator = NewGameObject("Animator").AddComponent<Animator>();
            animator.runtimeAnimatorController = CreateControllerWithNestedState();

            Assert.IsTrue(animator.HasState("Idle"));
            Assert.IsTrue(animator.HasState("Nested"));
            Assert.IsFalse(animator.HasState("Missing"));
        }

        [Test]
        public void HasState_WorksThroughOverrideController()
        {
            var animator = NewGameObject("Animator").AddComponent<Animator>();
            var overrideController = Register(new AnimatorOverrideController(CreateControllerWithNestedState()));
            animator.runtimeAnimatorController = overrideController;

            Assert.IsTrue(animator.HasState("Nested"));
            Assert.AreSame(overrideController.runtimeAnimatorController, CrossFadeAnimatorExtensionMethods.GetAnimatorController(animator));
        }

        [Test]
        public void HasState_NoController_ReturnsFalse()
        {
            var animator = NewGameObject("Animator").AddComponent<Animator>();

            Assert.IsFalse(animator.HasState("Idle"));
            Assert.IsFalse(((Animator)null).HasState("Idle"));
        }

        [Test]
        public void GetStateNames_IncludesNestedStates()
        {
            var animator = NewGameObject("Animator").AddComponent<Animator>();
            animator.runtimeAnimatorController = CreateControllerWithNestedState();

            var names = animator.GetStateNames();

            CollectionAssert.Contains(names, "Idle");
            CollectionAssert.Contains(names, "Nested");
        }

        [Test]
        public void GetDefaultEntryStateName_NoController_ReturnsEmpty()
        {
            var animator = NewGameObject("Animator").AddComponent<Animator>();

            Assert.AreEqual("", animator.GetDefaultEntryStateName());
        }

        #endregion

        #region Lights

        [Test]
        public void LightsBehaviour_DefaultColor_IsInZeroToOneRange()
        {
            var color = new LightsBehaviour().Color;

            Assert.LessOrEqual(color.maxColorComponent, 1f);
        }

        [Test]
        public void LightsBehaviour_NormalizeLegacyColor()
        {
            var legacy = LightsBehaviour.NormalizeLegacyColor(new Color(255f, 244f, 214f));
            Assert.AreEqual(1f, legacy.r, 0.0001f);
            Assert.AreEqual(244f / 255f, legacy.g, 0.0001f);

            var normal = new Color(0.2f, 0.4f, 0.6f);
            Assert.AreEqual(normal, LightsBehaviour.NormalizeLegacyColor(normal));
        }

        private class TestLightsMixer : LightsMixer
        {
            public void Start(Behaviour b) => ClipStarted(b);
            public void Active(Behaviour b, float w) => ClipActive(b, w);
            public void EaseOut(Behaviour b) => ClipEaseOutStartedOnce(b);
            public void End(Behaviour b) => ClipEnd(b);
        }

        [Test]
        public void LightsMixer_EaseOut_FollowsEaseWeight_AndRestoresOnEnd()
        {
            var light = NewGameObject("Light").AddComponent<Light>();
            light.intensity = 1f;
            light.range = 10f;
            light.color = Color.white;

            var mixer = new TestLightsMixer();
            InitializeMixer(mixer, light);
            var behaviour = new LightsBehaviour { Intensity = 3f, Range = 20f, Color = Color.red };

            mixer.Start(behaviour);
            mixer.Active(behaviour, 1f);
            Assert.AreEqual(3f, light.intensity, 0.001f);

            mixer.EaseOut(behaviour);
            mixer.Active(behaviour, 0.5f);
            Assert.AreEqual(2f, light.intensity, 0.001f, "Halfway through ease-out the light is halfway back");
            Assert.AreEqual(15f, light.range, 0.001f);

            mixer.Active(behaviour, 0f);
            Assert.AreEqual(1f, light.intensity, 0.001f);

            mixer.End(behaviour);
            Assert.AreEqual(1f, light.intensity, 0.001f);
            Assert.AreEqual(10f, light.range, 0.001f);
            Assert.AreEqual(Color.white, light.color);
        }

        [Test]
        public void LightsMixer_ClipWithoutEaseOut_SnapsBackOnEnd()
        {
            var light = NewGameObject("Light").AddComponent<Light>();
            light.intensity = 1f;

            var mixer = new TestLightsMixer();
            InitializeMixer(mixer, light);
            var behaviour = new LightsBehaviour { Intensity = 5f };

            mixer.Start(behaviour);
            mixer.Active(behaviour, 1f);
            mixer.End(behaviour);

            Assert.AreEqual(1f, light.intensity, 0.001f);
        }

        #endregion

        #region Parenting

        private class TestParentingMixer : ParentingMixer
        {
            public void Start(Behaviour b) => ClipStarted(b);
            public void End(Behaviour b) => ClipEnd(b);
        }

        [Test]
        public void Parenting_KeepWorldPosition_KeepsWorldPose()
        {
            var parent = NewGameObject("Parent").transform;
            parent.position = new Vector3(5, 0, 0);
            var child = NewGameObject("Child").transform;
            child.position = new Vector3(1, 2, 3);

            ParentingMixer.Attach(child, parent, KeepPosition.KeepWorldPosition);

            Assert.AreSame(parent, child.parent);
            Assert.That(Vector3.Distance(new Vector3(1, 2, 3), child.position), Is.LessThan(0.0001f));
        }

        [Test]
        public void Parenting_KeepLocalPosition_KeepsLocalValues()
        {
            var parent = NewGameObject("Parent").transform;
            parent.position = new Vector3(5, 0, 0);
            var child = NewGameObject("Child").transform;
            child.position = new Vector3(1, 2, 3);

            ParentingMixer.Attach(child, parent, KeepPosition.KeepLocalPosition);

            Assert.That(Vector3.Distance(new Vector3(1, 2, 3), child.localPosition), Is.LessThan(0.0001f));
        }

        [Test]
        public void Parenting_ZeroOutCompletely_ZeroesLocalPose()
        {
            var parent = NewGameObject("Parent").transform;
            parent.SetPositionAndRotation(new Vector3(5, 1, 0), Quaternion.Euler(0, 90, 0));
            var child = NewGameObject("Child").transform;
            child.SetPositionAndRotation(new Vector3(1, 2, 3), Quaternion.Euler(10, 20, 30));

            ParentingMixer.Attach(child, parent, KeepPosition.ZeroOutCompletely);

            Assert.That(child.localPosition.magnitude, Is.LessThan(0.0001f));
            Assert.That(Quaternion.Angle(Quaternion.identity, child.localRotation), Is.LessThan(0.01f));
            Assert.That(Vector3.Distance(parent.position, child.position), Is.LessThan(0.0001f));
        }

        [Test]
        public void Parenting_ClipEnd_RestoresParentPositionAndRotation()
        {
            var originalParent = NewGameObject("OriginalParent").transform;
            var parent = NewGameObject("Parent").transform;
            parent.SetPositionAndRotation(new Vector3(5, 1, 0), Quaternion.Euler(0, 90, 0));
            var child = NewGameObject("Child").transform;
            child.SetParent(originalParent);
            child.SetPositionAndRotation(new Vector3(1, 2, 3), Quaternion.Euler(10, 20, 30));
            var rotation = child.rotation;

            var mixer = new TestParentingMixer();
            InitializeMixer(mixer, parent);
            var behaviour = new ParentingBehaviour { Child = child, KeepPosition = KeepPosition.ZeroOutCompletely };

            mixer.Start(behaviour);
            Assert.AreSame(parent, child.parent);

            mixer.End(behaviour);
            Assert.AreSame(originalParent, child.parent);
            Assert.That(Vector3.Distance(new Vector3(1, 2, 3), child.position), Is.LessThan(0.0001f));
            Assert.That(Quaternion.Angle(rotation, child.rotation), Is.LessThan(0.01f));
        }

        [Test]
        public void Parenting_NullChild_DoesNotThrow()
        {
            var parent = NewGameObject("Parent").transform;
            var mixer = new TestParentingMixer();
            InitializeMixer(mixer, parent);
            var behaviour = new ParentingBehaviour();

            UnityEngine.TestTools.LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("no Child"));
            Assert.DoesNotThrow(() =>
            {
                mixer.Start(behaviour);
                mixer.End(behaviour);
            });
        }

        #endregion

        #region Rigidbody

        [TestCase(ForceMode.Impulse, ExpectedResult = true)]
        [TestCase(ForceMode.VelocityChange, ExpectedResult = true)]
        [TestCase(ForceMode.Force, ExpectedResult = false)]
        [TestCase(ForceMode.Acceleration, ExpectedResult = false)]
        public bool Rigidbody_IsOneShot(ForceMode mode)
        {
            return RigidbodyMixer.IsOneShot(mode);
        }

        [Test]
        public void Rigidbody_ContinuousForceScale_GivesSameImpulsePerSecondAtAnyFrameRate()
        {
            const float fixedDt = 0.02f;
            var at30 = 30 * RigidbodyMixer.ContinuousForceScale(1f / 30f, fixedDt) * fixedDt;
            var at144 = 144 * RigidbodyMixer.ContinuousForceScale(1f / 144f, fixedDt) * fixedDt;

            Assert.AreEqual(1f, at30, 0.0001f);
            Assert.AreEqual(1f, at144, 0.0001f);
            Assert.AreEqual(0f, RigidbodyMixer.ContinuousForceScale(0.016f, 0f));
        }

        private class TestRigidbodyMixer : RigidbodyMixer
        {
            public void Start(Behaviour b) => ClipStarted(b);
            public void End(Behaviour b) => ClipEnd(b);
        }

        [Test]
        public void Rigidbody_RestoresKinematicAndGravity_AfterLastClip()
        {
            var body = NewGameObject("Body").AddComponent<Rigidbody>();
            body.isKinematic = false;
            body.useGravity = true;

            var mixer = new TestRigidbodyMixer();
            InitializeMixer(mixer, body);
            var a = new RigidbodyBehaviour { IsKinematic = true, UseGravity = false };
            var b = new RigidbodyBehaviour { IsKinematic = true, UseGravity = false };

            mixer.Start(a);
            mixer.Start(b); // Overlap
            mixer.End(a);
            Assert.IsTrue(body.isKinematic, "Still inside clip B");

            mixer.End(b);
            Assert.IsFalse(body.isKinematic);
            Assert.IsTrue(body.useGravity);
        }

        [Test]
        public void Rigidbody_RestoreDisabled_KeepsClipValues()
        {
            var body = NewGameObject("Body").AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            var mixer = new TestRigidbodyMixer { RestoreStateOnEnd = false };
            InitializeMixer(mixer, body);
            var drop = new RigidbodyBehaviour { IsKinematic = false, UseGravity = true };

            mixer.Start(drop);
            mixer.End(drop);

            Assert.IsFalse(body.isKinematic);
            Assert.IsTrue(body.useGravity);
        }

        #endregion

        #region Samples

        [TestCase("assets/samples/timeline extensions/0.6.0/animationrigging/rigbehaviour.cs", ExpectedResult = true)]
        [TestCase("assets/_sosxr/sosxr-unity-timelineextensions/samples/animationrigging/rigbehaviour.cs", ExpectedResult = false)]
        [TestCase("packages/com.sosxr.timelineextensions/samples/animationrigging/rigbehaviour.cs", ExpectedResult = false)]
        public bool SampleImportHandler_OnlyReactsToImportedSampleCopies(string path)
        {
            return SampleImportHandler.IsUnderImportedSamplesFolder(path);
        }

        #endregion
    }
}
