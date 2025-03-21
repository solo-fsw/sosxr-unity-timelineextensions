# Timeline Extensions

- By: Maarten R. Struijk Wilbrink
- For: Leiden University SOSXR
- Thanks to: [GameDevGuide](https://youtu.be/12bfRIvqLW4) for explaining this in a coherent way.
- More info at: [Unity Blog](https://blog.unity.com/technology/extending-timeline-a-practical-guide)
  and [YouTube](https://www.youtube.com/watch?v=uBPRfcox5hE)
-

These are some custom Timeline playables for you to use in your own project.
Feel free to add to, or modify, anything you see fit.

Also thanks to [SALSA](https://crazyminnowstudio.com/docs/salsa-lip-sync/addons/timeline-salsa/)

# Installation

1. Open the Unity project you want to install this package in.
2. Open the Package Manager window.
3. Click on the `+` button and select `Add package from git URL...`.
4. Paste the URL of this repo into the text field and press `Add`. Make sure it ends with `.git`.

### For the Dev version:

Do above steps, but add `#dev` to the end of the URL.

#### Requirements

- Timeline
- TextMeshPro
- Animation Rigging
- PostProcessing

# TimeLine Window enhancements

### Enhanced Editor

Buttons for Playing, setting speed, Pausing and Stopping. These are meant for during testing, and not as a robust way of
working with Timeline in the Editor. However, it can still be handy.

### Remember Last Opened Timeline

The Timeline window will now remember the last opened Timeline, even when you select another GameObject in the scene.
This is especially useful when you have multiple Timelines in your scene. However, it forgets when getting in and out of
Playmode, and is generally a bit finicky. However, it is better than nothing. Improvements on the way.

### Edit Multiple Clips

- Change duration / position of multiple clips at once:
    - Alt + arrows for left edge (change duration 'from start', leave end position)
    - Cmd / Ctrl + arrows for right edge (change duration 'from end', leave start position)
    - Alt + Cmd / Ctrl + arrows for both edges (move clip)
- Change duration of ease of multiple clips:
    - Alt = or - for left edge (minus for move left, = for move right)
    - Cmd / Ctrl = or - for right edge (minus for move left, = for move right)

# Timeline Tracks in Main Folder

## Template / Base

### [Leiden University house color](https://huisstijl.leidenuniv.nl/nl/basiselementen/kleuren/)

`[TrackColor(0.0f, 0.17f, 0.88f)]`

### Override

Override `Behaviour`, `Clip`, `Mixer`, and `Track` for your own implementation.

### A small note:

1. You _need_ to override `GetBindingType` and `CreatePlayable` of your derived `Track`.
2. You also _need_ to override `CreatePlayable` on the derived `Clip`.
   - Here you _need_ to call the `Initialize` method on the `Behaviour`.

Look at the examples.

Only works during PlayMode.
## Animator

Control the animations on an Animator (simply!) through Timeline.

This Timeline system looks through the Animator's states, and list each one as a dropdown in the Timeline Clip. This
way, you can easily select the state you want to go to, and the duration / smoothness of the transition.

These SOSXR custom playables are designed to blend (where needed) each animation on the Animator, and smoothly transition between them. It completely relies on 'CrossFade' between the animations. This is a more robust way of handling animations than creating a spiderweb of transitions in the Animator. See [Tarodev's excellent tutorial](https://www.youtube.com/watch?v=ZwLekxsSY3Y&t=1s) on how it works. You don't __need__ any of the transitions in you Animator Controller, so my advice is to remove them. If they are required elsewhere they can stay, but keep in mind where they might interfere / add to / compete with this system. The simplest solution is remove all the transitions between states, except for the one from the 'Entry' to a basic Idle animation. This will be the state that your character will move into once the scene loads.

### Usage

- Add an Animator Track (SOSXR.TimelineExtensions > Animator Track).
- Click 'Add Animator Clip'.
- Use the dropdowns in the inspector to choose which animation (state) will be chosen:
    - Animation State when clip STARTS
    - Animation State when clip ENDS. By default it will select the 'Layer Default State' (the one in the Animator Controller with the arrow running from 'Entry') as the 'Animation State when clip ENDS'. If you don't add a state to transition to at the end of the clip, it will keep playing that animation, forever.
- Use the ease-in and ease-out times to blend between animations. Note that the blending of the END state starts when
  ease-out starts, and is finished exactly when the clip is done.

### Small word of caution

It may look like you can drag one Timeline clip over the other... but with this one you cannot. Don't.

Also: name the Animator's states in a way that makes sense to you. This way, you can easily find the state you want to go to.

Lastly: only use one layer in the Animator Controller.


## Enhanced Audio

You can use this instead of the default Timeline Audio if you want to have more control over your played audio clips from Timeline itself. The default Timeline audio implementation leaves much of the control of the played clip to the bound AudioSource, whereas this EnhancedAudio brings most of that control to the Timeline clip level.

Uses the EaseIn and EaseOut as Volume multiplier: set the Volume on the Clip to the desired Max volume, and use the easing of each Clip to gently get into / out of clips.

## Extender

Sometimes one of the other tracks here rely on having completed their action at the end of the clip. However, Timeline
does not always play these correctly if that clip is the last clip of the Timeline Playable graph / Director.
This Extender is simply an empty track with an empty clip. Position the end of the clip in such a way that it is
finished later than any of the other clips (anything later than 0.1 sec should be good enough).
Now all other clips can wrap up their execution gracefully prior to the Timeline ending.

## Interface

This versatile custom playable allows you to start a method via Timeline. It uses an interface (`ITimeControl`) provided
by Unity's Timeline package. The interface has three methods:

- `OnControlTimeStart` is called when the clip starts.
- `OnControlTimeStop` is called when the clip ends.
- `SetTime` is called on every frame when the clip is playing, and notifies the clip's current time.

Create your own class which implements ITimeControl (see the two examples in the Samples package), and use the above
methods method to start things when the clip start, and end them when the clip is done.

### A few things to note:

1) Each class you want to control with this needs to have the interface ITimelineControl attached to it.
2) There is no track binding. Binding is done on the Clip level.
3) This is a good example of a Timeline thing that benefits from having the above-mentioned 'Extender', in case it's the
   last clip on the track.

Some examples are provided, however, a better way would be to implement the ITimeControl interface into a more
comprehensive communication management system, such as
the [ScriptableObjectArchitecture](https://github.com/solo-fsw/sosxr-unity-scriptableobjectarchitecture)'s GameEvent
system (for small / medium-sized projects), or Mediator.

Further documentation can be found
on [Unity's own documentation page](https://docs.unity3d.com/Packages/com.unity.timeline@1.8/api/UnityEngine.Timeline.ITimeControl.html).

## Lights

The custom Lights Playable allows you to control the light settings through Timeline.

In case you want to use these scripts 'as is':

1) The light you wish to control should be set either to 'Mixed' or to 'Realtime'.
2) Light Intensity range is between 0 and 50. Intensity being affected by inputWeight (easing).
   You can change the allowed range of the intensity in LightsClip.
3) Light Range... uh... range is between 0 and 50. Range is being affected by inputWeight (easing).
   If you want to change the allowed range, do this in LightsClip.
4) All values that require easing (in your setup), should be 0 prior to the first clip starting. This is because
   currently
   I couldn't get the 'inverse easing' to work correctly. The ease-in worked fine (floatValue * (1 - inputWeight),
   instead of floatValue * inputWeight), but this wouldn't do for the ease-out.
   If you happen to work this out, please send us your improvements :)!
5) The previousIndex in the TrackMixer is there to make sure that behaviour is only turned on/off once instead of on
   every frame.
6) Can be run while application is running (Play Mode & Build) as well as in the Editor (Timeline window)

## Parenting

The custom parenting playable allows you to parent a gameobject (the one the track is bound to), to another (the one on
the clip).

In case you want to use these scripts 'as is', there are three things to keep in mind:

1) The Timeline system works on a ScriptableObject base. This means (amongst other things) that it actually cannot
   keep track of references to gameobjects / transforms / components in the scene, other than the ones your track is
   bound to.
   Therefore, the clip needs to find any references you link to the clip. In this case it's done by first setting the
   clip-name
   to the name of the bound GameObject. Then, if the reference to that GameObject is lost, it searches your scene for
   the correct one.
   Therefore, make sure you only have one gameobject with that name in the scene, and that the name of the clip is the
   same as the
   gameobject you're interested in.
2) It is recommended to have that 'parent' be an object which is a child of the actual parent you want to use. Then, let
   the
   track-gameobject parent itself to this parent, and adjust the position and the rotation of this gameobject.
   For example:
   I have a flashlight which I want my character to hold in her right hand.
   I create a child of the right hand, called 'RightHandFlashLightHolder'.
   I bind the track to the flashlight.
   In the clip, I reference 'RightHandFlashLightHolder' as my parent.
   During PlayMode, I pause the Timeline once the flashlight is parented to the 'RightHandFlashLightHolder'.
   I adjust rotation and position of the 'RightHandFlashLightHolder', so that the flashlight seems to be held correctly.
   I save the values of the 'RightHandFlashLightHolder' transform (either copy those values, or use something
   like [PlayModeSaver](https://assetstore.unity.com/packages/tools/utilities/play-mode-saver-104836).
   Now, every time the flashlight is parented to the right hand (via the 'RightHandFlashLightHolder'), it looks to be
   correct.
3) Can ONLY be run while application is running (Play Mode & Build). You can change this if you want, however, keep in
   mind
   that Unity remembers values changed during Edit mode, including those via Timeline.

## Rigidbody

The custom parenting playable allows you to control some values of a Rigidbody.

Can ONLY be run while application is running (Play Mode & Build). You can change this if you want, however, keep in mind
that Unity remembers values changed during Edit mode, including those via Timeline.

## RotateToTarget

## TimeControl

### Overview

The `TimeControl` system extends Unity's Timeline to provide advanced control over clip playback, including looping,
pausing, and time navigation.

### Components

#### TimeControlClip

A playable asset defining a segment in the Timeline with time control behavior. It determines how a clip interacts with
the Timeline's time scale and looping.

#### TimeControlBase

An abstract MonoBehaviour providing an interface for external objects to break loops or pause the Timeline.Derive from
this class to allow external objects to control playback. Use it's methods to pause, resume, or break loops. It also
receives info on when the clip where it's referenced starts and ends through Unity's [
`ITimeControl`](https://docs.unity3d.com/Packages/com.unity.timeline@1.8/api/UnityEngine.Timeline.ITimeControl.html)
interface. `OnControlTimeStart` and `OnControlTimeStop` are called when the clip starts and ends, respectively.

#### TimeState Enum

Defines different playback states:

- `TimeScaleZero`: Stops time of the Timeline, effectively pausing it. See note below.
- `Looping`: Repeats the clip.
- `GoToStart`: Moves to the clip's start, and break the loop.
- `GoToEnd`: Moves to the clip's end, and break the loop.
- `Continue`: Proceeds with the clip, and will not loop at the end.

### Usage

1. **Add a `TimeControlTrack`** to your Timeline.
2. **Create a `TimeControlClip`** and configure its properties.
3. **Derive from `TimeControlBase`** to allow external objects to control playback. Use its methods to pause, resume, or
   break loops.
4. Make sure that each TimeControlClip has a unique TimeController assigned to it. This is done in the clip's inspector.
   You cannot reuse.

### Note

Setting the Timeline's timescale is a little different from using the default Pause function. The default Pause will
also pause any components that are controlled by Timeline, but setting the TimeScale to 0 will only pause the Timeline.
This is useful when you want to pause the Timeline, but not the rest of the game.

## TLActivate

## ToTarget

This allows you to set a target to go to, from any gameobject. Once the playhead hits the clip, it will move && rotate
the
gameobject towards that target.

A couple of points to note:

1) Will draw a ray from the origin to the target.
2) Can use easing.
3) Many values in ToTargetClip are their for observation only, and should not be amended manually (see point 5).
4) The 'CreatePlayable' on the ToTargetTrack is overwritten almost verbatim. The only addition is the:
   var currentClip = (ToTargetClip) clip.asset; and currentClip.TimelineClip = clip; lines. This allow us to have the
   Timelineclip
   in our control, for instance for getting and setting duration and easing values.
5) Can partly be run (drawing target Ray) while application is running (Play Mode & Build) as well as in the Editor (
   Timeline window),
   but will not actually move or rotate the object, unless in PlayMode/Build.

# In Samples

## Design Patterns

In Samples because it requires [SeaShark](https://github.com/solo-fsw/sosxr-unity-seashark) to work.

### Mediator

## Text Mesh Pro (TextMeshPro)

You need to have Text Mesh Pro (Unity UGUI in Unity 6) package installed.
This custom playable as been made with version 1.0.3, but should work with other releases too.

A fairly direct copy of Matt's implementation of the Subtitle Playable (Renamed to TMPro instead of Subtitle).

A couple of points to note:

1) Set the color of the text in the inspector.
2) Make sure not to use alpha in setting the color: this is controlled by the easing, and allows for fading the text in
   and out.
   Don't use easing if you dont want the text to fade.
3) The 'previousIndex' in the TrackMixer is there to make sure that behaviour is only turned on/off once instead of on
   every frame.
4) Can be run while application is running (Play Mode & Build) as well as in the Editor (Timeline window)

## PostProcessing

In samples because it requires the PostProcessing (`com.unity.postprocessing`) package to work.
The custom PostProcessing VolumeProfile Playable allows you to control the LookupTexture (ColorLookup) on a
VolumeProfile through Timeline.
This allows you to change the overall tone (and emotion) of the scene through Timeline.

In case you want to use these scripts 'as is', there are three things to keep in mind:

1) Add a Volume Profile to the track, and set the Texture you wish to use in the Clip settings.
2) The LUT Contribution is default set to max, but can range anywhere from 0 to 1.
3) All values that require easing (in your setup), should be 0 prior to the first clip starting. This is because
   currently
   I couldn't get the 'inverse easing' to work correctly. The ease-in worked fine (floatValue * (1 - inputWeight),
   instead of floatValue * inputWeight), but this wouldn't do for the ease-out.
   If you happen to work this out, please send us your improvements :)!
4) Checks to see if the override states in the Volume Profile are set to true prior to running the
5) Can ONLY be run while application is running (Play Mode & Build)
6) Can easily be extended to control other Volume parameters as well. Make sure to grab the correct component,
   (see PostProcessingTrackMixer for ColorLookup example):

``` Csharp
   private Tonemapping toneMapping;
   private Bloom bloom;
   private ChromaticAberration chromaticAberration;
   private Vignette vignette;
   private LensDistortion lensDistortion;
   private ColorLookup colorLookup;

   private void GetOverrides(VolumeProfile data)
   {
   data.TryGet(out toneMapping);
   data.TryGet(out bloom);
   data.TryGet(out chromaticAberration);
   data.TryGet(out vignette);
   data.TryGet(out lensDistortion);
   data.TryGet(out colorLookup);
   }
```

Afterward, set each of the values (usually float) using the respective component + '.value', like you see in the
"SetValuesToComponent" method in the PostProcessingTrackMixer class.

## Rig Constraings (Animation Rigging)

You need to have Animation Rigging package (`com.unity.animation.rigging`) installed.
This custom playable as been made with version 1.0.3, but should work with other releases too.

This custom playable allows you to set the value of an entire rig via Timeline. Therefore, the best setup is to have
all constraints which govern a single rig or single movement as a single rig, and have different rigs govern different
actions.

A few things to keep in mind:

1) The Animation Rigging package does not allow transforms to be swapped during play mode. If the position of the IK
   is desired to change: change the position of the target, instead of changing targets mid-stream.
2) All values that require easing (in your setup), should be 0 prior to the first clip starting. This is because
   currently
   I couldn't get the 'inverse easing' to work correctly. The ease-in worked fine (floatValue * (1 - inputWeight),
   instead of floatValue * inputWeight), but this wouldn't do for the ease-out.
   If you happen to work this out, please send us your improvements :)!
3) Can ONLY be run while application is running (Play Mode & Build)
