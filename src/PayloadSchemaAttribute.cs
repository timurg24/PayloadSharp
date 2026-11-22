using System;

namespace PayloadSharp;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class PayloadSchemaAttribute : Attribute
{
    public string Name { get; }

    public PayloadSchemaAttribute(string name)
    {
        Name = name;
    }
}