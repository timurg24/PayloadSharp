using System;

namespace PayloadSharp;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class PayloadRequiredAttribute : Attribute
{
    public bool Required { get; }

    public PayloadRequiredAttribute(bool required)
    {
        Required = required;
    }
}