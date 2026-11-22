using System;

namespace PayloadSharp;

public struct PayloadEntry
{
    public Type type;
    public string key;
    public string value;
    public string unit;
}