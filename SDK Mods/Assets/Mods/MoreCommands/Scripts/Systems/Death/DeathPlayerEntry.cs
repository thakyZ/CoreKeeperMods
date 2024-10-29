#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MoreCommands.Data;
using UnityEngine;
using JsonSerializer = System.Text.Json.JsonSerializer;


// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.Death {
  [Serializable]
  public sealed class DeathPlayerEntry : IEquatable<DeathPlayerEntry> {
    [JsonPropertyName("player_uuid")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string PlayerUuid { get; } = string.Empty;

    [JsonPropertyName("player_name")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public string PlayerName { get; } = string.Empty;

    [JsonPropertyName("death_positions")]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public HashSet<DeathEntry> DeathPositions { get; } = new HashSet<DeathEntry>();

    [JsonConstructor]
    private DeathPlayerEntry(string playerUuid, string playerName, HashSet<DeathEntry>? deathPositions) {
      this.PlayerUuid = playerUuid;
      this.PlayerName = playerName;

      this.DeathPositions = deathPositions ?? new HashSet<DeathEntry>();
    }

    private DeathPlayerEntry(PlayerController pc, HashSet<DeathEntry>? deathPositions) {
      this.PlayerName = pc.playerName;
      this.DeathPositions = deathPositions ?? new HashSet<DeathEntry>();
    }

    private DeathPlayerEntry(PlayerController pc, Vector3 position, Direction direction) {
      this.PlayerName = pc.playerName;
      this.AddDeathEntry(position, direction);
    }

    public DeathPlayerEntry() { }

    internal static DeathPlayerEntry Create(string playerUuid, string playerName, HashSet<DeathEntry>? deathPositions)
      => new DeathPlayerEntry(playerUuid, playerName, deathPositions);

    internal static DeathPlayerEntry Create(PlayerController pc, HashSet<DeathEntry>? deathPositions)
      => new DeathPlayerEntry(pc, deathPositions);

    internal static DeathPlayerEntry CreateOnDeath(PlayerController pc, Vector3 position, Direction direction)
      => new DeathPlayerEntry(pc, position, direction);

    internal DeathEntry AddDeathEntry(Vector3 position, Direction direction) {
      var result = DeathEntry.Create(position, direction);
      this.DeathPositions.Add(result);
      return result;
    }

    public override string ToString()
      => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    public bool Equals(DeathPlayerEntry? other)
      => other is not null && this.PlayerName.Equals(other.PlayerName, StringComparison.Ordinal);

    public override bool Equals(object? obj)
      => obj is DeathPlayerEntry other && this.Equals(other: other);

    public override int GetHashCode()
      => this.PlayerName.GetHashCode();
  }
}
