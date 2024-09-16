# ExtendingTimeline

- By: Maarten R. Struijk Wilbrink
- For: Leiden University SOSXR
- Thanks to: [GameDevGuide](https://youtu.be/12bfRIvqLW4)
- More info at: [Unity Blog](https://blog.unity.com/technology/extending-timeline-a-practical-guide)

These are some custom Timeline playables for you to use in your own project.
Feel free to add to, or modify, anything you see fit.



## Animator
The custom Animator Playable allows you to control the animations on an animator through Timeline.
It was always possible to control the Animator through Timeline, but you'd have to specify each animation from Timeline itself.
This made blending between animations controlled on the Animator, and animations controlled through Timeline, quite difficult.
These custom playables are designed to control the Parameters specified on your Animator instead of the Animations themselves.
These custom playables should theoretically be able to work in conjunction with the standard Timeline Animation tracks.

In case you want to use these scripts 'as is', there are three things to keep in mind:
1) The floats X, Y, and Z need to be present in the Animator you're controlling. Even if you are not using them to drive
   your own animations, you need to have these named in the Animator. They are set using the x/y/z of the Vector3 'movement'.
2) The other more 'custom' variables need to be present in the Animator you're controlling if you are using them. So if
   set a name for either the int, bool, trigger or custom float, you need to have that one specified in your Animator, so make
   sure you check for spelling.
3) All values that require easing (in your setup), should be 0 prior to the first clip starting. This is because currently
   I couldn't get the 'inverse easing' to work correctly. The ease-in worked fine (floatValue * (1 - inputWeight),
   instead of floatValue * inputWeight), but this wouldn't do for the ease-out.
   If you happen to work this out, please send us your improvements :)!
4) Test the rounding of the integer (in the AnimatorTrackMixer) in your project. It is done so you can use easing with
   integers as well, but might give weird results, depending on your setup. If rounding doesn't work as expected, remove the
   Mathf.RoundToInt() and the multiplication with the inputWeight call.
5) Can ONLY be run while application is running (Play Mode & Build)



## Interact

This versatile custom playable allows you to start a method via Timeline. It uses an interface (IInteract), and a method Interact();
Create your own class which implements IInteract, and use the Interact() method to either put in all your desired behaviour (see ExampleOneInteracting),
or link to your own method inside the interface method (see ExampleTwoInteracting).

A few things to note:
1) Each class you want to control with this needs to have the interface IInteract implemented, and the Interact() method as well.
2) The track binding is the class which has the IInteract interface attached to it.
3) The method will be called once per clip
4) Can be ONLY run while application is running (Play Mode & Build)



## Lights

The custom Lights Playable allows you to control the light settings through Timeline.

In case you want to use these scripts 'as is':
1) The light you wish to control should be set either to 'Mixed' or to 'Realtime'.
2) Light Intensity range is between 0 and 50. Intensity being affected by inputWeight (easing).
   You can change the allowed range of the intensity in LightsClip.
3) Light Range... uh... range is between 0 and 50. Range is being affected by inputWeight (easing).
   If you want to change the allowed range, do this in LightsClip.
4) All values that require easing (in your setup), should be 0 prior to the first clip starting. This is because currently
   I couldn't get the 'inverse easing' to work correctly. The ease-in worked fine (floatValue * (1 - inputWeight),
   instead of floatValue * inputWeight), but this wouldn't do for the ease-out.
   If you happen to work this out, please send us your improvements :)!
5) The previousIndex in the TrackMixer is there to make sure that behaviour is only turned on/off once instead of on every frame.
6) Can be run while application is running (Play Mode & Build) as well as in the Editor (Timeline window)


## Looper

The custom Looper Playable allows you to loop a clip until a certain condition has been met.
The condition is broken by the LoopBreakerBase. Create a derived class from LoopBreakerBase, and implement the BreakLoop() method to simply have it stop looping, but continue where it is. 
Use GoToStart() to have it loop back to the start of the clip, use GoToEnd() to have it loop back to the end of the clip.



## Parenting

The custom parenting playable allows you to parent a gameobject (the one the track is bound to), to another (the one on the clip).


In case you want to use these scripts 'as is', there are three things to keep in mind:
1) The Timeline system works on a ScriptableObject base. This means (amongst other things) that it actually cannot
   keep track of references to gameobjects / transforms / components in the scene, other than the ones your track is bound to.
   Therefore, the clip needs to find any references you link to the clip. In this case it's done by first setting the clip-name
   to the name of the bound GameObject. Then, if the reference to that GameObject is lost, it searches your scene for the correct one.
   Therefore, make sure you only have one gameobject with that name in the scene, and that the name of the clip is the same as the
   gameobject you're interested in.
2) It is recommended to have that 'parent' be an object which is a child of the actual parent you want to use. Then, let the
   track-gameobject parent itself to this parent, and adjust the position and the rotation of this gameobject.
   For example:
   I have a flashlight which I want my character to hold in her right hand.
   I create a child of the right hand, called 'RightHandFlashLightHolder'.
   I bind the track to the flashlight.
   In the clip, I reference 'RightHandFlashLightHolder' as my parent.
   During PlayMode, I pause the Timeline once the flashlight is parented to the 'RightHandFlashLightHolder'.
   I adjust rotation and position of the 'RightHandFlashLightHolder', so that the flashlight seems to be held correctly.
   I save the values of the 'RightHandFlashLightHolder' transform (either copy those values, or use something like [PlayModeSaver](https://assetstore.unity.com/packages/tools/utilities/play-mode-saver-104836).
   Now, every time the flashlight is parented to the right hand (via the 'RightHandFlashLightHolder'), it looks to be correct.
3) Can ONLY be run while application is running (Play Mode & Build). You can change this if you want, however, keep in mind
   that Unity remembers values changed during Edit mode, including those via Timeline.


## PostProcessing

The custom PostProcessing VolumeProfile Playable allows you to control the LookupTexture (ColorLookup) on a VolumeProfile through Timeline.
This allows you to change the overall tone (and emotion) of the scene through Timeline.

In case you want to use these scripts 'as is', there are three things to keep in mind:
1) Add a Volume Profile to the track, and set the Texture you wish to use in the Clip settings.
2) The LUT Contribution is default set to max, but can range anywhere from 0 to 1.
3) All values that require easing (in your setup), should be 0 prior to the first clip starting. This is because currently
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

You need to have Animation Rigging package installed.
This custom playable as been made with version 1.0.3, but should work with other releases too.

This custom playable allows you to set the value of an entire rig via Timeline. Therefore, the best setup is to have
all constraints which govern a single rig or single movement as a single rig, and have different rigs govern different actions.

A few things to keep in mind:
1) The Animation Rigging package does not allow transforms to be swapped during play mode. If the position of the IK
   is desired to change: change the position of the target, instead of changing targets mid-stream. 
2) All values that require easing (in your setup), should be 0 prior to the first clip starting. This is because currently
   I couldn't get the 'inverse easing' to work correctly. The ease-in worked fine (floatValue * (1 - inputWeight),
   instead of floatValue * inputWeight), but this wouldn't do for the ease-out.
   If you happen to work this out, please send us your improvements :)!
3) Can ONLY be run while application is running (Play Mode & Build)


## Rigidbody

The custom parenting playable allows you to control some values of a Rigidbody.

Can ONLY be run while application is running (Play Mode & Build). You can change this if you want, however, keep in mind
that Unity remembers values changed during Edit mode, including those via Timeline.



## TMPro (TextMeshPro)

You need to have Text Mesh Pro package installed.
This custom playable as been made with version 1.0.3, but should work with other releases too.

A fairly direct copy of Matt's implementation of the Subtitle Playable (Renamed to TMPro instead of Subtitle).

A couple of points to note:
1) Set the color of the text in the inspector.
2) Make sure not to use alpha in setting the color: this is controlled by the easing, and allows for fading the text in and out.
   Don't use easing if you dont want the text to fade.
3) The 'previousIndex' in the TrackMixer is there to make sure that behaviour is only turned on/off once instead of on every frame.
4) Can be run while application is running (Play Mode & Build) as well as in the Editor (Timeline window)



## ToTarget

This allows you to set a target to go to, from any gameobject. Once the playhead hits the clip, it will move && rotate the
gameobject towards that target.

A couple of points to note:
1) Will draw a ray from the origin to the target.
2) Can use easing.
3) Many values in ToTargetClip are their for observation only, and should not be amended manually (see point 5). 
4) The 'CreatePlayable' on the ToTargetTrack is overwritten almost verbatim. The only addition is the:
   var currentClip = (ToTargetClip) clip.asset; and currentClip.TimelineClip = clip; lines. This allow us to have the Timelineclip
   in our control, for instance for getting and setting duration and easing values. 
5) Can partly be run (drawing target Ray) while application is running (Play Mode & Build) as well as in the Editor (Timeline window),
   but will not actually move or rotate the object, unless in PlayMode/Build.