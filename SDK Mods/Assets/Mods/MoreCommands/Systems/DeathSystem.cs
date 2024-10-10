#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unity.Mathematics;
using MoreCommands.Data;
using NekoBoiNick.CoreKeeper.Common.Util;

namespace MoreCommands.Systems {
  [Serializable]
  public sealed class DeathEntry : IEquatable<DeathEntry> {
    [JsonPropertyName("position")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public float3 Position { get; set; } = float3.zero;

    [JsonPropertyName("direction")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public Direction Direction { get; set; } = Direction.forward;

    [JsonConstructor]
    public DeathEntry(float3 position, Direction direction) {
      Position = position;
      Direction = direction;
    }

    public DeathEntry(float3 position, Direction direction, List<DeathEntry> parent) {
      Position = position;
      Direction = direction;
    }

    [JsonConstructor]
    public DeathEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    public static bool Equals(DeathEntry x, DeathEntry y) {
      return x.Equals(other: y);
    }

    public bool Equals(DeathEntry? other) {
      if (other is null) return false;
      return this.Position.Equals(other: other.Position) && this.Direction == other.Direction;
    }

    public override bool Equals(object? obj) {
      if (obj is null) return false;
      if (obj is DeathEntry other) return this.Equals(other: other);
      return false;
    }

    public static int GetHashCode(DeathEntry obj) {
      return HashCode.Combine(obj.Direction.GetHashCode(), obj.Position.GetHashCode());
    }

    public override int GetHashCode() {
      return HashCode.Combine(this.Direction.GetHashCode(), this.Position.GetHashCode());
    }
  }

  [Serializable]
  public sealed class DeathPlayerEntry : IEquatable<DeathPlayerEntry> {
    [JsonPropertyName("player_uuid")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string PlayerUuid { get; set; } = string.Empty;

    [JsonPropertyName("player_name")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("death_positions")]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public List<DeathEntry> DeathPositions { get; set; } = new();

    public DeathPlayerEntry(string player_uuid, string player_name, List<DeathEntry> death_positions) {
      this.PlayerUuid = player_uuid;
      this.PlayerName = player_name;

      if (death_positions == null) {
        this.DeathPositions = new();
      } else {
        this.DeathPositions = death_positions;
      }
    }

    public DeathPlayerEntry(PlayerController player) {
      PlayerName = player.playerName;
      if (this.DeathPositions == null) {
        this.DeathPositions = new();
      }
      this.DeathPositions.AddDeathEntry(player);
    }

    [JsonConstructor()]
    public DeathPlayerEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    public static bool Equals(DeathPlayerEntry x, DeathPlayerEntry y) {
      return x.Equals(other: y);
    }

    public bool Equals(DeathPlayerEntry? other) {
      if (other is null) return false;
      return this.PlayerName.Equals(other.PlayerName, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) {
      if (obj is null) return false;
      if (obj is DeathPlayerEntry other) return this.Equals(other: other);
      return false;
    }

    public static int GetHashCode(DeathPlayerEntry obj) {
      return obj.PlayerName.GetHashCode();
    }

    public override int GetHashCode() {
      return this.PlayerName.GetHashCode();
    }
  }

  [Serializable]
  public sealed class DeathWorldEntry : IEquatable<DeathWorldEntry> {
    [JsonPropertyName("world_name")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string WorldName { get; set; } = string.Empty;

    [JsonPropertyName("player_entries")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public List<DeathPlayerEntry> PlayerEntries { get; set; } = new();

    public DeathWorldEntry(string world_name, List<DeathPlayerEntry> player_entries) {
      this.WorldName = world_name;
      if (player_entries == null) {
        this.PlayerEntries = new();
      } else {
        this.PlayerEntries = player_entries;
      }
    }

    [JsonConstructor]
    public DeathWorldEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    internal DeathPlayerEntry AddEntry(PlayerController pc) {
      if (PlayerEntries.Exists(x => x.PlayerName == pc.playerName)) {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }

      return PlayerEntries.AddEntry(pc);
    }

    public bool TryGetPlayerEntry(string playerName, [NotNullWhen(true)] out DeathPlayerEntry? deathPlayerEntry) {
      deathPlayerEntry = PlayerEntries.Find(x => x.PlayerName.Equals(playerName, StringComparison.Ordinal));
      return deathPlayerEntry is not null;
    }

    public static bool Equals(DeathWorldEntry x, DeathWorldEntry y) {
      return x.Equals(other: y);
    }

    public bool Equals(DeathWorldEntry? other) {
      if (other is null) return false;
      return this.WorldName.Equals(other.WorldName, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) {
      if (obj is null) return false;
      if (obj is DeathWorldEntry other) return this.Equals(other: other);
      return false;
    }

    public static int GetHashCode(DeathWorldEntry obj) {
      return obj.WorldName.GetHashCode();
    }

    public override int GetHashCode() {
      return this.WorldName.GetHashCode();
    }
  }

  public static class DeathSystemExtensions {
    public static void Init(this List<DeathWorldEntry?> list) {
      foreach ((int key, DeathWorldEntry? value) in list.Select((value, index) => (index, value))) {
        if (value is null) {
          list[key] = new();
        }
      }
    }

    public static DeathEntry AddDeathEntry(this List<DeathEntry> entries, PlayerController player) {
      var deathEntry = new DeathEntry(player.SmoothWorldPosition, player.facingDirection, entries);

      entries.Add(deathEntry);
      return deathEntry;
    }

    public static DeathPlayerEntry AddEntry(this List<DeathPlayerEntry> entries, PlayerController player) {
      var playerEntry = new DeathPlayerEntry(player);

      entries.Add(playerEntry);
      return playerEntry;
    }

    public static DeathWorldEntry AddEntry(this List<DeathWorldEntry?> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var deathWorldEntry)) {
        return deathWorldEntry;
      }
      var temp = new DeathWorldEntry(worldName, new());
      list.Add(temp);
      return list.AddEntry(worldName);
    }

    public static DeathPlayerEntry AddPlayerEntry(this List<DeathWorldEntry?> list, PlayerController pc) {
      return list.GetWorldEntry(pc.world.Name).PlayerEntries.AddEntry(pc);
    }

    public static DeathWorldEntry GetWorldEntry(this List<DeathWorldEntry?> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var deathSystem)) {
        return deathSystem;
      }

      return list.AddEntry(worldName);
    }

    public static DeathPlayerEntry GetPlayerEntry(this List<DeathWorldEntry?> list, string worldName, PlayerController pc) {
      var deathSystem = list.GetWorldEntry(worldName);

      if (deathSystem.TryGetPlayerEntry(pc.playerName, out var deathPlayerEntry)) {
        return deathPlayerEntry;
      }

      return deathSystem.AddEntry(pc);
    }

    public static bool TryGetWorldEntry(this List<DeathWorldEntry?> list, string worldName, [NotNullWhen(true)] out DeathWorldEntry? deathWorldEntry) {
      deathWorldEntry = list.Find(x => x?.WorldName.Equals(worldName, StringComparison.Ordinal) == true);
      return deathWorldEntry is not null;
    }
  }
}
