#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoreCommands.Data;
using NekoBoiNick.CoreKeeper.Common.Util;
using PugMod;
using Unity.Mathematics;

namespace MoreCommands.Systems.HomeList
{
  [Serializable]
  public sealed class HomeListPlayerEntry : IEquatable<HomeListPlayerEntry> {
    [JsonPropertyName("player_uuid")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string PlayerUuid { get; } = string.Empty;

    [JsonPropertyName("player_name")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("list_of_houses")]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public List<HomeListEntry> ListOfHouses { get; set; } = new List<HomeListEntry>();

    public HomeListPlayerEntry(string playerUuid, string playerName, List<HomeListEntry>? listOfHouses) {
      this.PlayerUuid   = playerUuid;
      this.PlayerName   = playerName;
      this.ListOfHouses = listOfHouses ?? new List<HomeListEntry>();
    }

    public HomeListPlayerEntry(PlayerController player) {
      this.PlayerName = player.playerName;
      this.AddBed(player);
    }

    [JsonConstructor]
    public HomeListPlayerEntry() { }

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);

    public bool HasHouse(string label) => this.ListOfHouses.Exists(x => x.Label == label);

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

    public HomeListEntry AddHouse(string label, PlayerController pc) {
      if (this.HasHouse(label)) {
        return this.ListOfHouses.First(x => x.Label == label);
      }

      var homeEntry = new HomeListEntry(label, pc.WorldPosition.ToFloat2(), pc.facingDirection);

      this.ListOfHouses.Add(homeEntry);
      return homeEntry;
    }

    public HomeListEntry AddBed(PlayerController pc) {
      var homeEntry = new HomeListEntry("bed");

      if (API.Server.World.EntityManager.HasComponent<PlayerClaimedBed>(pc.entity)) {
        var claimedBed = API.Server.World.EntityManager.GetComponentData<PlayerClaimedBed>(pc.entity);
        homeEntry.Position = claimedBed.position;
        homeEntry.Direction = API.Server.World.EntityManager.GetComponentObject<Bed>(claimedBed.claimedBedEntity).rotationIndex.ToDirection();
      } else {
        homeEntry.Position = float2.zero;
        homeEntry.Direction = Direction.forward;
      }

      this.ListOfHouses.Add(homeEntry);
      return homeEntry;
    }

    public HomeListEntry? GetHouse(string label, PlayerController? pc) {
      var output = this.ListOfHouses.FirstOrDefault(x => x.Label == label);
      if (pc is not null && output is null) {
        return AddHouse(label, pc);
      }

      return output;
    }

    public bool Equals(HomeListPlayerEntry? other)
      => other is not null && other.PlayerName.Equals(this.PlayerName, StringComparison.Ordinal) &&
         other.PlayerUuid.Equals(this.PlayerUuid, StringComparison.Ordinal) &&
         other.ListOfHouses.Equals(this.ListOfHouses);

    public override bool Equals(object? obj)
      => obj is HomeListPlayerEntry other && this.Equals(other: other);

    public override int GetHashCode()
      => HashCode.Combine(this.PlayerUuid.GetHashCode());
  }
}
