using PayloadSharp;

[PayloadSchema("Aircraft")]
public struct AircraftData
{
    [PayloadKey("name")]
    public string Name;
}