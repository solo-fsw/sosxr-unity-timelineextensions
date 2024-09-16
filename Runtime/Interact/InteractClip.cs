using System;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
///     Allows us to set the values in the editor
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable]
public class InteractClip : PlayableAsset
{
    private InteractBehaviour template = new();

    public InteractBehaviour Template => template;


    /// <summary>
    ///     Here we write our logic for creating the playable behaviour
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<InteractBehaviour>.Create(graph, Template); // Create a playable, using the constructor

        return playable;
    }
}