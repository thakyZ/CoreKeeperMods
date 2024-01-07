using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Unity.Entities;

using UnityEngine;

namespace MoreCommands.Systems {
  [Serializable()]
  public class DeathEntry {
    [JsonPropertyName("position")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public Vector3 Position { get; set; } = PlayerController.PLAYER_SPAWN_POSITION;
    [JsonPropertyName("direction")]
    [JsonPropertyOrder(2)]
    [JsonRequired()]
    public Direction Direction { get; set; } = Direction.forward;

    [JsonConstructor()]
    public DeathEntry(Vector3 position, Direction direction) {
      Position = position;
      Direction = direction;
    }

    public DeathEntry(Vector3 position, Direction direction, List<DeathEntry> parent) {
      Position = position;
      Direction = direction;
    }

    public static bool Equals(DeathEntry x, DeathEntry y) {
      return x.Position == y.Position && x.Direction == y.Direction;
    }

    public bool Equals(DeathEntry obj) {
      return obj.Position == this.Position && obj.Direction == this.Direction;
    }

    public new bool Equals(object obj) {
      if (obj.GetType() == typeof(DeathEntry)) {
        return Equals((DeathEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(DeathEntry obj) {
      return HashCode.Combine(obj.Direction.GetHashCode(), obj.Position.GetHashCode());
    }

    public new int GetHashCode() {
      return HashCode.Combine(this.Direction.GetHashCode(), this.Position.GetHashCode());
    }
  }

  [Serializable()]
  public class DeathPlayerEntry {
    [JsonPropertyName("player_uuid")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public string PlayerUuid { get; set; }
    [JsonPropertyName("player_name")]
    [JsonPropertyOrder(2)]
    [JsonRequired()]
    public string PlayerName { get; set; }
    [JsonPropertyName("death_positions")]
    [JsonPropertyOrder(3)]
    [JsonRequired()]
    public List<DeathEntry> DeathPositions { get; } = new();

    [JsonConstructor()]
    public DeathPlayerEntry(string player_uuid, string player_name, List<DeathEntry> death_positions) {
      this.PlayerUuid = player_uuid;
      this.PlayerName = player_name;

      this.DeathPositions = death_positions == null ? new() : death_positions;
    }

    public DeathPlayerEntry(PlayerController player) {
      PlayerName = player.playerName;
    }

    public static bool Equals(DeathPlayerEntry x, DeathPlayerEntry y) {
      return x.PlayerName == y.PlayerName;
    }

    public bool Equals(DeathPlayerEntry obj) {
      return obj.PlayerName == this.PlayerName;
    }

    public new bool Equals(object obj) {
      if (obj.GetType() == typeof(DeathPlayerEntry)) {
        return Equals((DeathPlayerEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(DeathPlayerEntry obj) {
      return obj.PlayerName.GetHashCode();
    }

    public new int GetHashCode() {
      return this.PlayerName.GetHashCode();
    }
  }

  [Serializable()]
  public class DeathSystem {
    [JsonPropertyName("player_entries")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public List<DeathPlayerEntry> PlayerEntries { get; } = new();

    [JsonConstructor()]
    public DeathSystem(List<DeathPlayerEntry> player_entries) {
      this.PlayerEntries = player_entries == null ? new() : player_entries;
    }

    public DeathSystem() { }

    internal DeathPlayerEntry AddEntry(PlayerController pc) {
      if (PlayerEntries.Any(x => x.PlayerName == pc.playerName)) {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }

      return PlayerEntries.AddEntry(pc);
    }

    public bool TryGetPlayerEntry(string playerName, out DeathPlayerEntry deathPlayerEntry) {
      foreach (var entry in PlayerEntries) {
        if (entry.PlayerName == playerName) {
          deathPlayerEntry = entry;
          return true;
        }
      }
      deathPlayerEntry = null;
      return false;
    }
  }

  public static class DeathSystemExtensions {
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

    public static void Init(this Dictionary<string, DeathSystem> dictionary) {
      foreach (var (key, value) in dictionary) {
        if (value == null) {
          dictionary[key] = new();
        }
      }
    }

    public static DeathSystem GetDeathSystem(this Dictionary<string, DeathSystem> dictionary, World world) {
      if (dictionary.TryGetValue(world.Name, out var deathSystem)) {
        return deathSystem;
      }

      dictionary.Add(world.Name, new DeathSystem());
      return dictionary[world.Name];
    }

    public static DeathPlayerEntry GetDeathPlayerEntry(this Dictionary<string, DeathSystem> dictionary, World world, PlayerController pc) {
      var deathSystem = dictionary.GetDeathSystem(world);

      if (deathSystem.TryGetPlayerEntry(pc.playerName, out var deathPlayerEntry)) {
        return deathPlayerEntry;
      }

      return deathSystem.AddEntry(pc);
    }

    public static void AddEntry(this Dictionary<string, DeathSystem> dictionary, PlayerController pc) {
      dictionary.GetDeathSystem(pc.world).PlayerEntries.AddEntry(pc);
    }
  }
}
