using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using MoreCommands.Util;

using PugMod;

using UnityEngine;

namespace MoreCommands.Systems {
  [Serializable()]
  public class HomeListEntry {
    [JsonPropertyName("label")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public string Label { get; set; }

    [JsonPropertyName("position")]
    [JsonPropertyOrder(2)]
    [JsonRequired()]
    public Vector3 Position { get; set; }

    [JsonPropertyName("direction")]
    [JsonPropertyOrder(3)]
    [JsonRequired()]
    public Direction Direction { get; set; }

    public HomeListEntry(string label, Vector3 position, Direction direction) {
      this.Label = label;
      this.Position = position;
      this.Direction = direction;
    }

    public HomeListEntry(string label) {
      Label = label;
      Position = Vector3.zero;
      Direction = Direction.zero;
    }

    [JsonConstructor()]
    public HomeListEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this);

    public static bool Equals(HomeListEntry x, HomeListEntry y) {
      return x.Label == y.Label;
    }

    public bool Equals(HomeListEntry obj) {
      return obj.Label == this.Label;
    }

    public new bool Equals(object obj) {
      if (obj.GetType() == typeof(HomeListEntry)) {
        return Equals((HomeListEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(HomeListEntry obj) {
      return HashCode.Combine(obj.Label.GetHashCode());
    }

    public new int GetHashCode() {
      return HashCode.Combine(this.Label.GetHashCode());
    }
  }

  [Serializable()]
  public class HomeListPlayerEntry {
    [JsonPropertyName("player_uuid")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public string PlayerUuid { get; set; }

    [JsonPropertyName("player_name")]
    [JsonPropertyOrder(2)]
    [JsonRequired()]
    public string PlayerName { get; set; }

    [JsonPropertyName("list_of_houses")]
    [JsonPropertyOrder(3)]
    [JsonRequired()]
    public List<HomeListEntry> ListOfHouses { get; set; } = new();

    public HomeListPlayerEntry(string player_uuid, string player_name, List<HomeListEntry> list_of_houses) {
      this.PlayerUuid = player_uuid;
      this.PlayerName = player_name;

      if (list_of_houses == null) {
        this.ListOfHouses = new();
      } else {
        this.ListOfHouses = list_of_houses;
      }
    }

    public HomeListPlayerEntry(PlayerController player) {
      PlayerName = player.playerName;
      PlayerUuid = "";
      this.ListOfHouses = new();
      this.ListOfHouses.AddDefaultEntry(player);
    }

    [JsonConstructor()]
    public HomeListPlayerEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this);

    public void RemoveHouse(HomeListEntry HomeListEntry) {
      foreach ((int index, HomeListEntry house) in this.ListOfHouses.Select((value, index) => (index, value))) {
        if (HomeListEntry.Equals(house)) {
          this.ListOfHouses.RemoveAt(index);
        }
      }
    }

    public void RemoveHouse(string label) {
      foreach ((int index, HomeListEntry house) in this.ListOfHouses.Select((value, index) => (index, value))) {
        if (house.Label == label) {
          this.ListOfHouses.RemoveAt(index);
        }
      }
    }

    public static bool Equals(HomeListPlayerEntry x, HomeListPlayerEntry y) {
      return x.PlayerName == y.PlayerName && x.PlayerUuid == x.PlayerUuid && x.ListOfHouses.Equals(y.ListOfHouses);
    }

    public bool Equals(HomeListPlayerEntry obj) {
      return obj.PlayerName == this.PlayerName && obj.PlayerUuid == this.PlayerUuid && obj.ListOfHouses.Equals(this.ListOfHouses);
    }

    public new bool Equals(object obj) {
      if (obj.GetType() == typeof(HomeListPlayerEntry)) {
        return Equals((HomeListPlayerEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(HomeListPlayerEntry obj) {
      return HashCode.Combine(obj.PlayerName.GetHashCode(), obj.PlayerUuid.GetHashCode(), obj.ListOfHouses.GetHashCode());
    }

    public new int GetHashCode() {
      return HashCode.Combine(this.PlayerName.GetHashCode(), this.PlayerUuid.GetHashCode(), this.ListOfHouses.GetHashCode());
    }
  }

  [Serializable()]
  public class HomeListWorldEntry {
    [JsonPropertyName("world_name")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public string WorldName { get; set; }

    [JsonPropertyName("player_entries")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public List<HomeListPlayerEntry> PlayerEntries { get; set; } = new();

    public HomeListWorldEntry(string world_name, List<HomeListPlayerEntry> player_entries) {
      this.WorldName = world_name;
      if (player_entries == null) {
        this.PlayerEntries = new();
      } else {
        this.PlayerEntries = player_entries;
      }
    }

    [JsonConstructor()]
    public HomeListWorldEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this);

    internal HomeListPlayerEntry AddEntry(string label, PlayerController pc) {
      if (PlayerEntries.Any(x => x.PlayerName == pc.playerName)) {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }

      return PlayerEntries.AddEntry(label, pc);
    }

    public bool TryGetPlayerEntry(string playerName, out HomeListPlayerEntry housingPlayerEntry) {
      foreach (var entry in PlayerEntries) {
        if (entry.PlayerName == playerName) {
          housingPlayerEntry = entry;
          return true;
        }
      }

      housingPlayerEntry = null;
      return false;
    }
  }

  public static class HomeListSystemExtensions {
    public static void Init(this List<HomeListWorldEntry> list) {
      foreach (var (key, value) in list.Select((value, index) => (index, value))) {
        if (value == null) {
          list[key] = new();
        }
      }
    }

    public static HomeListEntry AddHomeListEntry(this List<HomeListEntry> entries, string label, PlayerController player) {
      var homeEntry = new HomeListEntry(label, player.WorldPosition, player.facingDirection);

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
        homeEntry.Position = PlayerController.PLAYER_SPAWN_POSITION;
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

    public static HomeListWorldEntry GetWorldEntry(this List<HomeListWorldEntry> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var housingSystem)) {
        return housingSystem;
      }

      return list.AddEntry(worldName);
    }

    public static HomeListPlayerEntry GetPlayerEntry(this List<HomeListWorldEntry> list, string label, string worldName, PlayerController pc) {
      var housingSystem = list.GetWorldEntry(worldName);

      if (housingSystem.TryGetPlayerEntry(pc.playerName, out var housingPlayerEntry)) {
        return housingPlayerEntry;
      }

      return housingSystem.AddEntry(label, pc);
    }

    public static bool TryGetWorldEntry(this List<HomeListWorldEntry> list, string worldName, out HomeListWorldEntry homeWorldEntry) {
      foreach (var entry in list) {
        if (entry.WorldName == worldName) {
          homeWorldEntry = entry;
          return true;
        }
      }

      homeWorldEntry = null;
      return false;
    }

    public static HomeListWorldEntry AddEntry(this List<HomeListWorldEntry> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var homeWorldEntry)) {
        return homeWorldEntry;
      }

      var temp = new HomeListWorldEntry(worldName, new());
      list.Add(temp);
      return list.AddEntry(worldName);
    }

    public static HomeListPlayerEntry AddPlayerEntry(this List<HomeListWorldEntry> list, string label, PlayerController pc) {
      return list.GetWorldEntry(pc.world.Name).PlayerEntries.AddEntry(label, pc);
    }
  }
}
