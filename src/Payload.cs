using System.IO;
using System.Collections.Generic;
using System.Numerics;

namespace PayloadSharp;

public class PayloadFile
{
// private
    private Dictionary<string, string> strings;
    private Dictionary<string, int> integers;
    private Dictionary<string, float> floats;
    private Dictionary<string, bool> booleans;
    private Dictionary<string, byte> bytes;
    private Dictionary<string, Vector2> vector2s;
    private Dictionary<string, Vector3> vector3s;
    private Dictionary<string, Vector4> vector4s;
    private Dictionary<string, Quaternion> quats;

// public

    public void LoadFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        foreach(string line in lines)
        {
            
        }
    }
}
