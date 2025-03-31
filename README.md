# Timeline Extensions

- By: Maarten R. Struijk Wilbrink
- For: Leiden University SOSXR
- Thanks to: [GameDevGuide](https://youtu.be/12bfRIvqLW4) for explaining this in a coherent way.
- More info at: [Unity Blog](https://blog.unity.com/technology/extending-timeline-a-practical-guide)
  and [YouTube](https://www.youtube.com/watch?v=uBPRfcox5hE)

These are some custom Timeline playables for you to use in your own project.
Feel free to add to, or modify, anything you see fit.

# Version 3!

# Installation

1. Open the Unity project you want to install this package in.
2. Open the Package Manager window.
3. Click on the `+` button and select `Add package from git URL...`.
4. Paste the URL of this repo into the text field and press `Add`. Make sure it ends with `.git`.

### For the Dev version:

Do above steps, but add `#dev` to the end of the URL.

#### Requirements

- Timeline
- Animation Rigging (in Samples)
- PostProcessing (in Samples)

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

These SOSXR custom playables are designed to blend (where needed) each animation on the Animator, and smoothly
transition between them. It completely relies on 'CrossFade' between the animations. This is a more robust way of
handling animations than creating a spiderweb of transitions in the Animator.
See [Tarodev's excellent tutorial](https://www.youtube.com/watch?v=ZwLekxsSY3Y&t=1s) on how it works. You don't __need__
any of the transitions in you Animator Controller, so my advice is to remove them. If they are required elsewhere they
can stay, but keep in mind where they might interfere / add to / compete with this system. The simplest solution is
remove all the transitions between states, except for the one from the 'Entry' to a basic Idle animation. This will be
the state that your character will move into once the scene loads.

### Usage

- Add an Animator Track (SOSXR.TimelineExtensions > Animator Track).
- Click 'Add Animator Clip'.
- Use the dropdowns in the inspector to choose which animation (state) will be chosen:
    - Animation State when clip STARTS
    - Animation State when clip ENDS. By default it will select the 'Layer Default State' (the one in the Animator
      Controller with the arrow running from 'Entry') as the 'Animation State when clip ENDS'. If you don't add a state
      to transition to at the end of the clip, it will keep playing that animation, forever.
- Use the ease-in and ease-out times to blend between animations. Note that the blending of the END state starts when
  ease-out starts, and is finished exactly when the clip is done.

### Small word of caution

It may look like you can drag one Timeline clip over the other... but with this one you cannot. Don't.

Also: name the Animator's states in a way that makes sense to you. This way, you can easily find the state you want to
go to.

Lastly: only use one layer in the Animator Controller.

## Enhanced Audio

You can use this instead of the default Timeline Audio if you want to have more control over your played audio clips
from Timeline itself. The default Timeline audio implementation leaves much of the control of the played clip to the
bound AudioSource, whereas this EnhancedAudio brings most of that control to the Timeline clip level.

Uses the EaseIn and EaseOut as Volume multiplier: set the Volume on the Clip to the desired Max volume, and use the
easing of each Clip to gently get into / out of clips.

## Extender

Sometimes one of the other tracks here rely on having completed their action at the end of the clip. However, Timeline
does not always play these correctly if that clip is the last clip of the Timeline Playable graph / Director.
This Extender is simply an empty track with an empty clip. Position the end of the clip in such a way that it is
finished later than any of the other clips (anything later than 0.1 sec should be good enough).
Now all other clips can wrap up their execution gracefully prior to the Timeline ending.

## Control

This versatile custom playable allows you to start a method via Timeline. It uses an interface, `IControl`, with various
methods.

- `OnClipStart` is called when the clip starts.
- `OnEaseInDone`
- `WhileClipIsActive`
- `OnEaseOutStarted`
- `OnClipIsDone` is called when the clip ends.

Create your own class which implements `IControl` (see the two examples in the Samples package), and use the above
methods method to start things when the clip start, and end them when the clip is done.

### A few things to note:

1) Each class you want to control with this needs to have the interface `IControl` attached to it.
2) This is a good example of a Timeline thing that benefits from having the above-mentioned 'Extender', in case it's the
   last clip on the track.

Some examples are provided, however, a better way would be to implement the `IControl` interface into a more
comprehensive communication management system, such as
the [ScriptableObjectArchitecture](https://github.com/solo-fsw/sosxr-unity-scriptableobjectarchitecture)'s GameEvent
system (for small / medium-sized projects), or Mediator.

Further documentation can be found
on [Unity's own documentation page](https://docs.unity3d.com/Packages/com.unity.timeline@1.8/api/UnityEngine.Timeline.ITimeControl.html).

## Lights

The custom Lights Playable allows you to control the light settings through Timeline.

1) The light you wish to control should be set either to 'Mixed' or to 'Realtime'.
   Intensity being affected by inputWeight (easing).
   You can change the allowed range of the intensity in LightsClip.
2) Color is affected by inputWeigh (easing).
3) Range is being affected by inputWeight (easing).
   If you want to change the allowed range, do this in LightsClip.
4) Lights blend from and to their original color, intensity and range.

## Parenting

The custom parenting playable allows you to make a child of the Transform bound in the Clip to the parent (TrackBinding
of the Track).
Can zero out (meaning jump to the parent).
Will revert to original parent on Clip finished.

## Rigidbody

The custom parenting playable allows you to control some values of a Rigidbody.

## RotateToTarget

The thing on the `Clip` rotates towards the thing on the `Track`.
Select which axis you do / don't want to use (e.g. if you want to rotate something horizontally, set 'y' to 0. Elsewise
set to 1).
Have a look at the EaseSpeed. It works in conjunction with the ease in and out of the `Clip`.
The rotation during the ease-out may be more interesting than you've bargained for :).

## TimeControl

### Overview

The `TimeControl` system! The TrackBinding (`TimelineControl`, or any derivative of it) is the thing in control over the
looping / playback of the Timeline. I suggest only to have one, or to have a wild ride.
Set what you want each `Clip` in that `Track` to have for starting state. Use the `TimelineControl` MonoBehaviour for
the well... control.

### TimeState Enum

Defines different playback states:

- `TimeScaleZero`: Stops time of the Timeline, effectively pausing it. See note below.
- `Looping`: Repeats the clip.
- `GoToStart`: Moves to the clip's start, and break the loop.
- `GoToEnd`: Moves to the clip's end, and break the loop.
- `Continue`: Proceeds with the clip, and will not loop at the end.

### Usage

1. **Add a `TimeControlTrack`** to your Timeline.
2. **Create a `TimeControlClip`** and configure its properties.
3. **Derive from `TimelineControl`** to allow external objects to control playback. Use its methods to pause, resume, or
   break loops.
4. Make sure that each TimeControlTrack has a unique TimeController assigned to it.

### Note

Setting the Timeline's timescale is a little different from using the default Pause function. The default Pause will
also pause any components that are controlled by Timeline, but setting the TimeScale to 0 will only pause the Timeline.
This is useful when you want to pause the Timeline, but not the rest of the game.

# In Samples

## PostProcessing

In samples because it requires the PostProcessing (`com.unity.postprocessing`) package to work.
Blend the weight of two separate Volumes.

### Some notes:

Not everything can be blended well (e.g. ACES turned when Tonemapping is first disabled). Play around to see what works.

## Rig Constraints (Animation Rigging)

You need to have Animation Rigging package (`com.unity.animation.rigging`) installed.
This custom playable as been made with version 1.3.0, but should work with other releases too.

This custom playable allows you to set the weight of an entire rig, or of an individual constraint via Timeline.
There's an enum per Clip where you can select whether that Clip governs the weight of the Rig (TrackBinding), or
Constraint (Clip).

## A few things to keep in mind:

1) The Animation Rigging package does not allow transforms to be swapped during play mode. If the position of the IK
   is desired to change: change the position of the target, instead of changing targets mid-stream.
2) All values that require easing (in your setup), should be 0 prior to the first clip starting. This is because
   currently I couldn't get the 'inverse easing' to work correctly. The ease-in worked fine (floatValue * (1 -
   inputWeight),
   instead of floatValue * inputWeight), but this wouldn't do for the ease-out.
   If you happen to work this out, please send us your improvements :)!
3) Can ONLY be run while application is running (Play Mode & Build)
