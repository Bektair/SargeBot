using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SargeBot.Features.GameInfo;

public class Point2D_DictionarySerializer<TValue> : JsonConverter<Dictionary<Point2D, TValue>>
{
    private readonly JsonConverter<TValue> _valueConverter;
    private readonly Type _valueType;
    private readonly char delimiter = ',';
    public Point2D_DictionarySerializer()
    {
        // For performance, use the existing converter if available.

        // Cache the key and value types.
        _valueType = typeof(TValue);
    }

    public override Dictionary<Point2D, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }
        var dictionary = new Dictionary<Point2D, TValue>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }
            // Get the key.
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            string? propertyName = reader.GetString();
            string[] xy = propertyName.Split(delimiter);
            
            Point2D key = new (){X= float.Parse(xy[0]), Y= float.Parse(xy[1]) };
            TValue value;
            if (_valueConverter != null)
            {
                reader.Read();
                value = _valueConverter.Read(ref reader, _valueType, options)!;
            }
            else
            {
                value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;
            }
            dictionary.Add(key, value);
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Point2D, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (KeyValuePair<Point2D, TValue> keyValuePair in value)
        {
            string propertyName = keyValuePair.Key.X.ToString() + delimiter + keyValuePair.Key.Y.ToString();
            writer.WritePropertyName(propertyName);

            if (_valueConverter != null)
            {
                _valueConverter.Write(writer, keyValuePair.Value, options);
            }
            else
            {
                JsonSerializer.Serialize(writer, keyValuePair.Value, options);
            }
        }
        writer.WriteEndObject();
    }
}

