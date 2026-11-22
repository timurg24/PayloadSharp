using PayloadSharp;
using System;

public class Program
{
    public static void Main()
    {
        PayloadSharp.Payload file = new Payload();
        AircraftData aircraft =
            file.LoadFile<AircraftData>("example.pay");

        
    }
}

[PayloadSchema("Aircraft")]
public struct AircraftData
{
    [PayloadKey("name")]
    public string Name;

    [PayloadKey("crew")]
    [PayloadRange(1,5)]
    public int Crew;

    [PayloadKey("length")]
    [PayloadUnit("m")]
    public float Length;

    [PayloadKey("wingspan")]
    [PayloadUnit("m")]
    public float Wingspan;

    [PayloadKey("emptyWeight")]
    [PayloadUnit("kg")]
    public float EmptyWeight;

    [PayloadKey("mtow")]
    [PayloadUnit("kg")]
    public float MTOW;

    [PayloadKey("fuelCapacity")]
    [PayloadUnit("L")]
    public float FuelCapacity;
}