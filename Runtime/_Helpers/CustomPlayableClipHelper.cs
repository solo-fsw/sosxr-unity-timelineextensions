using System;


public static class CustomPlayableClipHelper
{
    public const string Colon = ":";
    public const string Divider = " - ";


    public static string RemoveTrailingDivider(string dispName)
    {
        if (string.IsNullOrEmpty(dispName))
        {
            return dispName;
        }

        var removeLast = dispName.LastIndexOf(Divider, StringComparison.Ordinal);

        if (removeLast < 0)
        {
            return dispName;
        }

        dispName = dispName.Remove(removeLast);

        return dispName;
    }


    public static string SetDisplayNameIfStillEmpty(string dispName, string defaultClipName)
    {
        if (string.IsNullOrEmpty(dispName))
        {
            dispName = defaultClipName;
        }

        return dispName;
    }
}