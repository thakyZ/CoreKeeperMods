using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using MoreCommands.Util;

using PugMod;

using Unity.Entities;

using UnityEngine;

namespace MoreCommands.Systems
{
  [Serializable()]
  public class HouseEntry
  {
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

    public HouseEntry(string label)
    {
      Label = label;
      Position = Vector3.zero;
      Direction = Direction.zero;
    }

    [JsonConstructor]
    public HouseEntry(string label, Vector3 position, Direction direction)
    {
      this.Label = label;
      this.Position = position;
      this.Direction = direction;
    }

    public static bool Equals(HouseEntry x, HouseEntry y)
    {
      return x.Label == y.Label;
    }

    public bool Equals(HouseEntry obj)
    {
      return obj.Label == this.Label && obj.Position == this.Position && obj.Direction == this.Direction;
    }

    public new bool Equals(object obj)
    {
      if (obj.GetType() == typeof(HouseEntry))
      {
        return Equals((HouseEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(HouseEntry obj)
    {
      return HashCode.Combine(obj.Label.GetHashCode(), obj.Position.GetHashCode(), obj.Direction.GetHashCode());
    }

    public new int GetHashCode()
    {
      return HashCode.Combine(this.Label.GetHashCode(), this.Position.GetHashCode(), this.Direction.GetHashCode());
    }
  }

  [Serializable()]
  public class HousingPlayerEntry
  {
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
    public List<HouseEntry> ListOfHouses { get; } = new();

    [JsonConstructor()]
    public HousingPlayerEntry(string player_uuid, string player_name, List<HouseEntry> list_of_houses)
    {
      this.PlayerUuid = player_uuid;
      this.PlayerName = player_name;
      this.ListOfHouses = list_of_houses == null ? new() : list_of_houses;
    }

    public HousingPlayerEntry(PlayerController player)
    {
      PlayerName = player.playerName;
      PlayerUuid = "";
      this.ListOfHouses = new();
      this.ListOfHouses.AddHousingEntry(player);
    }

    public void RemoveHouse(HouseEntry houseEntry)
    {
      foreach ((int index, HouseEntry house) in this.ListOfHouses.Select((value, index) => (index, value)))
      {
        if (houseEntry.Equals(house))
        {
          this.ListOfHouses.RemoveAt(index);
        }
      }
    }

    public void RemoveHouse(string label)
    {
      foreach ((int index, HouseEntry house) in this.ListOfHouses.Select((value, index) => (index, value)))
      {
        if (house.Label == label)
        {
          this.ListOfHouses.RemoveAt(index);
        }
      }
    }

    public static bool Equals(HousingPlayerEntry x, HousingPlayerEntry y)
    {
      return x.PlayerName == y.PlayerName && x.PlayerUuid == x.PlayerUuid && x.ListOfHouses.Equals(y.ListOfHouses);
    }

    public bool Equals(HousingPlayerEntry obj)
    {
      return obj.PlayerName == this.PlayerName && obj.PlayerUuid == this.PlayerUuid && obj.ListOfHouses.Equals(this.ListOfHouses);
    }

    public new bool Equals(object obj)
    {
      if (obj.GetType() == typeof(HousingPlayerEntry))
      {
        return Equals((HousingPlayerEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(HousingPlayerEntry obj)
    {
      return HashCode.Combine(obj.PlayerName.GetHashCode(), obj.PlayerUuid.GetHashCode(), obj.ListOfHouses.GetHashCode());
    }

    public new int GetHashCode()
    {
      return HashCode.Combine(this.PlayerName.GetHashCode(), this.PlayerUuid.GetHashCode(), this.ListOfHouses.GetHashCode());
    }
  }

  public class HousingSystem
  {
    [JsonPropertyName("player_entries")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    private List<HousingPlayerEntry> PlayerEntries { get; } = new();

    [JsonConstructor()]
    public HousingSystem(List<HousingPlayerEntry> player_entries)
    {
      if (player_entries == null)
      {
        this.PlayerEntries = new();
      } else
      {
        this.PlayerEntries = player_entries;
      }
    }

    public HousingSystem() { }

    internal HousingPlayerEntry AddEntry(string label, PlayerController pc)
    {
      if (PlayerEntries.Any(x => x.PlayerName == pc.playerName))
      {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }

      return PlayerEntries.AddEntry(label, pc);
    }

    public bool TryGetPlayerEntry(string playerName, out HousingPlayerEntry housingPlayerEntry)
    {
      foreach (var entry in PlayerEntries)
      {
        if (entry.PlayerName == playerName)
        {
          housingPlayerEntry = entry;
          return true;
        }
      }
      housingPlayerEntry = null;
      return false;
    }
  }

  public static class HousingSystemExtensions
  {
    public static void Init(this Dictionary<string, HousingSystem> dictionary)
    {
      foreach (var (key, value) in dictionary)
      {
        if (value == null)
        {
          dictionary[key] = new();
        }
      }
    }

    public static HouseEntry AddHousingEntry(this List<HouseEntry> entries, string label, PlayerController player)
    {
      var housingEntry = new HouseEntry(label)
      {
        Position = player.WorldPosition,
        Direction = player.facingDirection
      };

      entries.Add(housingEntry);
      return housingEntry;
    }

    public static HouseEntry AddHousingEntry(this List<HouseEntry> entries, PlayerController player)
    {
      var housingEntry = new HouseEntry("bed");
      if (API.Server.World.EntityManager.HasComponent<PlayerClaimedBed>(player.entity))
      {
        var claimedBed = API.Server.World.EntityManager.GetComponentData<PlayerClaimedBed>(player.entity);
        housingEntry.Position = claimedBed.position;
        housingEntry.Direction = API.Server.World.EntityManager.GetComponentObject<Bed>(claimedBed.claimedBedEntity).rotationIndex.ToDirection();
      } else
      {
        housingEntry.Position = PlayerController.PLAYER_SPAWN_POSITION;
        housingEntry.Direction = Direction.forward;
      }

      entries.Add(housingEntry);
      return housingEntry;
    }

    public static HousingPlayerEntry AddEntry(this List<HousingPlayerEntry> entries, string label, PlayerController player)
    {
      var playerEntry = new HousingPlayerEntry(player);

      entries.Add(playerEntry);
      return playerEntry;
    }

    public static HousingSystem GetDeathSystem(this Dictionary<string, HousingSystem> dictionary, World world)
    {
      if (dictionary.TryGetValue(world.Name, out var deathSystem))
      {
        return deathSystem;
      }

      dictionary.Add(world.Name, new HousingSystem());
      return dictionary[world.Name];
    }

    public static HousingPlayerEntry GetDeathPlayerEntry(this Dictionary<string, HousingSystem> dictionary, string label, World world, PlayerController pc)
    {
      var housingSystem = dictionary.GetDeathSystem(world);

      if (housingSystem.TryGetPlayerEntry(pc.playerName, out var housingPlayerEntry))
      {
        return housingPlayerEntry;
      }

      return housingSystem.AddEntry(label, pc);
    }
  }
}
