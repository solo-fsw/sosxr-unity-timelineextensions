# Changelog

All notable changes to this project will be documented in this file.
The changelog format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [Unreleased]

### Fixed

- **IInterface**: Removed illegal `public` access modifiers from interface methods (compilation error)
- **Mixer.OnGraphStart**: Fixed early return bug that skipped remaining inputs when one behaviour was null (changed `return` to `continue`)
- **Track.CreateTrackMixer**: Added null check for PlayableDirector to prevent NullReferenceException
- **ToTargetClip**: Added null guards for AnimationCurve references to prevent NullReferenceException
- **C# Compatibility**: Replaced C# 8/9 features with compatible syntax for broader Unity version support:
  - `Behaviours[^1]` → `Behaviours[Behaviours.Count - 1]`
  - `TrackBinding ??= playerData` → explicit null check
  - Target-typed `new()` → explicit type constructors

### Changed

- **Performance**: Optimized `Mixer.ProcessFrame` to use cached Behaviours list instead of calling `GetInput()` every frame
- **Security**: Added ExposedReference validation with warning logs in ToTargetClip, RigidbodyClip, and RotateToTargetClip
- **Documentation**: Added XML documentation to public APIs (ToTargetClip, RigidbodyClip, RotateToTargetClip CreatePlayable methods, and Mixer.IsLast)

### Added

- **Tests**: Created basic test suite structure with NUnit tests for core architecture

## [0.4.0] -- 2026-03-26

### Fixed

- RotateToTargetMixer: Added null check for rotator to prevent NullReferenceException
- ToTargetBehaviour: Fixed Time.deltaTime → FrameData.deltaTime for Timeline accuracy
- ToTargetBehaviour: Wrapped Debug.DrawRay in #if UNITY_EDITOR
- Bug where ClipStart didn't fire when it happened at the first frame of the graph
- Animator hard-switching between clips instead of smooth blending (now calculates actual overlap duration for crossfades)
- Build compatibility issues with Editor-only AnimatorController APIs
- **Base Behaviour lifecycle**: Complete rewrite of ease tracking state machine
  - Fixed race conditions in EaseInDoneOnce/EaseOutStartedOnce detection
  - Added proper discontinuity detection (seek/loop/jump) with automatic flag re-arming
  - Fixed overlapping ease windows where ease-in and ease-out durations exceed clip length
  - Ensures ClipEaseOutStartedOnceAction fires exactly once even if clip ends before natural ease-out

### Changed

- **Base Behaviour/Mixer architecture**: Complete redesign of lifecycle tracking
  - Behaviour now uses internal _easeInFired/_easeOutOrFallbackFired flags instead of one-shot bool properties
  - Added EvaluateThresholds() with proper overlap clamping logic
  - Mixer.ProcessFrame is now sealed; all per-frame logic goes through ClipActive()
- **ToTarget Track**: Complete rewrite with ease-weighted integral approach
  - Removed: StartingPoint, StoppingDistance, ForceClipLength fields
  - Added: TotalEaseWeightIntegral, AccumulatedEaseTime, NormalizedPosition for frame-rate-independent movement
  - Clips now chain start positions automatically (previous clip's end becomes next clip's start)
  - Clip duration calculated from distance, MoveSpeed, and ease curve integrals
- **TMProMixer**: Refactored to inherit from base Mixer class for consistency
- **PostProcessingMixer**: Refactored to use base Mixer lifecycle callbacks
- **LooperMixer**: Now properly restarts Director when looping at Timeline end (Extender no longer needed)
- **PersistentTimelineSelection**: Enhanced to restore selection when returning from Play Mode
- TimeControl renamed to Looper
- Control renamed to Interface ('Control' was already used by Unity)
- **Animator Track**: Complete redesign with proper crossfade support
  - Track Inspector now shows `Default State` dropdown (configured in track header)
  - Each clip has a single `State` field for the target animation state
  - Seamless crossfading between overlapping clips using calculated overlap duration
  - Returns to `Default State` when no clips are active using ease-out duration
  - Removed complex `ClipPosition` enum and Start/End state fields

### Removed

- **ScreenFader**: Entire feature removed (was incomplete/experimental)
- Behaviour.cs: Removed unused _activationStarted field
- PlayableDirectorExtendedEditor: Removed debug logs from production code
- ExtenderTrack/ExtenderClip: Marked as obsolete (Looper handles end-of-timeline correctly now)
- RotateToTarget: Removed EaseSpeed property (was commented out, now fully removed)
- ToTargetBehaviour: Removed StartingPoint, StoppingDistance, ForceClipLength

### Added

- **Git hooks** (.githooks/): install.sh, post-checkout, pre-commit for automated workflows
- **AnimatorEventHandler sample**: Shows how to respond to Animation Events from Timeline-driven animations
- **CrossFadeAnimatorExtensionMethods**: Major expansion
  - `GetStateAnimationClip()` - retrieves the AnimationClip for a state (supports BlendTrees)
  - `DoesStateLoop()` - checks if state's animation has loopTime enabled
  - `GetStateAnimationLength()` - returns animation clip length
- **Animator Clip Inspector Enhancements**
  - "Match Duration to Animation" button resizes Timeline clip to match animation length
  - Warning shown when non-looping animation clip is shorter than Timeline clip duration
  - Detection of animation `loopTime` property from Animator Controller states
- **API definition files** (.api): Added for all assemblies to support Unity's API Updater

## [0.3.2] - 09-04-2025

### Added

- Icons
- Executive Producer for playing multiple timelines in order

### Changed

- Unity "6000.0"

### Fixed

- Pretty much the entire "what happens when clip starts"-logic
- Lights clip name

## [0.3.1] - 02-04-2025

### Fixed

- Bug fixes when building: Editor methods were still in Runtime capable things
- Bug where I didn't call the Initialize of the ControlBehaviour from the Clip
-

### Changed

- MatchDurationToClips renamed to MatchClipToStartStateDuration, which now also sets the ease to 0
- Animator now uses `Animator.CrossFadeInFixedTime` instead of `Animator.CrossFade`, since that was incompatible with the idea of the [ease in seconds](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Animator.CrossFade.html).

## [0.3.0] - 21-03-2025

> ### Package Numbering Change
> #### Package will now be numbered starting with 0, to better reflect the current status in development (see the official semver information [here](https://semver.org/#spec-item-4)).
>
> If any issues arise when updating from previous (and higher numbered versions), please delete the old version before updating to this version.

### Added

- ~~Context menu actions for the loopbreaker~~
- LoopBreakerBase is now an interface (ILoopBreaker) instead of an abstract class
- Enhanced Audio (from SHINE)
- RotateToTarget (from SHINE)
- TimelineSpeed (from SHINE)
- TimeScale (from SHINE)
- TLActivate (from SHINE)
- Many editors (from SHINE)
- Change duration / position of multiple clips at once:
    - Alt + arrows for left edge (change duration 'from start', leave end position)
    - Cmd / Ctrl + arrows for right edge (change duration 'from end', leave start position)
    - Alt + Cmd / Ctrl + arrows for both edges (move clip)
- Change duration of ease of multiple clips:
    - Alt = or - for left edge (minus for move left, = for move right)
    - Cmd / Ctrl = or - for right edge (minus for move left, = for move right)
- Mediator
- Extender

### Removed

- Removed bool to hand control to other class in Looper: this is default. Always hand control to other class.
- Unused Tracks (moved to future version)
- Many more

### Changed

- Main change: all extend from `Clip` / `Behaviour` / `Mixer` / `Track`
- AnimatorClips / Behaviour now completely rely on CrossFade the animation. This is a more robust way of handling
  animations.
    - Also uses the AnimatorController's "Default State" as it's default exit-clip animation state
- EnhancedAudio now uses EaseIn / EaseOut as volume multiplier
- Renamed all TrackMixers to just Mixer
- Name: Timeline Extensions instead of Extending Timeline
- Much more

## [2.1.0] - 2025-01-31

### Added

- Added samples as a separate thing: download them through the 'Samples' button in the package manager

## [2.0.0] - 2025-01-31

### Changed

- Changed from GNU GPL 3 license to MIT license

## [1.1.1] - 2025-01-07

### Added

- PersistentTimelineSelection.cs now ensures the TimelineWindow remembers the last selected PlayableDirector (when
  non-TimeLine GameObject is chosen)

### Changed

- AnimatorTrackMixer.cs `DoTrigger()` method is now awaitable

## [1.0.2] - 2024-09-16

### Added

- Made as a package for Unity Package Manager

### Fixed

- Typos

### Changed

- Updated licence to GNU GPL v3.0
- Updated README.md to include all separate README files

### Removed

- Removed Odin Inspector dependency
