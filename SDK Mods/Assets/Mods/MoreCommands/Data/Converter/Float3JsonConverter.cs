#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unity.Mathematics;

namespace MoreCommands.Data.Converter {
  public class Float3JsonConverter : JsonConverterFactory {
    public override bool CanConvert(Type typeToConvert)
    {
      return typeToConvert.IsEquivalentTo(typeof(float3));
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
      return new Float3JsonConverterInner(options);
    }

    public class Float3JsonConverterInner : JsonConverter<float3>
    {
      private readonly JsonConverter<float> _valueConverter;

      public Float3JsonConverterInner(JsonSerializerOptions options)
      {
        _valueConverter = (JsonConverter<float>)options.GetConverter(typeof(float));
      }

      public override float3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
          if (reader.TokenType != JsonTokenType.StartObject)
          {
            throw new JsonException();
          }

          float3 output = float3.zero;
          while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndObject) {
              return output;
            }

            if (reader.TokenType != JsonTokenType.PropertyName) {
              throw new JsonException();
            }

            string? propertyName = reader.GetString();

            if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrWhiteSpace(propertyName)) {
              if (propertyName.Equals("x", StringComparison.OrdinalIgnoreCase)) {
                reader.Read();
                float @value = _valueConverter.Read(ref reader, typeof(float), options)!;
                output.x = @value;
              }

              if (propertyName.Equals("y", StringComparison.OrdinalIgnoreCase)) {
                reader.Read();
                float @value = _valueConverter.Read(ref reader, typeof(float), options)!;
                output.y = @value;
              }

              if (propertyName.Equals("z", StringComparison.OrdinalIgnoreCase)) {
                reader.Read();
                float @value = _valueConverter.Read(ref reader, typeof(float), options)!;
                output.z = @value;
              }
            }
        }

        throw new JsonException();
      }

      public override void Write(Utf8JsonWriter writer, float3 value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        _valueConverter.Write(writer, value.x, options);
        writer.WritePropertyName("y");
        _valueConverter.Write(writer, value.y, options);
        writer.WritePropertyName("z");
        _valueConverter.Write(writer, value.z, options);
        writer.WriteEndObject();
      }
    }
  }
}
