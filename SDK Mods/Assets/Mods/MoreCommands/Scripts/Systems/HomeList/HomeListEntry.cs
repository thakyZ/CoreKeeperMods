#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoreCommands.Data;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.HomeList {
  [Serializable]
  public sealed class HomeListEntry : IEquatable<HomeListEntry> {
    [JsonPropertyName("label")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public Vector2 Position { get; set; } = Vector2.zero;

    [JsonPropertyName("direction")]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public Direction Direction { get; set; } = Direction.zero;

    private HomeListEntry(string label, Vector2 position, Direction direction) {
      this.Label = label;
      this.Position = position;
      this.Direction = direction;
    }

    private HomeListEntry(string label) {
      Label = label;
      Position = Vector2.zero;
      Direction = Direction.zero;
    }

    public static HomeListEntry Create(string label, Vector2 position, Direction direction) {
      return new HomeListEntry(label, position, direction);
    }

    public static HomeListEntry Create(string label) {
      return new HomeListEntry(label);
    }

    [JsonConstructor]
    public HomeListEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    public static bool Equals(HomeListEntry x, HomeListEntry y) {
      return x.Equals(other: y);
    }

    public bool Equals(HomeListEntry other)
      => other.Label.Equals(this.Label, StringComparison.Ordinal);

    public override bool Equals(object? obj)
      => obj is HomeListEntry other && Equals(other: other);

    public static int GetHashCode(HomeListEntry obj) {
      return HashCode.Combine(obj.Label.GetHashCode());
    }

    public override int GetHashCode() {
      return HashCode.Combine(this.Label.GetHashCode());
    }
  }
}
