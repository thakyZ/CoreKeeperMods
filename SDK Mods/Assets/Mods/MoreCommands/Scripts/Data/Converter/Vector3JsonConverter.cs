using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace MoreCommands.Data.Converter {
  public class Vector3JsonConverter : JsonConverter<Vector3> {
    public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      if (reader.TokenType == JsonTokenType.StartObject) {
        JsonElement element = JsonDocument.ParseValue(ref reader).RootElement;
        return new Vector3(
            element.GetProperty("x").GetSingle(),
            element.GetProperty("y").GetSingle()
        );
      }

      throw new InvalidOperationException("Rect must be an object!");
    }

    public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options) {
      writer.WriteStartObject();

      writer.WriteNumber("x", value.x);
      writer.WriteNumber("y", value.y);

      writer.WriteEndObject();
    }
  }
}
