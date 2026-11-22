using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

// TODO: Add syntax error handling

namespace PayloadSharp;

public class Payload
{
    public string ObjectName;
    public string SchemaName;

    Dictionary<string, PayloadEntry> data = new();

    bool insideBrackets;
    bool endOfFile;

    public T LoadFile<T>(string path)
    {
        string[] lines = File.ReadAllLines(path);

        lines = ClearComments(lines);
        lines = GetHeader(lines);


        foreach(string line in lines)
        {
            ParseLine(line);
        }

        throw new NotImplementedException();
    }

    string[] GetHeader(string[] lines)
    {
        string[] header = lines[0].Split(':');
        ObjectName = header[0].Trim();
        SchemaName = header[1].Trim();
        lines = lines.Skip(1).ToArray();
        return lines;
    }

    string[] ClearComments(string[] lines)
    {
        lines = lines.Where(line => !line.TrimStart().StartsWith("//")).ToArray();
        lines = lines.Select(line => line.Split("//", 2)[0].TrimEnd()).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        return lines;
    }

    void ParseLine(string line)
    {
        // TOOD: MAKE THIS SAFER
        if(line.Trim() == "{") {
            insideBrackets = true;
            return;
        }
        if(line.Trim() == "}") {
            endOfFile = true;
            return;
        }

        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        PayloadEntry entry = new();
        // Get data type
        entry.type = parts[0] switch
        {
            "string" => typeof(string),
            "int" => typeof(int),
            "float" => typeof(float),
            "double" => typeof(double),
            "bool" => typeof(bool),
            "byte" => typeof(byte),
            "Vector2" => typeof(Vector2),
            "Vector3" => typeof(Vector3),
            _ => throw new NotSupportedException($"Unknown type: {parts[0]}")
        };

        entry.key = parts[1];
        entry.value = parts[3];

        Console.WriteLine($"Type: {entry.type}");
        Console.WriteLine($"Key: {entry.key}");
        Console.WriteLine($"Value: {entry.value}");
    }

}