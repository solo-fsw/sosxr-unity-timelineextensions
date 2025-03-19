using System;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class ExampleClip : Clip<ExampleBehaviour>
    {
        public ExposedReference<Transform> ExampleReference; // An exposed reference is on the Clip


        public override void InitializeClip(IExposedPropertyTable resolver)
        {
            if (GenericBehaviourImplementation is not ExampleBehaviour exampleBehaviour)
            {
                return;
            }

            exampleBehaviour.Example = ExampleReference.Resolve(resolver);
        }
    }
}