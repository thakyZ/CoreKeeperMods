#nullable enable
using System;
using System.Text.Json.Serialization;

using MoreCommands.Data;

using UnityEngine;

using JsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.Death {
  [Serializable]
  public sealed class DeathEntry : IEquatable<DeathEntry> {
    [JsonPropertyName("position"), JsonPropertyOrder(1), JsonRequired]
    public Vector3 Position { get; } = Vector3.zero;

    [JsonPropertyName("direction"), JsonPropertyOrder(2), JsonRequired]
    public Direction Direction { get; } = Direction.forward;

    [JsonConstructor]
    private DeathEntry(Vector3 position, Direction direction) {
      Position = position;
      Direction = direction;
    }

    public static DeathEntry Create(Vector3 position, Direction direction)
      => new(position, direction);

    public DeathEntry() { }

    public override string ToString()
      => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    public static bool operator ==(DeathEntry a, object? b)
      => a.Equals(obj: b);

    public static bool operator !=(DeathEntry a, object? b)
      => !(a == b);

    public bool Equals(DeathEntry? other)
      => other is not null && this.Position.Equals(other: other.Position) && this.Direction == other.Direction;

    public override bool Equals(object? obj)
      => obj is DeathEntry other && this.Equals(other: other);

    public override int GetHashCode()
      => HashCode.Combine(this.Direction.GetHashCode(), this.Position.GetHashCode());
  }
}
