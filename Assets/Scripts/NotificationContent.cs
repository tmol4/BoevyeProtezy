using System;
using UnityEngine;

public struct NotificationContent
{
    public double startTime;
    public double endTime;
    public KeyCode key;
    public string message;

    public NotificationContent(double startTime, double endTime, KeyCode key, string text)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.key = key;
        this.message = text;
    }

    static public NotificationContent ParseTag(string tag)
    {
        string[] split = tag.Split('\t');
        double startTime = double.Parse(split[0]);
        double endTime = double.Parse(split[1]);
        if((split[1])[0] == '+')
            endTime += startTime;
        KeyCode key = Enum.Parse<KeyCode>(split[2]);
        string message = split[3].Replace('_', ' ') + " [" + split[2] + "]";
        return new NotificationContent(startTime, endTime, key, message);
    }
}
