using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LoopBreakerBase), true)] // That boolean allows us to use this Custom Editor for child classes too
public class LoopBreakerBaseDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var loopBreaker = (LoopBreakerBase) target;

        GUILayout.Space(10f);
        GUILayout.BeginVertical(EditorStyles.helpBox);

        if (GUILayout.Button(nameof(loopBreaker.GoToStart)))
        {
            loopBreaker.GoToStart();
        }
        else if (GUILayout.Button(nameof(loopBreaker.BreakLoop)))
        {
            loopBreaker.BreakLoop();
        }
        else if (GUILayout.Button(nameof(loopBreaker.GoToEnd)))
        {
            loopBreaker.GoToEnd();
        }

        GUILayout.EndVertical();
    }
}