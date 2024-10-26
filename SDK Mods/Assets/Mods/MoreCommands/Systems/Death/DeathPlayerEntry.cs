#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using MoreCommands.Data;

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
    public List<DeathEntry> DeathPositions { get; } = new List<DeathEntry>();
    
    [JsonConstructor]
    private DeathPlayerEntry(string playerUuid, string playerName, List<DeathEntry>? deathPositions) {
      this.PlayerUuid = playerUuid;
      this.PlayerName = playerName;

      this.DeathPositions = deathPositions ?? new List<DeathEntry>();
    }
    
    private DeathPlayerEntry(PlayerController player) {
      this.PlayerName = player.playerName;
      this.DeathPositions.AddDeathEntry(player);
    }

    public DeathPlayerEntry() { }

    public static DeathPlayerEntry Create(string playerUuid, string playerName, List<DeathEntry>? deathPositions)
      => new DeathPlayerEntry(playerUuid, playerName, deathPositions);

    public static DeathPlayerEntry CreateOnDeath(PlayerController player)
      => new DeathPlayerEntry(player);

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
