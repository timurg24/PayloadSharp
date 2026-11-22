using System;

namespace PayloadSharp;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class PayloadUnitAttribute : Attribute
{
    public string Unit { get; }

    public PayloadUnitAttribute(string unit)
    {
        Unit = unit;
    }
}