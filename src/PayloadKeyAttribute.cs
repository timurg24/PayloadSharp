using System;

namespace PayloadSharp;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class PayloadKeyAttribute : Attribute
{
    public string Name { get; }

    public PayloadKeyAttribute(string name)
    {
        Name = name;
    }
}