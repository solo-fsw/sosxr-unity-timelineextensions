# Timeline Extensions

Custom Timeline playable tracks for Unity, developed at [Leiden University SOSXR](https://researchwiki.solo.universiteitleiden.nl/xwiki/bin/view/Main/).

- **Author:** Maarten R. Struijk Wilbrink
- **Package:** `com.sosxr.timelineextensions`
- **Unity:** 6000.0+
- **Dependency:** `com.unity.timeline 1.8.8`

---

## Installation

1. Open the Unity project you want to add this package to.
2. Open **Window → Package Manager**.
3. Click **+** → **Add package from git URL…**
4. Paste the repository URL (ending in `.git`) and click **Add**.

**Dev branch:**
Append `#dev` to the URL to install from the development branch.

**Optional packages** (required only for the matching Samples):

| Sample            | Package                       |
| ----------------- | ----------------------------- |
| Animation Rigging | `com.unity.animation.rigging` |
| Post Processing   | `com.unity.postprocessing`    |

---

## Architecture

Every track follows the same four-class pattern, ensuring consistent behavior and easier debugging across the package:

| Class         | Role                                                                                                        |
| ------------- | ----------------------------------------------------------------------------------------------------------- |
| **Track**     | Extends `TrackAsset`. Declares the binding type and clip type. Creates the Mixer.                           |
| **Clip**      | Extends `PlayableAsset`. Holds Inspector-editable data. Creates the Behaviour playable.                     |
| **Behaviour** | Extends `PlayableBehaviour`. Carries serialized per-clip data at runtime.                                   |
| **Mixer**     | Extends `PlayableBehaviour`. Reads the active Behaviour each frame and applies changes to the bound object. |

The base classes in `Runtime/_Base/` provide shared lifecycle handling (ease tracking, action callbacks) so concrete implementations only need to override a handful of virtual methods.

### Base Behaviour lifecycle callbacks

`Mixer` subscribes to these `Action` fields on each `Behaviour` automatically:

| Action                         | When                                     |
| ------------------------------ | ---------------------------------------- |
| `ClipStartedAction`            | First frame the clip is active           |
| `ClipEaseInDoneOnceAction`     | Exactly once when ease-in completes      |
| `ClipEaseOutStartedOnceAction` | Exactly once when ease-out begins        |
| `ClipEndedAction`              | First frame the clip is no longer active |

Override the corresponding virtual methods in your `Mixer` subclass:

```csharp
protected override void ClipStarted(Behaviour b) { }
protected override void ClipEaseInDoneOnce(Behaviour b) { }
protected override void ClipActive(Behaviour b, float easeWeight) { }  // called every frame
protected override void ClipEaseOutStartedOnce(Behaviour b) { }
protected override void ClipEnd(Behaviour b) { }
```

> **Note:** All tracks run in Play Mode only. Scrubbing in the Editor is not supported.

---

## Tracks

### Animator

**Binding:** `Animator`
**Menu:** `SOSXR.TimelineExtensions > Animator Track`

Drives Animator state transitions from Timeline using `Animator.CrossFadeInFixedTime`. Each clip blends to its target state from whatever state the Animator is currently in.

**Track settings:**

| Field         | Description                                                              |
| ------------- | ------------------------------------------------------------------------ |
| Default State | The idle/default state to return to when no clips are active.            |

**Per-clip settings:**

| Field | Description                                                              |
| ----- | ------------------------------------------------------------------------ |
| State | The Animator state to blend to when this clip becomes active.            |

**How It Works:**

The Animator track uses a simple mental model:

1. **Default State**: Set on the track itself — this is where the Animator returns when no clips are playing
2. **Clip State**: Each clip has one target state
3. **Blending**: Clips automatically crossfade between states using ease-in/out durations

**Crossfade Behavior:**

```
Single Clip:
Idle → [Clip: Walk] → Idle
      (easeIn)     (easeOut)

Overlapping Clips (0.5s overlap):
Idle → Walk → Walk→Run → Run → Idle
      (clip1)  (0.5s)   (clip2)  (end)
```

- **Start**: Blends from Default State to clip's state using ease-in duration
- **Overlap**: Automatically calculates overlap time and blends smoothly between clip states
- **End**: Blends back to Default State using ease-out duration

**Tips:**

- Use a single Animator Controller layer with no state-to-state transitions (except Entry → your default state).
- Overlap clips in Timeline to create automatic crossfades between animation states.
- The ease-in duration controls how long the blend *into* the clip takes.
- The ease-out duration controls how long the blend *back to idle* takes (or to the next overlapping clip).
- Click **Match Duration to Animation** button in the Inspector to resize the Timeline clip to match the animation length.
- Warning displayed if non-looping animation is shorter than Timeline clip (animation will freeze at end).

> [!note] Unity's Animation Track
> Not to be confused with Unity's Animation track, to which this is complementary.
> The Animation track uses the Playables API to play the Animation directly.
> This Animator track drives states inside the Animator.
> The decision to use either mainly rests on how / if you're steering an Animator Controller in other parts of the scene / game / experience. If in those places you're also relying on the Playables API - use the Unity's Animation track.

---

### Enhanced Audio

**Binding:** `AudioSource`
**Menu:** `SOSXR.TimelineExtensions > Enhanced Audio Track`

An `AudioSource`-based audio track with per-clip control over volume, pitch, spatial blend, and distance attenuation. The ease-in/out of each clip acts as an automatic volume fade.

**Per-clip settings:**

| Field                | Description                                                                        |
| -------------------- | ---------------------------------------------------------------------------------- |
| Audio                | The `AudioClip` to play. Setting this snaps the clip duration to the audio length. |
| Max Volume           | Peak volume (0–1). Multiplied by the ease weight each frame.                       |
| Pitch                | Playback pitch (−3 to 3).                                                          |
| Spatial Blend        | 0 = 2D, 1 = 3D.                                                                    |
| Distance             | Min/max distance for rolloff (x = min, y = max).                                   |
| Volume Over Distance | Custom rolloff curve.                                                              |

If the clip is extended beyond the audio length, the audio loops automatically. Click **Match Duration To Clip** to reset it to the audio's exact length.

---

### Interface (Control)

**Binding:** `GameObject` (must have a component implementing `IInterface`)

Calls methods on any `MonoBehaviour` that implements `IInterface` at each phase of the clip:

```csharp
public interface IInterface
{
    void OnClipStart();
    void OnEaseInDone();
    void ClipActive();
    void OnEaseOutStart();
    void OnClipEnd();
}
```

Implement `IInterface` on your own component, bind the GameObject to the track, and your methods will be called at the correct timeline moments. See `Samples~/Samples/InterfaceExample.cs` for a working example.

---

### Lights

**Binding:** `Light`

Lerps a Light's `intensity`, `color`, and `range` between their original values and the clip's target values, driven by the ease weight. The light reverts to its original values when the clip ends.

**Requirements:**

- Light mode must be **Mixed** or **Realtime**.

**Per-clip settings:** Intensity, Color (RGB), Range. The clip label shows configured values at a glance.

---

### Looper

**Binding:** `LooperControl` MonoBehaviour

Can control the playback state of the Timeline itself. Each clip represents a segment with a configured `TimeState`.

There are two states generally useful for setting the Looper to:

| TimeState           | Behaviour                                                                            |
| ------------------- | ------------------------------------------------------------------------------------ |
| `Looping`           | Jumps back to the clip's start when it ends.                                         |
| `TimeScaleZero`     | Sets the Director's speed to 0, pausing Timeline without pausing other game systems. This happens at the start of the clip. |

The below three states are what you would probably want to call programmatically from another section of your code, which has it's own conditional check and implements `LooperControl`. When the check is satisfied, send out one of the below states, to break the Timeline out of it's looper state and continue playing:

| TimeState           | Behaviour                                                                            |
| ------------------- | ------------------------------------------------------------------------------------ |
| `BreakAndContinue`  | Stops looping and continues playback forward.                                        |
| `BreakAndGoToStart` | Jumps to the clip's start and then continues.                                        |
| `BreakAndGoToEnd`   | Jumps to the clip's end and then continues.                                          |

**Runtime control via `LooperControl`:**

```csharp
looperControl.BreakAndContinue(); // break loop, but continue the timeline normally
looperControl.BreakAndGoToStart(); // break loop, and jump back to the beginning of the LooperControl clip
looperControl.BreakAndGoToEnd(); // break loop, and jump back to the end of the LooperControl clip
```

State changes are **buffered** if called before the playhead reaches the clip, and applied automatically once it arrives.

> Each `LooperTrack` must have a **unique** `LooperControl` assigned to it.

---

### Parenting

**Binding:** `Transform` (the parent)

Reparents a child Transform to the track-bound parent Transform while the clip is active. The original parent is restored when the clip ends.

**Per-clip settings:**

| Field             | Description                                                                  |
| ----------------- | ---------------------------------------------------------------------------- |
| Child             | The Transform to reparent (scene reference).                                 |
| Zero In On Parent | If enabled, snaps the child's local position/rotation to zero on attachment. |

---

### Rigidbody

**Binding:** `Rigidbody`

Sets `isKinematic` and `useGravity` on the bound Rigidbody when the clip starts, and optionally fires a one-shot force impulse toward a target Transform.

**Per-clip settings:** `IsKinematic`, `UseGravity`, `AddForce`, `Target`, `Amount`, `ForceMode`.

---

### RotateToTarget

**Binding:** `Transform` (the look-at target)

Slerps a **Rotator** Transform to face the track-bound target Transform during the clip. During ease-out the rotation direction reverses, rotating away from the target. Includes safety checks for the rotator component to prevent null reference errors.

**Per-clip settings:**

| Field       | Description                                                                      |
| ----------- | -------------------------------------------------------------------------------- |
| Axis To Use | Which axes to include (0 = ignore, 1 = use). E.g. `(1,0,1)` for horizontal-only. |
| Rotator     | The Transform that will rotate (ExposedReference — can be any scene object).     |

---

### TextMeshPro

**Binding:** `TextMeshProUGUI`

Sets text content and color on a TMP UI component per clip. The alpha channel is driven by the ease weight, so ease-in/out acts as a text fade.

> **Do not** set the Alpha on the clip color — it is overwritten at runtime.

---

### ToTarget

**Binding:** `GameObject`

Moves and rotates the bound GameObject from a starting point to a destination over the clip's duration. This track uses ease-weighted, frame-rate-independent movement and includes visual debugging via `Debug.DrawRay` when active in the Scene view. The clip duration is automatically calculated from the distance, ease curves, and move speed.

**Per-clip settings:**

| Field        | Description                                                    |
| ------------ | -------------------------------------------------------------- |
| Target       | The destination GameObject (ExposedReference).                 |
| Axis To Use  | Axes to include in displacement (0 = ignore).                  |
| Rotate Speed | Rotation slerp speed toward the direction of travel.           |
| Move Speed   | Translation speed (units/second). Determines clip duration.    |

---

## Code Quality

This package undergoes regular refactoring to ensure consistency:

- **Base Mixer Inheritance:** Components like `TMProMixer` follow the core `Mixer` pattern for reliable lifecycle callbacks.
- **Null Safety:** All tracks include runtime checks to prevent common `NullReferenceException` risks.
- **Performance:** Removed dead code and editor-only debug logs to keep the runtime footprint minimal.

For a full list of recent fixes and improvements, see the [Changelog](CHANGELOG.md).

---

## Editor Enhancements

### Timeline Window

These utilities are applied automatically to the Unity Timeline window in the Editor:

- **Persistent Timeline Selection** — The Timeline window remembers the last-opened Timeline when you click other GameObjects. Note: selection is reset on entering/exiting Play Mode.
- **Edit Multiple Clips** — Select multiple clips and use:
  - `Alt + Arrows` — adjust left edge (change start, keep end)
  - `Cmd/Ctrl + Arrows` — adjust right edge (change end, keep start)
  - `Alt + Cmd/Ctrl + Arrows` — move both edges (move clip)
  - `Alt + = / -` — change ease-in duration
  - `Cmd/Ctrl + = / -` — change ease-out duration

### PlayableDirector Enhanced Editor

Adds **Play / Pause / Stop** buttons and a speed slider to the PlayableDirector Inspector for quick testing during development.

---

## Executive Director

`ExecutiveDirector` is a MonoBehaviour that plays a list of `PlayableDirector` components in sequence, automatically waiting for each one's full duration before starting the next.

**Setup:**

1. Add `ExecutiveDirector` to a GameObject.
2. Add each `PlayableDirector` to the list.
3. Set **Auto Play** (Never / OnAwake / OnStart / OnEnable) or call `PlayAllDirectors()` from code.

The total duration of all directors is shown in the Inspector (read-only). Calling `PlayAllDirectors()` while already running stops all directors and restarts from the first.

---

## Creating a Custom Track

1. Create four files: `MyBehaviour : Behaviour`, `MyClip : Clip`, `MyMixer : Mixer`, `MyTrack : Track`.
2. In `MyTrack`, add `[TrackBindingType(typeof(YourType))]` and `[TrackClipType(typeof(MyClip))]`, then implement `CreateMixer`.
3. In `MyClip`, implement `CreatePlayable` — call `InitializeBehaviour` on the behaviour clone.
4. In `MyMixer`, implement `InitializeMixer` and override the lifecycle methods you need.

See `Samples~/Samples/ExampleImplementation/` for a complete minimal example.

---

## Samples

Import via **Package Manager → Timeline Extensions → Samples**.

| Sample                | Contents                                                                                                                                                                        |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Samples**           | `ExampleImplementation` (minimal custom track), `InterfaceExample`, `InterfaceExampleTwo`, `InterfaceToUnityEvents`, `InterfaceToUnityEventsSO`, `AnimatorEventHandler`         |
| **Animation Rigging** | `RigTrack` — controls rig/constraint weight via Timeline. Requires `com.unity.animation.rigging`.                                                                               |
| **Post Processing**   | `PostProcessingTrack` — blends Post Processing Volume weights. Requires `com.unity.postprocessing`.                                                                             |

---

## Links

- [Unity: Extending Timeline — practical guide](https://blog.unity.com/technology/extending-timeline-a-practical-guide)
- [GameDevGuide — Custom Playables](https://youtu.be/12bfRIvqLW4)
- [Unity Timeline API docs](https://docs.unity3d.com/Packages/com.unity.timeline@1.8/api/UnityEngine.Timeline.ITimeControl.html)
- [SOSXR Research Wiki](https://researchwiki.solo.universiteitleiden.nl/xwiki/bin/view/Main/)
- [Changelog](CHANGELOG.md)
