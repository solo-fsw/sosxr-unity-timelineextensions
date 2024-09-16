using UnityEngine;


public class FloatClip : AnimatorClip
{
    public string floatName = "";
    public float floatValue;
    public float minFloat = 0.02f;


    protected override void SetValuesOnBehaviourFromClip(AnimatorBehaviour behaviour)
    {
        behaviour.floatIndex = Animator.StringToHash(floatName);
        behaviour.floatValue = floatValue;
    }


    protected override string SetDisplayName()
    {
        var dispName = "";

        if (floatName != "")
        {
            dispName += floatName + colon + floatValue;
        }

        return dispName;
    }
}