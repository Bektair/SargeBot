using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SargeBot.Features.GameData;
public class uintDictJsonConverter<TKey, TValue> : JsonConverter<Dictionary<uint, TValue>>
{
    public override Dictionary<uint, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var value = new Dictionary<uint, TValue>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return value;
            }

            string keyString = reader.GetString();

            if (!uint.TryParse(keyString, out uint keyAsInt32))
            {
                throw new JsonException($"Unable to convert \"{keyString}\" to System.Int32.");
            }

            TValue itemValue;
            //Do I have to make a custom serializer for the values?
            itemValue = JsonSerializer.Deserialize<TValue>(ref reader, options)!;

            value.Add(keyAsInt32, itemValue);
        }
        throw new JsonException("Error Occured");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<uint, TValue> dictionary, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (KeyValuePair<uint, TValue> item in dictionary)
        {
            var property = item.Key;
            writer.WritePropertyName(property.ToString());

            JsonSerializer.Serialize(writer, item.Value, options);
        }

        writer.WriteEndObject();
    }

}

