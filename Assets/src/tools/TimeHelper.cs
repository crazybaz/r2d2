using System;

public class TimeHelper
{
    public static double GetCurrentMilliseconds
    {
        get { return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds; }
    }
}