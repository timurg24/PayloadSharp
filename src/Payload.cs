using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

// TODO: Add syntax error handling

namespace PayloadSharp;

public class Payload
{
    public string ObjectName;
    public string SchemaName;
    public string StructSchemaName;

    Dictionary<string, PayloadEntry> data = new();
    Dictionary<string, FieldInfo> schemaData = new();

    bool insideBrackets;
    bool endOfFile;

    public T LoadFile<T>(string path)
    {
        StructSchemaName = GetSchemaName(typeof(T));
        string[] lines = File.ReadAllLines(path);

        lines = ClearComments(lines);
        lines = GetHeader(lines);

        if(SchemaName != StructSchemaName)
            throw new Exception("Attempted to read a Payload with an incorrect schema");

        foreach(string line in lines)
        {
            ParseLine(line);
        }

        return (T)GetSchemaData(typeof(T));
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
        entry.value = entry.value.Replace("\"", "");

        if (parts.Length > 4)
        {
            entry.unit = parts[4];
        }

        data.Add(entry.key, entry);
    }

    string GetSchemaName(Type type)
    {
        PayloadSchemaAttribute? schema =
            type.GetCustomAttribute<PayloadSchemaAttribute>();

        return schema?.Name ?? "";
    }

    // Gets all of the schema entries and gets their respective data from the Payload
    object GetSchemaData(Type type)
    {
        object instance = Activator.CreateInstance(type)!;
        // Go through every schema entry
        foreach(FieldInfo field in type.GetFields())
        {
            var attribute = field.GetCustomAttribute<PayloadKeyAttribute>();
            
            if(attribute == null) continue;

            var range = field.GetCustomAttribute<PayloadRangeAttribute>();
            var unit = field.GetCustomAttribute<PayloadUnitAttribute>();
             
            if(data.TryGetValue(attribute.Name, out PayloadEntry entry))
            {
                object value = Convert.ChangeType(
                    entry.value,
                    field.FieldType,
                    System.Globalization.CultureInfo.InvariantCulture // not suprised that a bill gates product would be talking about globalization and cultures...
                );

                // RANGE CHECK
                if (range != null &&
                    (field.FieldType == typeof(int) ||
                    field.FieldType == typeof(float) ||
                    field.FieldType == typeof(double) ||
                    field.FieldType == typeof(long) ||
                    field.FieldType == typeof(decimal)))
                {
                    double number = Convert.ToDouble(value);

                    if (number > range.Max || number < range.Min)
                    {
                        throw new ArgumentOutOfRangeException(
                            field.Name,
                            $"Value {number} must be between {range.Min} and {range.Max}."
                        );
                    }
                }



                field.SetValue(instance, value);
            }
        }
        return instance;
    }
}