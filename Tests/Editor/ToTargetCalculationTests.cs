using NUnit.Framework;
using UnityEngine;
using System.Reflection;

namespace SOSXR.TimelineExtensions.Tests
{
    /// <summary>
    ///     Tests for ToTarget clip calculations (Week 1 - Critical Path).
    /// </summary>
    public class ToTargetCalculationTests
    {
        [Test]
        public void AreaUnderCurve_LinearCurve_ReturnsCorrectTriangleArea()
        {
            var curve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(1, 1)
            );

            float area = InvokeAreaUnderCurve(curve);

            Assert.AreEqual(0.5f, area, 0.01f, "Triangle area should be 0.5");
        }

        [Test]
        public void AreaUnderCurve_FlatCurveAtOne_ReturnsFullArea()
        {
            var curve = new AnimationCurve(
                new Keyframe(0, 1),
                new Keyframe(1, 1)
            );

            float area = InvokeAreaUnderCurve(curve);

            Assert.AreEqual(1.0f, area, 0.01f, "Flat curve at 1 should have area of 1");
        }

        [Test]
        public void AreaUnderCurve_FlatCurveAtZero_ReturnsZero()
        {
            var curve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(1, 0)
            );

            float area = InvokeAreaUnderCurve(curve);

            Assert.AreEqual(0.0f, area, 0.01f, "Flat curve at 0 should have area of 0");
        }

        [Test]
        public void AreaUnderCurve_QuadraticCurve_ReturnsReasonableArea()
        {
            // y = x^2 curve: integral from 0 to 1 of x^2 dx = 1/3 ≈ 0.333
            var curve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.5f, 0.25f),
                new Keyframe(1, 1)
            );

            float area = InvokeAreaUnderCurve(curve);

            Assert.That(area, Is.GreaterThan(0.25f).And.LessThan(0.45f), "Quadratic curve (y=x^2) area should be approximately 0.333");
        }

        [Test]
        public void AreaUnderCurve_EaseInCurve_HasLowerArea()
        {
            var easeIn = new AnimationCurve(
                new Keyframe(0, 0, 0, 0),
                new Keyframe(1, 1, 2, 2)
            );

            float area = InvokeAreaUnderCurve(easeIn);

            Assert.That(area, Is.LessThan(0.5f), "Ease-in curve should have area less than linear");
        }

        [Test]
        public void AreaUnderCurve_EaseOutCurve_HasHigherArea()
        {
            var easeOut = new AnimationCurve(
                new Keyframe(0, 0, 2, 2),
                new Keyframe(1, 1, 0, 0)
            );

            float area = InvokeAreaUnderCurve(easeOut);

            Assert.That(area, Is.GreaterThan(0.5f), "Ease-out curve should have area greater than linear");
        }

        [Test]
        public void AreaUnderCurve_SymmetricCurve_AreaAroundHalf()
        {
            var curve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.5f, 1),
                new Keyframe(1, 0)
            );

            float area = InvokeAreaUnderCurve(curve);

            Assert.AreEqual(0.5f, area, 0.05f, "Symmetric curve should have area around 0.5");
        }

        [Test]
        public void AreaUnderCurve_NullCurve_ThrowsException()
        {
            // When invoking via reflection, null reference throws TargetInvocationException
            Assert.Throws<System.Reflection.TargetInvocationException>(() => InvokeAreaUnderCurve(null));
        }

        private float InvokeAreaUnderCurve(AnimationCurve curve)
        {
            var method = typeof(ToTargetClip).GetMethod("AreaUnderCurve", BindingFlags.NonPublic | BindingFlags.Static);
            return (float)method?.Invoke(null, new object[] { curve });
        }
    }
}
