#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoreCommands.Data;
using Unity.Mathematics;

namespace MoreCommands.Systems.HomeList
{
  [Serializable]
  public sealed class HomeListEntry : IEquatable<HomeListEntry> {
    [JsonPropertyName("label")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public float2 Position { get; set; } = float2.zero;

    [JsonPropertyName("direction")]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public Direction Direction { get; set; } = Direction.zero;

    public HomeListEntry(string label, float2 position, Direction direction) {
      this.Label = label;
      this.Position = position;
      this.Direction = direction;
    }

    public HomeListEntry(string label) {
      Label = label;
      Position = float2.zero;
      Direction = Direction.zero;
    }

    [JsonConstructor()]
    public HomeListEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    public static bool Equals(HomeListEntry x, HomeListEntry y) {
      return x.Equals(other: y);
    }

    public bool Equals(HomeListEntry other) {
      return other.Label.Equals(this.Label, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) {
      if (obj is null) return false;
      if (obj is HomeListEntry other) return Equals(other: other);
      return false;
    }

    public static int GetHashCode(HomeListEntry obj) {
      return HashCode.Combine(obj.Label.GetHashCode());
    }

    public override int GetHashCode() {
      return HashCode.Combine(this.Label.GetHashCode());
    }
  }
}
