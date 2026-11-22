using System;

namespace PayloadSharp;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class PayloadIgnoreAttribute : Attribute
{
    public bool Ignore { get; }

    public PayloadIgnoreAttribute(bool ignore)
    {
        Ignore = ignore;
    }
}