#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.HomeList {
  public static class HomeListSystemExtensions {
    public static void Init(this List<HomeListWorldEntry> list) {
      for (int i = 0; i < list.Count; i++) {
        HomeListWorldEntry? value = list[i];
        if (value == null) {
          list[i] = new HomeListWorldEntry();
        }
      }
    }

    #region Home List World Entries

    // ReSharper disable once UnusedMember.Global
    public static HomeListWorldEntry AddWorld(this List<HomeListWorldEntry> list, PlayerController pc) {
      return list.AddWorld(pc.world);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static HomeListWorldEntry AddWorld(this List<HomeListWorldEntry> list, World world) {
      return list.AddWorld(world.Name);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static HomeListWorldEntry AddWorld(this List<HomeListWorldEntry> list, string worldName) {
      if (list.TryGetWorld(worldName, out var homeListWorldEntry)) {
        return homeListWorldEntry;
      }

      var temp = new HomeListWorldEntry(worldName, null);
      list.Add(temp);
      return temp;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static HomeListWorldEntry GetWorld(this List<HomeListWorldEntry> list, PlayerController pc) {
      return list.GetWorld(pc.world.Name);
    }

    // ReSharper disable once UnusedMember.Global
    public static HomeListWorldEntry GetWorld(this List<HomeListWorldEntry> list, World world) {
      return list.GetWorld(world.Name);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static HomeListWorldEntry GetWorld(this List<HomeListWorldEntry> list, string worldName) {
      if (list.TryGetWorld(worldName, out var homeListSystem)) {
        return homeListSystem;
      }

      return list.AddWorld(worldName);
    }

    // ReSharper disable once UnusedMember.Global
    public static bool TryGetWorld(this List<HomeListWorldEntry> list, PlayerController pc,
      [NotNullWhen(true)] out HomeListWorldEntry? homeListWorldEntry) {
      return list.TryGetWorld(pc.world, out homeListWorldEntry);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool TryGetWorld(this List<HomeListWorldEntry> list, World world,
      [NotNullWhen(true)] out HomeListWorldEntry? homeListWorldEntry) {
      return list.TryGetWorld(world.Name, out homeListWorldEntry);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool TryGetWorld(this List<HomeListWorldEntry> list, string worldName,
      [NotNullWhen(true)] out HomeListWorldEntry? homeListWorldEntry) {
      homeListWorldEntry = list.Find((HomeListWorldEntry worldEntry) =>
        worldEntry.WorldName.Equals(worldName, StringComparison.Ordinal));
      return homeListWorldEntry is not null;
    }

    #endregion

    #region Home List Player Entries

    // ReSharper disable once MemberCanBePrivate.Global
    public static HomeListPlayerEntry AddPlayer(this List<HomeListWorldEntry> list, PlayerController pc) {
      var homeListSystem = list.GetWorld(pc);
#if IDE
      if (homeListSystem.TryGetPlayer(pc, out var homeListPlayerEntry)) {
#else
      if (homeListSystem.TryGetPlayer(pc, out var homeListPlayerEntry) && homeListPlayerEntry is not null) {
#endif
        return homeListPlayerEntry;
      }

      return homeListSystem.AddPlayer(pc);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static HomeListPlayerEntry GetPlayer(this List<HomeListWorldEntry> list, PlayerController pc) {
#if IDE
      if (list.TryGetPlayer(pc, out var homeListPlayerEntry)) {
#else
      if (list.TryGetPlayer(pc, out var homeListPlayerEntry) && homeListPlayerEntry is not null) {
#endif
        return homeListPlayerEntry;
      }

      return list.AddPlayer(pc);
    }

    // ReSharper disable once MemberCanBePrivate.Global
#if IDE
    public static bool TryGetPlayer(this List<HomeListWorldEntry> list, PlayerController pc, [NotNullWhen(true)] out HomeListPlayerEntry? homeListPlayerEntry) {
#else
    public static bool TryGetPlayer(this List<HomeListWorldEntry> list, PlayerController pc, out HomeListPlayerEntry? homeListPlayerEntry) {
#endif
      homeListPlayerEntry = list.GetWorld(pc).PlayerEntries
        .Find((HomeListPlayerEntry playerEntry) => playerEntry.PlayerName == pc.playerName);
      return homeListPlayerEntry is not null;
    }

    // ReSharper disable once UnusedMember.Global
#if IDE
    public static bool TryGetPlayer(this List<HomeListWorldEntry> list, string worldName, string playerName, [NotNullWhen(true)] out HomeListPlayerEntry? homeListPlayerEntry) {
#else
    public static bool TryGetPlayer(this List<HomeListWorldEntry> list, string worldName, string playerName, out HomeListPlayerEntry? homeListPlayerEntry) {
#endif
      homeListPlayerEntry = list.GetWorld(worldName).PlayerEntries
        .Find((HomeListPlayerEntry playerEntry) => playerEntry.PlayerName == playerName);
      return homeListPlayerEntry is not null;
    }

    #endregion

    #region Home List Entries

    // ReSharper disable once UnusedMember.Global
    public static HomeListEntry AddHouse(this List<HomeListWorldEntry> list, PlayerController pc, string label,
      Vector2 position, Direction direction) {
      var playerEntry = list.GetPlayer(pc);
      var homeListEntry = HomeListEntry.Create(label, position, direction);
      playerEntry.ListOfHouses.Add(homeListEntry);
      return homeListEntry;
    }

    // ReSharper disable once UnusedMember.Global
    public static HomeListEntry SetHouse(this List<HomeListWorldEntry> list, PlayerController pc, string label,
      Vector2 position, Direction direction) {
      return list.GetPlayer(pc).SetHouse(label, pc, position, direction);
    }

    // ReSharper disable once UnusedMember.Global
    public static HomeListEntry? GetHouse(this List<HomeListWorldEntry> list, PlayerController pc, string label) {
      return list.GetPlayer(pc).ListOfHouses.FirstOrDefault((HomeListEntry house) => house.Label == label);
    }

    #endregion

    #region Home List Bed Entries

    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static HomeListEntry SetBed(this List<HomeListWorldEntry> list, PlayerController pc, Vector2 position,
      Direction direction) {
      return list.GetPlayer(pc).SetBed(pc, position, direction);
    }

    // ReSharper disable once UnusedMember.Global
    public static HomeListEntry AddBed(this List<HomeListWorldEntry> list, PlayerController pc, Vector2 position,
      Direction direction) {
      return list.GetPlayer(pc).AddBed(pc, position, direction);
    }

    // ReSharper disable once UnusedMember.Global
    public static HomeListEntry GetBed(this List<HomeListWorldEntry> list, PlayerController pc) {
      return list.GetPlayer(pc).GetBed(pc);
    }

    #endregion
  }
}
