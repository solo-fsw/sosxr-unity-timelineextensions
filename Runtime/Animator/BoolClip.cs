using System;


[Serializable]
public class BoolClip : AnimatorClip
{
    protected override void SetClipOnBehaviour(AnimatorBehaviour behaviour)
    {
        behaviour.animatorClip = this;
    }


    protected override void SetValuesOnBehaviourFromClip(AnimatorBehaviour behaviour)
    {
    }


    protected override string SetDisplayName()
    {
        var displayName = "";

        if (template.boolName != "")
        {
            displayName += template.boolName + colon + template.boolValue;
        }

        if (template.resetBool)
        {
            displayName += " [RESET]";
        }

        return displayName;
    }
}