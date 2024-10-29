#nullable enable
using System;
using System.Collections.Generic;
#if IDE
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Text.Json.Serialization;
using MoreCommands.Data;
using UnityEngine;
using JsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.Death {
  [Serializable]
  public sealed class DeathWorldEntry : IEquatable<DeathWorldEntry> {
    [JsonPropertyName("world_name")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string WorldName { get; } = string.Empty;

    [JsonPropertyName("player_entries")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public List<DeathPlayerEntry> PlayerEntries { get; set; } = new List<DeathPlayerEntry>();

    [JsonConstructor]
    public DeathWorldEntry(string worldName, List<DeathPlayerEntry>? playerEntries) {
      this.WorldName = worldName;
      this.PlayerEntries = playerEntries ?? new List<DeathPlayerEntry>();
    }

    public DeathWorldEntry() { }

    public override string ToString()
      => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    internal DeathPlayerEntry AddPlayer(PlayerController pc) {
      if (PlayerEntries.Exists(x => x.PlayerName == pc.playerName)) {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }

      var playerEntry = DeathPlayerEntry.Create(pc, null);

      this.PlayerEntries.Add(playerEntry);
      return playerEntry;
    }

    public DeathPlayerEntry AddPlayerOnDeath(PlayerController pc, Vector3 position, Direction direction) {
      var playerEntry = DeathPlayerEntry.CreateOnDeath(pc, position, direction);

      this.PlayerEntries.Add(playerEntry);
      return playerEntry;
    }

#if IDE
    public bool TryGetPlayer(string playerName, [NotNullWhen(true)] out DeathPlayerEntry? result) {
#else
    public bool TryGetPlayer(string playerName, out DeathPlayerEntry result) {
#endif
      result = PlayerEntries.Find(x => x.PlayerName.Equals(playerName, StringComparison.Ordinal));
      return result is not null;
    }

    public bool Equals(DeathWorldEntry? other)
      => other is not null && this.WorldName.Equals(other.WorldName, StringComparison.Ordinal);

    public override bool Equals(object? obj)
      => obj is DeathWorldEntry other && this.Equals(other: other);

    public override int GetHashCode()
      => this.WorldName.GetHashCode();
  }
}
