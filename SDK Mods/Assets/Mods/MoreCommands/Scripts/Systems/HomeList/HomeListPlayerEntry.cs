#nullable enable
using System;
using System.Collections.Generic;
#if IDE
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoreCommands.Data;
using NekoBoiNick.CoreKeeper.Common.Util;
using PugMod;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.HomeList {
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
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
    public List<HomeListEntry> ListOfHouses { get; set; } = new List<HomeListEntry>();

    public HomeListPlayerEntry(string playerUuid, string playerName, List<HomeListEntry>? listOfHouses) {
      this.PlayerUuid = playerUuid;
      this.PlayerName = playerName;
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

    public bool HasBed() => this.ListOfHouses.Exists(x => x.Label == "bed");

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

      var homeEntry = HomeListEntry.Create(label, pc.WorldPosition.ToFloat2(), pc.facingDirection);

      this.ListOfHouses.Add(homeEntry);
      return homeEntry;
    }

    public HomeListEntry AddBed(PlayerController pc, Vector2? position = null, Direction? direction = null) {
      var homeEntry = HomeListEntry.Create("bed");

      if (API.Server.World.EntityManager.HasComponent<PlayerClaimedBed>(pc.entity)) {
        var claimedBed = API.Server.World.EntityManager.GetComponentData<PlayerClaimedBed>(pc.entity);
        homeEntry.Position = claimedBed.position;
        homeEntry.Direction = API.Server.World.EntityManager.GetComponentObject<Bed>(claimedBed.claimedBedEntity)
          .rotationIndex.ToDirection();
      } else if (position.HasValue && direction.HasValue) {
        homeEntry.Position = position.Value;
        homeEntry.Direction = direction.Value;
      } else {
        homeEntry.Position = float2.zero;
        homeEntry.Direction = Direction.forward;
      }

      this.ListOfHouses.Add(homeEntry);
      return homeEntry;
    }

    public HomeListEntry SetBed(PlayerController pc, Vector2 position, Direction direction) {
      if (!this.HasBed()) {
        return AddBed(pc, position, direction);
      }

      HomeListEntry? output = null;
      foreach (var homeListEntry in this.ListOfHouses.Where(x => x.Label == "bed")) {
        homeListEntry.Position = position;
        homeListEntry.Direction = direction;
        output = homeListEntry;
      }

      return output ?? AddBed(pc, position, direction);
    }

    public HomeListEntry GetBed(PlayerController pc) {
      var output = this.ListOfHouses.FirstOrDefault(x => x.Label == "bed");
      if (output is null) {
        return AddBed(pc);
      }

      return output;
    }

    public HomeListEntry GetHouse(string label, PlayerController pc) {
      var output = this.ListOfHouses.FirstOrDefault(x => x.Label == label);
      if (output is null) {
        return AddHouse(label, pc);
      }

      return output;
    }

    public HomeListEntry SetHouse(string label, PlayerController pc, Vector2 position, Direction direction) {
      if (!this.HasHouse(label)) {
        return AddHouse(label, pc);
      }

      HomeListEntry? output = null;
      foreach (var homeListEntry in this.ListOfHouses.Where(x => x.Label == label)) {
        homeListEntry.Position = position;
        homeListEntry.Direction = direction;
        output = homeListEntry;
      }

      return output ?? AddHouse(label, pc);
    }

    // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Global
#if IDE
    public bool TryGetHouse(string label, [NotNullWhen(true)] out HomeListEntry? result) {
#else
    public bool TryGetHouse(string label, out HomeListEntry result) {
#endif
      result = this.ListOfHouses.FirstOrDefault(x => x.Label == label);
      return result is not null;
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
