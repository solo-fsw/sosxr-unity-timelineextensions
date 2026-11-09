using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     Performance tests for critical paths (Week 2 - Important).
    /// </summary>
    public class PerformanceTests
    {
        private const int IterationCount = 10000;

        [Test]
        public void TMProMixer_DictionaryLookup_FasterThanListIndexOf()
        {
            int behaviourCount = 100;
            var behaviours = new List<TestBehaviour>();
            var behaviourToIndex = new Dictionary<TestBehaviour, int>();
            
            for (int i = 0; i < behaviourCount; i++)
            {
                var behaviour = new TestBehaviour { Id = i };
                behaviours.Add(behaviour);
                behaviourToIndex[behaviour] = i;
            }

            var targetBehaviour = behaviours[behaviourCount / 2];

            var listStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < IterationCount; i++)
            {
                int index = behaviours.IndexOf(targetBehaviour);
            }
            listStopwatch.Stop();

            var dictStopwatch = Stopwatch.StartNew();
            for (int i = 0; i < IterationCount; i++)
            {
                int index = behaviourToIndex.TryGetValue(targetBehaviour, out var idx) ? idx : -1;
            }
            dictStopwatch.Stop();

            UnityEngine.Debug.Log($"List.IndexOf: {listStopwatch.ElapsedMilliseconds}ms, Dictionary.TryGetValue: {dictStopwatch.ElapsedMilliseconds}ms");

            Assert.That(dictStopwatch.ElapsedTicks, Is.LessThan(listStopwatch.ElapsedTicks), 
                "Dictionary lookup should be faster than List.IndexOf");
        }

        [Test]
        public void AreaUnderCurve_Performance_CompletesInReasonableTime()
        {
            var curve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.5f, 1),
                new Keyframe(1, 0)
            );

            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < 1000; i++)
            {
                InvokeAreaUnderCurve(curve);
            }
            
            stopwatch.Stop();

            UnityEngine.Debug.Log($"1000 AreaUnderCurve calls: {stopwatch.ElapsedMilliseconds}ms");

            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000), 
                "1000 AreaUnderCurve calls should complete in less than 1 second");
        }

        private float InvokeAreaUnderCurve(AnimationCurve curve)
        {
            const int steps = 100;
            var sum = 0f;

            for (var i = 0; i < steps; i++)
            {
                var t0 = (float)i / steps;
                var t1 = (float)(i + 1) / steps;
                sum += (curve.Evaluate(t0) + curve.Evaluate(t1)) * 0.5f * (t1 - t0);
            }

            return sum;
        }

        private class TestBehaviour
        {
            public int Id { get; set; }
        }
    }
}
