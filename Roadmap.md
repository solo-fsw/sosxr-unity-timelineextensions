# Roadmap Repo

- By: Maarten R. Struijk Wilbrink
- For: Leiden University SOSXR
- Fully open source: Feel free to add to, or modify, anything you see fit.

## Bugs Current.

- [ ] Animator - I encountered small issues where a "previous state" was still active, even though the ease was fully
  finished. Not quite sure what was happening. Band-aid fix was to no longer check for whether the Animator is already
  in the state prior to crossfading to it.

## Future Features

- [x] Animator - Detect AnimationClip length
- [ ] Make work outside of PlayMode
- [ ] Fix EnhancedAudio drawer: show on the TL whether a clip is getting looped in a prettier way
- [ ] Make a plet-compatible version
- [ ] Make lights blendable between clips - This is now not done since it already blends from it's default values (
  whatever you set it in the inspector prior to play)
- [ ] Put names to PostProcess clips
- [ ] Create 'Matching Start value function': right now the easing usually makes a clip start / end at zero value for
  whatever it is controlling. However, what to do with things that are already 'on'? I would like to meet them where
  they're at (e.g. instantiating an AnimationCurve with a startpoint at the current value for the thing)
- [ ] Not quite sure why the `[Button]` sometimes displays and sometimes don't. I can't set it on `Object` because then
  it doesn't work at all.
- [ ] Putting DesignPatterns in
- [ ] Putting TMPro back in
- [ ] Putting ToTarget back in
