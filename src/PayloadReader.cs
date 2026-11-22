using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

// TODO: Add syntax error handling

namespace PayloadSharp;

public class PayloadReader
{
    public string ObjectName;
    public string SchemaName;
    public string StructSchemaName;

    Dictionary<string, PayloadEntry> data = new();

    public T LoadFile<T>(string path)
    {
        data.Clear();
        ObjectName = "";
        SchemaName = "";
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
        if (lines.Length == 0)
            throw new FormatException("Payload file is empty.");

        string[] header = lines[0].Split(':', 2);

        if (header.Length != 2 ||
            string.IsNullOrWhiteSpace(header[0]) ||
            string.IsNullOrWhiteSpace(header[1]))
        {
            throw new FormatException("Invalid Payload header.");
        }

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
        line = line.Trim();

        if (line == "{" || line == "}")
            return;

        string[] sides = line.Split('=', 2, StringSplitOptions.TrimEntries);
        string[] parts = sides[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (sides.Length != 2 || parts.Length != 2 ||
            string.IsNullOrWhiteSpace(sides[1]))
        {
            throw new FormatException($"Invalid entry: {line}");
        }

        PayloadEntry entry = new();

        // Your existing type switch
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

        if (entry.type == typeof(string))
        {
            entry.value = sides[1].Trim().Trim('"');
        }
        else
        {
            string[] valueParts = sides[1].Split(
                ' ', 2, StringSplitOptions.RemoveEmptyEntries
            );

            entry.value = valueParts[0];

            if (valueParts.Length > 1)
                entry.unit = valueParts[1];
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

                // TYPE CHECK
                if (entry.type != field.FieldType)
                {
                    throw new InvalidOperationException(
                        $" ** Type mismatch for '{entry.key}'. " +
                        $"Schema expects '{field.FieldType.Name}', " +
                        $"but Payload contains '{entry.type.Name}'. **"
                    );
                }

                // UNIT CHECK
                if(unit != null)
                {
                    if(unit.Unit != entry.unit)
                        throw new Exception($" ** Units are not matching for entry: {entry.key}. (Schema: {unit.Unit}, Payload: {entry.unit}) **");
                }

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