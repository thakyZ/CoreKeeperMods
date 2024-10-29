#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Data.Converter {
  public class Vector2JsonConverter : JsonConverterFactory {
    public override bool CanConvert(Type typeToConvert) {
      return typeToConvert.IsEquivalentTo(typeof(Vector2));
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
      return new Vector2JsonConverterInner(options);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public class Vector2JsonConverterInner : JsonConverter<Vector2> {
      // ReSharper disable once InconsistentNaming
      private readonly JsonConverter<float> _valueConverter;

      public Vector2JsonConverterInner(JsonSerializerOptions options) {
        _valueConverter = (JsonConverter<float>)options.GetConverter(typeof(float));
      }

      public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
          throw new JsonException();
        }

        Vector2 output = Vector2.zero;

        while (reader.Read()) {
          if (reader.TokenType == JsonTokenType.EndObject) {
            return output;
          }

          if (reader.TokenType != JsonTokenType.PropertyName) {
            throw new JsonException();
          }

          string? propertyName = reader.GetString();

          // ReSharper disable once InvertIf
          if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrWhiteSpace(propertyName)) {
            if (propertyName.Equals("x", StringComparison.OrdinalIgnoreCase)) {
              reader.Read();
              float value = _valueConverter.Read(ref reader, typeof(float), options);
              output.x = value;
            }

            // ReSharper disable once InvertIf
            if (propertyName.Equals("y", StringComparison.OrdinalIgnoreCase)) {
              reader.Read();
              float value = _valueConverter.Read(ref reader, typeof(float), options);
              output.y = value;
            }
          }
        }

        return output;
      }

      public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options) {
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
