using System;
using UnityEngine;

public class TextHelper
{
    public static string FormatThousands(int value)
    {
        return String.Format("{0:0}", Mathf.Max(0, value));
    }

    public static string FormatThousands(uint value)
    {
        return String.Format("{0:0}", Mathf.Max(0, value));
    }

    public static string FormatTime(long millis)
    {
        var span = TimeSpan.FromMilliseconds(Mathf.Max(0, millis));

        if (span.Hours > 0)
            return string.Format("{0:D2}:{1:D2}", span.Hours, span.Minutes);

        return string.Format("{0:D2}:{1:D2}", span.Minutes, span.Seconds);
    }

    public static string MillisToHours(long millis)
    {
        return TimeSpan.FromMilliseconds(Mathf.Max(0, millis)).Hours.ToString();
    }
}