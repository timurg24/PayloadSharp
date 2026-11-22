using System;

namespace PayloadSharp;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class PayloadRangeAttribute : Attribute
{
    public double Min { get; }
    public double Max { get; }

    public PayloadRangeAttribute(double min, double max)
    {
        Min = min;
        Max = max;
    }
}