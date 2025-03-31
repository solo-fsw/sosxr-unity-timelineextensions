# Changelog

All notable changes to this project will be documented in this file.
The changelog format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

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
