# Timeline Extensions template

Below you can find the four classes that you'd need to build to create your own extensions for Timeline. There are many guides and tutorials available on how to do this, so I will only briefly explain what is different here.

## Why use this template

When you follow most of the tutorials and official guidelines, you may find that you're either writing a fair bit of boilerplate code, or that certain functionality is missing or hard to find.

That's why I've tried to abstract away the confusion into four classes: `Track`, `Mixer`, `Behaviour`, and `Clip`. To benefit from this abstraction, just derive your own from these four.

Below you will see an example of how this could look like if we'd strip away the implementation specific. If you'd copy-paste these into four `.cs` files, you'll get access to a (admittedly useless) Timeline track. You can fill it out with your desired implementation. See the online tutorials, the Timeline official Samples, and the other TimelineExtensions for inspiration on how to do that.

Please let me know if there's anything unclear, and especially if you've made something that is useful to you! I'd love it to have your contributions here too.

## examples

``` csharp name="ExampleTrack.cs"
[TrackClipType(typeof(ExampleClip))]
[TrackBindingType(typeof(Transform))] // Change binding here
public class ExampleTrack : Track
{
    protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
    {
        var playable = ScriptPlayable<ExampleMixer>.Create(graph, inputCount);
        var mixer = playable.GetBehaviour();

        // Now you can do stuff with the Mixer
        mixer.ExampleMixerProperty = 42;
        mixer.TrackBinding = TrackBinding;

        return playable;
    }
}
```

```csharp name="ExampleMixer.cs"
public class ExampleMixer : Mixer
{
    public float ExampleMixerProperty { get; set; }


    protected override void InitializeMixer(Playable playable)
    {
        // This runs before most other things. Use it to setup, get values, etc.
        // This is a good place to cache the Track Binding (the thing / object that you drag into the left side of the Track in the Timeline window)
    }


    protected override void ClipStarted(Behaviour activeBehaviour)
    {
        Debug.Log("Started");
    }


    protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
    {
        var behaviour = (ExampleBehaviour) activeBehaviour;

        if (behaviour == null)
        {
            Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

            return;
        }

        if (behaviour.Example != null)
        {
            Debug.Log(behaviour.Example.name);
        }
        else
        {
            Debug.Log("No reference, maybe couldn't resolve");
        }

        if (activeBehaviour.EaseInDoneOnce)
        {
            Debug.Log("Ease in finished");
        }

        if (activeBehaviour.EaseOutStartedOnce)
        {
            Debug.Log("Ease out has started");
        }
    }


    protected override void ClipEnd(Behaviour activeBehaviour)
    {
        Debug.Log("Clip done");
    }
}
```

```csharp name="ExampleBehaviour.cs"
[Serializable]
public class ExampleBehaviour : Behaviour
{
    public Transform Example; // Data is stored on the Behaviour


    public override void InitializeBehaviour(TimelineClip timelineClip, object trackBinding)
    {
        base.InitializeBehaviour(timelineClip, trackBinding); // Always call this first

        Debug.Log("Any other Behaviour initialization code goes here.");
    }
}
```

```csharp name="ExampleClip.cs"
[Serializable] // Clips need to be serialized
public class ExampleClip : Clip
{
    public ExampleBehaviour Template;
    public ExposedReference<Transform> ExampleReference; // An exposed reference is on the Clip

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ExampleBehaviour>.Create(graph, Template);
        var clone = playable.GetBehaviour();

        clone.InitializeBehaviour(TimelineClip, TrackBinding); // This really should be called here, since it allows you to set up the Behaviour (the clone!) with the correct data

        clone.Example = ExampleReference.Resolve(Resolver); // Resolve the ExposedReference to the actual object

        return playable;
    }

    /// <summary>
    ///     Make sure to call the base method when overriding this method.
    /// </summary>
    /// <param name="trackBinding"></param>
    /// <param name="timelineClip"></param>
    /// <param name="resolver"></param>
    public override void InitializeClip(
        object trackBinding,
        TimelineClip timelineClip,
        IExposedPropertyTable resolver
    )
    {
        base.InitializeClip(trackBinding, timelineClip, resolver);

        // Do other stuff here
    }
}
```
