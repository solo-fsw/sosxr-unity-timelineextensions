using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions.PlayModeTests
{
    /// <summary>
    ///     PlayMode integration tests. Each test builds a TimelineAsset in memory, plays it on a real PlayableDirector in
    ///     Manual update mode and steps the graph frame by frame, so results do not depend on the real frame rate.
    ///     PlayMode is needed because the base Behaviour only fires its callbacks while Application.isPlaying.
    /// </summary>
    public class TimelinePlayModeTests
    {
        private const float FrameStep = 1f / 60f;

        private readonly List<Object> _cleanup = new();
        private SimulationMode _previousSimulationMode;
        private float _previousFixedDeltaTime;

        [SetUp]
        public void Setup()
        {
            _previousSimulationMode = Physics.simulationMode;
            _previousFixedDeltaTime = Time.fixedDeltaTime;
            Physics.simulationMode = SimulationMode.Script;
            Time.fixedDeltaTime = 0.02f;
        }

        [TearDown]
        public void Teardown()
        {
            Physics.simulationMode = _previousSimulationMode;
            Time.fixedDeltaTime = _previousFixedDeltaTime;

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

        private TimelineAsset NewTimeline(double fixedDuration)
        {
            var timeline = Register(ScriptableObject.CreateInstance<TimelineAsset>());
            timeline.durationMode = TimelineAsset.DurationMode.FixedLength;
            timeline.fixedDuration = fixedDuration;

            return timeline;
        }

        private T NewTrack<T>(TimelineAsset timeline) where T : TrackAsset, new()
        {
            return Register(timeline.CreateTrack<T>(null, typeof(T).Name));
        }

        private TimelineClip NewClip<TClip>(TrackAsset track, double start, double duration) where TClip : Clip
        {
            var timelineClip = track.CreateClip<TClip>();
            timelineClip.start = start;
            timelineClip.duration = duration;
            Register(timelineClip.asset);

            return timelineClip;
        }

        private PlayableDirector NewDirector(TimelineAsset timeline)
        {
            var director = Register(new GameObject("Director")).AddComponent<PlayableDirector>();
            director.playOnAwake = false;
            director.timeUpdateMode = DirectorUpdateMode.Manual;
            director.extrapolationMode = DirectorWrapMode.Hold;
            director.playableAsset = timeline;

            return director;
        }

        private static void Advance(PlayableDirector director, float seconds, float step = FrameStep)
        {
            var frames = Mathf.RoundToInt(seconds / step);

            for (var i = 0; i < frames; i++)
            {
                director.playableGraph.Evaluate(step);
            }
        }

        #region Looper

        private (PlayableDirector director, LooperControl control) NewLooperTimeline(TimeState state)
        {
            var timeline = NewTimeline(5);
            var track = NewTrack<LooperTrack>(timeline);
            var clip = (LooperClip)NewClip<LooperClip>(track, 0, 1).asset;
            clip.Template = new LooperBehaviour();
            clip.InitialState = state;

            var control = Register(new GameObject("LooperControl")).AddComponent<LooperControl>();
            var director = NewDirector(timeline);
            director.SetGenericBinding(track, control);

            return (director, control);
        }

        [Test]
        public void Looper_LoopingClip_JumpsBackAtClipEnd()
        {
            var (director, _) = NewLooperTimeline(TimeState.Looping);
            director.Play();

            Advance(director, 1.25f);

            Assert.AreEqual(PlayState.Playing, director.state);
            Assert.Less(director.time, 1.0, "Playhead should have looped back into the clip");
        }

        [Test]
        public void Looper_BreakAndContinue_LetsPlayheadLeaveTheClip()
        {
            var (director, control) = NewLooperTimeline(TimeState.Looping);
            director.Play();

            Advance(director, 0.5f);
            control.BreakAndContinue();
            Advance(director, 1f);

            Assert.Greater(director.time, 1.0);
        }

        [Test]
        public void Looper_PauseDuringLoopingClip_StaysPaused()
        {
            var (director, _) = NewLooperTimeline(TimeState.Looping);
            director.Play();
            Advance(director, 0.5f);
            var timeAtPause = director.time;

            director.Pause();

            Assert.AreEqual(PlayState.Paused, director.state, "Pausing a looping clip must not restart the Director");
            Assert.AreEqual(timeAtPause, director.time, 0.0001, "Pausing must not move the playhead to the clip start");
        }

        [Test]
        public void Looper_StopDuringLoopingClip_StaysStopped()
        {
            var (director, _) = NewLooperTimeline(TimeState.Looping);
            director.Play();
            Advance(director, 0.5f);

            director.Stop();

            Assert.AreNotEqual(PlayState.Playing, director.state, "Stopping a looping clip must not restart the Director");
        }

        #endregion

        #region Enhanced Audio

        private AudioClip NewAudioClip(string name, float seconds)
        {
            const int frequency = 44100;

            return Register(AudioClip.Create(name, Mathf.RoundToInt(seconds * frequency), 1, frequency, false));
        }

        private TimelineClip NewAudioTimelineClip(TrackAsset track, AudioClip audio, double start)
        {
            var timelineClip = NewClip<EnhancedAudioClip>(track, start, audio.length);
            var asset = (EnhancedAudioClip)timelineClip.asset;
            asset.Template = new EnhancedAudioBehaviour();
            asset.Audio = audio;

            return timelineClip;
        }

        [Test]
        public void EnhancedAudio_StartMidClip_StartsAudioAtMatchingOffset()
        {
            var timeline = NewTimeline(10);
            var track = NewTrack<EnhancedAudioTrack>(timeline);
            NewAudioTimelineClip(track, NewAudioClip("FourSeconds", 4f), 0);

            var source = Register(new GameObject("Audio")).AddComponent<AudioSource>();
            var director = NewDirector(timeline);
            director.SetGenericBinding(track, source);

            director.time = 2.5;
            director.Play();
            Advance(director, FrameStep);

            if (!source.isPlaying)
            {
                Assert.Ignore("Audio output is disabled in this Editor/run, so AudioSource.time cannot be checked.");
            }

            Assert.AreEqual(2.5f, source.time, 0.1f, "Audio should start where the Timeline clip starts playing, not at 0");
        }

        [Test]
        public void EnhancedAudio_OverlappingClips_OlderClipEndingDoesNotStopNewerClip()
        {
            var timeline = NewTimeline(10);
            var track = NewTrack<EnhancedAudioTrack>(timeline);
            var first = NewAudioClip("First", 3f);
            var second = NewAudioClip("Second", 3f);
            NewAudioTimelineClip(track, first, 0);
            NewAudioTimelineClip(track, second, 2);

            var source = Register(new GameObject("Audio")).AddComponent<AudioSource>();
            var director = NewDirector(timeline);
            director.SetGenericBinding(track, source);
            director.Play();

            Advance(director, 3.5f); // The first clip ended at 3s, the second runs until 5s

            Assert.AreSame(second, source.clip);

            if (AudioPlaybackAvailable())
            {
                Assert.IsTrue(source.isPlaying, "The newer clip must keep playing after the older clip ends");
            }
        }

        private bool AudioPlaybackAvailable()
        {
            var probe = Register(new GameObject("AudioProbe")).AddComponent<AudioSource>();
            probe.clip = NewAudioClip("Probe", 1f);
            probe.Play();
            var available = probe.isPlaying;
            probe.Stop();

            return available;
        }

        #endregion

        #region Rigidbody

        private (PlayableDirector director, Rigidbody body) NewRigidbodyTimeline(ForceMode mode, float amount, double clipDuration, bool isKinematic = false, bool useGravity = false)
        {
            var timeline = NewTimeline(5);
            var track = NewTrack<RigidbodyTrack>(timeline);
            var clip = (RigidbodyClip)NewClip<RigidbodyClip>(track, 0, clipDuration).asset;
            clip.Template = new RigidbodyBehaviour();
            clip.IsKinematic = isKinematic;
            clip.UseGravity = useGravity;
            clip.AddForce = true;
            clip.Amount = amount;
            clip.ForceMode = mode;
            clip.Target = new ExposedReference<Transform> { exposedName = new PropertyName("rigidbodyTarget") };

            var body = Register(new GameObject("Body")).AddComponent<Rigidbody>();
            body.mass = 1f;
            body.useGravity = false;

            var target = Register(new GameObject("Target")).transform;
            target.position = new Vector3(1000, 0, 0);

            var director = NewDirector(timeline);
            director.SetGenericBinding(track, body);
            director.SetReferenceValue(clip.Target.exposedName, target);

            return (director, body);
        }

        /// <summary>Steps the Timeline per rendered frame and physics per fixed step, the way Unity's player loop does.</summary>
        private static void RunFramesWithPhysics(PlayableDirector director, float seconds, float frameDeltaTime)
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            var frames = Mathf.RoundToInt(seconds / frameDeltaTime);
            var accumulator = 0f;

            for (var i = 0; i < frames; i++)
            {
                director.playableGraph.Evaluate(frameDeltaTime);
                accumulator += frameDeltaTime;

                while (accumulator >= fixedDeltaTime - 0.00001f)
                {
                    Physics.Simulate(fixedDeltaTime);
                    accumulator -= fixedDeltaTime;
                }
            }
        }

        private float VelocityAfterOneSecondOfForce(float frameDeltaTime)
        {
            var (director, body) = NewRigidbodyTimeline(ForceMode.Force, 10f, 2);
            director.Play();

            RunFramesWithPhysics(director, 1f, frameDeltaTime);

            var speed = body.linearVelocity.x;
            director.Stop();

            return speed;
        }

        [Test]
        public void Rigidbody_ContinuousForce_IsFrameRateIndependent()
        {
            var slowFrames = VelocityAfterOneSecondOfForce(0.04f); // 25 fps: two physics steps per frame
            var fastFrames = VelocityAfterOneSecondOfForce(0.01f); // 100 fps: one physics step per two frames

            Assert.AreEqual(10f, slowFrames, 0.5f, "10 N on 1 kg for 1 s should give about 10 m/s");
            Assert.AreEqual(10f, fastFrames, 0.5f, "10 N on 1 kg for 1 s should give about 10 m/s");
            Assert.AreEqual(slowFrames, fastFrames, 0.25f, "Frame rate must not change the result");
        }

        [Test]
        public void Rigidbody_VelocityChange_FiresOnce()
        {
            var (director, body) = NewRigidbodyTimeline(ForceMode.VelocityChange, 2f, 2);
            director.Play();

            RunFramesWithPhysics(director, 0.5f, 0.02f);

            Assert.AreEqual(2f, body.linearVelocity.magnitude, 0.05f, "VelocityChange applies once at clip start, not every frame");
        }

        [Test]
        public void Rigidbody_AfterLastClip_RestoresKinematicAndGravity()
        {
            var (director, body) = NewRigidbodyTimeline(ForceMode.Impulse, 0f, 0.5, isKinematic: true);
            body.isKinematic = false;
            body.useGravity = true;
            director.Play();

            RunFramesWithPhysics(director, 0.25f, 0.02f);
            Assert.IsTrue(body.isKinematic, "Inside the clip the clip's values apply");

            RunFramesWithPhysics(director, 0.5f, 0.02f);
            Assert.IsFalse(body.isKinematic, "After the clip the original values come back");
            Assert.IsTrue(body.useGravity);
        }

        #endregion
    }
}
