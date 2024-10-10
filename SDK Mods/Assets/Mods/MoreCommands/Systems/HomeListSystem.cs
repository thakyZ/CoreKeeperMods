#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unity.Mathematics;
using PugMod;
using MoreCommands.Data;
using NekoBoiNick.CoreKeeper.Common.Util;

namespace MoreCommands.Systems {
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

  [Serializable]
  public sealed class HomeListPlayerEntry : IEquatable<HomeListPlayerEntry> {
    [JsonPropertyName("player_uuid")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string PlayerUuid { get; set; } = string.Empty;

    [JsonPropertyName("player_name")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("list_of_houses")]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public List<HomeListEntry> ListOfHouses { get; set; } = new();

    public HomeListPlayerEntry(string playerUuid, string playerName, List<HomeListEntry> listOfHouses) {
      this.PlayerUuid = playerUuid;
      this.PlayerName = playerName;

      if (listOfHouses == null) {
        this.ListOfHouses = new();
      } else {
        this.ListOfHouses = listOfHouses;
      }
    }

    public HomeListPlayerEntry(PlayerController player) {
      PlayerName = player.playerName;
      this.ListOfHouses = new();
      this.ListOfHouses.AddDefaultEntry(player);
    }

    [JsonConstructor]
    public HomeListPlayerEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    public void RemoveHouse(HomeListEntry homeListEntry) {
      for (int i = 0; i < this.ListOfHouses.Count; i++) {
        if (homeListEntry.Equals(this.ListOfHouses[i])) {
          this.ListOfHouses.RemoveAt(i);
        }
      }
    }

    public void RemoveHouse(string label) {
      for (int i = 0; i < this.ListOfHouses.Count; i++) {
        if (label.Equals(this.ListOfHouses[i].Label)) {
          this.ListOfHouses.RemoveAt(i);
        }
      }
    }

    public static bool Equals(HomeListPlayerEntry x, HomeListPlayerEntry y) {
      return x.Equals(other: y);
    }

    public bool Equals(HomeListPlayerEntry? other) {
      if (other is null) return false;
      return other.PlayerName.Equals(this.PlayerName, StringComparison.Ordinal) && other.PlayerUuid.Equals(this.PlayerUuid, StringComparison.Ordinal) && other.ListOfHouses.Equals(this.ListOfHouses);
    }

    public override bool Equals(object? obj) {
      if (obj is null) return false;
      if (obj is HomeListPlayerEntry other) return this.Equals(other: other);
      return false;
    }

    public static int GetHashCode(HomeListPlayerEntry obj) {
      return HashCode.Combine(obj.PlayerName.GetHashCode(), obj.PlayerUuid.GetHashCode(), obj.ListOfHouses.GetHashCode());
    }

    public override int GetHashCode() {
      return HashCode.Combine(this.PlayerName.GetHashCode(), this.PlayerUuid.GetHashCode(), this.ListOfHouses.GetHashCode());
    }
  }

  [Serializable]
  public class HomeListWorldEntry {
    [JsonPropertyName("world_name")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string WorldName { get; set; } = string.Empty;

    [JsonPropertyName("player_entries")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public List<HomeListPlayerEntry> PlayerEntries { get; set; } = new();

    public HomeListWorldEntry(string world_name, List<HomeListPlayerEntry>? playerEntries) {
      this.WorldName = world_name;
      if (playerEntries is null) {
        this.PlayerEntries = new();
      } else {
        this.PlayerEntries = playerEntries;
      }
    }

    [JsonConstructor]
    public HomeListWorldEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this);

    internal HomeListPlayerEntry AddEntry(string label, PlayerController pc) {
      if (PlayerEntries.Exists(x => x.PlayerName == pc.playerName)) {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }

      return PlayerEntries.AddEntry(label, pc);
    }

    public bool TryGetPlayerEntry(string playerName, [NotNullWhen(true)] out HomeListPlayerEntry? housingPlayerEntry) {
      housingPlayerEntry = this.PlayerEntries.Find(x => x.PlayerName.Equals(playerName, StringComparison.Ordinal));
      return housingPlayerEntry is not null;
    }
  }

  public static class HomeListSystemExtensions {
    public static void Init(this List<HomeListWorldEntry?> list) {
      foreach ((int key, HomeListWorldEntry? value) in list.Select((value, index) => (index, value))) {
        if (value is null) {
          list[key] = new();
        }
      }
    }

    public static HomeListEntry AddHomeListEntry(this List<HomeListEntry> entries, string label, PlayerController player) {
      var homeEntry = new HomeListEntry(label, player.WorldPosition.ToFloat2(), player.facingDirection);

      entries.Add(homeEntry);
      return homeEntry;
    }

    public static HomeListEntry AddDefaultEntry(this List<HomeListEntry> entries, PlayerController player) {
      var homeEntry = new HomeListEntry("bed");

      if (API.Server.World.EntityManager.HasComponent<PlayerClaimedBed>(player.entity)) {
        var claimedBed = API.Server.World.EntityManager.GetComponentData<PlayerClaimedBed>(player.entity);
        homeEntry.Position = claimedBed.position;
        homeEntry.Direction = API.Server.World.EntityManager.GetComponentObject<Bed>(claimedBed.claimedBedEntity).rotationIndex.ToDirection();
      } else {
        homeEntry.Position = float2.zero;
        homeEntry.Direction = Direction.forward;
      }

      entries.Add(homeEntry);
      return homeEntry;
    }

    public static HomeListPlayerEntry AddEntry(this List<HomeListPlayerEntry> entries, string label, PlayerController player) {
      var playerEntry = new HomeListPlayerEntry(player);

      entries.Add(playerEntry);
      return playerEntry;
    }

    public static HomeListWorldEntry AddEntry(this List<HomeListWorldEntry?> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var homeWorldEntry)) {
        return homeWorldEntry;
      }

      var temp = new HomeListWorldEntry(worldName, new());
      list.Add(temp);
      return list.AddEntry(worldName);
    }

    public static HomeListPlayerEntry AddPlayerEntry(this List<HomeListWorldEntry?> list, string label, PlayerController pc) {
      return list.GetWorldEntry(pc.world.Name).PlayerEntries.AddEntry(label, pc);
    }

    public static HomeListWorldEntry GetWorldEntry(this List<HomeListWorldEntry?> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var housingSystem)) {
        return housingSystem;
      }

      return list.AddEntry(worldName);
    }

    public static HomeListPlayerEntry GetPlayerEntry(this List<HomeListWorldEntry?> list, string label, string worldName, PlayerController pc) {
      var housingSystem = list.GetWorldEntry(worldName);

      if (housingSystem.TryGetPlayerEntry(pc.playerName, out var housingPlayerEntry)) {
        return housingPlayerEntry;
      }

      return housingSystem.AddEntry(label, pc);
    }

    public static bool TryGetWorldEntry(this List<HomeListWorldEntry?> list, string worldName, [NotNullWhen(true)] out HomeListWorldEntry? homeWorldEntry) {
      homeWorldEntry = list.Find(x => x?.WorldName.Equals(worldName, StringComparison.Ordinal) == true);
      return homeWorldEntry is not null;
    }
  }
}
