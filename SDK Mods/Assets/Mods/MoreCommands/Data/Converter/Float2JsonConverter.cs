#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unity.Mathematics;

namespace MoreCommands.Data.Converter {
  public class Float2JsonConverter : JsonConverterFactory {
    public override bool CanConvert(Type typeToConvert)
    {
      return typeToConvert.IsEquivalentTo(typeof(float2));
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
      return new Float2JsonConverterInner(options);
    }

    public class Float2JsonConverterInner : JsonConverter<float2>
    {
      private readonly JsonConverter<float> _valueConverter;

      public Float2JsonConverterInner(JsonSerializerOptions options)
      {
        _valueConverter = (JsonConverter<float>)options.GetConverter(typeof(float));
      }

      public override float2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
          if (reader.TokenType != JsonTokenType.StartObject) {
            throw new JsonException();
          }

          float2 output = float2.zero;

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
            }
        }

        throw new JsonException();
      }

      public override void Write(Utf8JsonWriter writer, float2 value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        _valueConverter.Write(writer, value.x, options);
        writer.WritePropertyName("y");
        _valueConverter.Write(writer, value.y, options);
        writer.WriteEndObject();
      }
    }
  }
}
