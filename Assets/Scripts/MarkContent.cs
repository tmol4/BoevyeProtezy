using UnityEngine;

public struct MarkContent
{
    public double clickTime;
    public int beat;
    public int meter;

    public MarkContent(double clickTime, int beat, int meter)
    {
        this.clickTime = clickTime;
        this.beat = beat % meter;
        this.meter = meter;
    }
}
