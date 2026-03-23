using System;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Shared string-formatting utilities for building Timeline clip display names.
    /// </summary>
    public static class CustomPlayableClipHelper
    {
        public const string Colon = ":";
        public const string Divider = " - ";

        /// <summary>Removes a trailing <see cref="Divider"/> sequence from the display name, if present.</summary>
        /// <param name="dispName">The raw display name string.</param>
        /// <returns>The trimmed display name.</returns>
        public static string RemoveTrailingDivider(string dispName)
        {
            if (string.IsNullOrEmpty(dispName))
            {
                return dispName;
            }

            int removeLast = dispName.LastIndexOf(Divider, StringComparison.Ordinal);

            if (removeLast < 0)
            {
                return dispName;
            }

            dispName = dispName[..removeLast];

            return dispName;
        }

        /// <summary>Returns <paramref name="defaultClipName"/> when <paramref name="dispName"/> is null or empty.</summary>
        /// <param name="dispName">Candidate display name.</param>
        /// <param name="defaultClipName">Fallback used when the candidate is empty.</param>
        public static string SetDisplayNameIfStillEmpty(string dispName, string defaultClipName)
        {
            if (string.IsNullOrEmpty(dispName))
            {
                dispName = defaultClipName;
            }

            return dispName;
        }
    }
}
