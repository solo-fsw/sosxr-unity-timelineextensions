using System;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ExampleClip : Clip<ExampleBehaviour>
    {
        public ExposedReference<Transform> ExampleReference; // An exposed reference is on the Clip
    }
}