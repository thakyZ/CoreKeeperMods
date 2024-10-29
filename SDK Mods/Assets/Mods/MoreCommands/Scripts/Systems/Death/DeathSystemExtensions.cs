#nullable enable
using System;
using System.Collections.Generic;
#if IDE
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.Death {
  public static class DeathSystemExtensions {
    public static void Init(this List<DeathWorldEntry> list) {
      for (int i = 0; i < list.Count; i++) {
        DeathWorldEntry? value = list[i];
        if (value == null) {
          list[i] = new DeathWorldEntry();
        }
      }
    }

#region Death World Entries

    // ReSharper disable once UnusedMember.Global
    public static DeathWorldEntry AddWorld(this List<DeathWorldEntry> list, PlayerController pc) {
      return list.AddWorld(pc.world);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static DeathWorldEntry AddWorld(this List<DeathWorldEntry> list, World world) {
      return list.AddWorld(world.Name);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static DeathWorldEntry AddWorld(this List<DeathWorldEntry> list, string worldName) {
      if (list.TryGetWorld(worldName, out var deathWorldEntry)) {
        return deathWorldEntry;
      }

      var temp = new DeathWorldEntry(worldName, null);
      list.Add(temp);
      return temp;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static DeathWorldEntry GetWorld(this List<DeathWorldEntry> list, PlayerController pc) {
      return list.GetWorld(pc.world.Name);
    }

    // ReSharper disable once UnusedMember.Global
    public static DeathWorldEntry GetWorld(this List<DeathWorldEntry> list, World world) {
      return list.GetWorld(world.Name);
    }

    public static DeathWorldEntry GetWorld(this List<DeathWorldEntry> list, string worldName) {
      if (list.TryGetWorld(worldName, out var deathSystem)) {
        return deathSystem;
      }

      return list.AddWorld(worldName);
    }

    // ReSharper disable once MemberCanBePrivate.Global
#if IDE
    public static bool TryGetWorld(this List<DeathWorldEntry> list, PlayerController pc, [NotNullWhen(true)] out DeathWorldEntry? deathWorldEntry) {
#else
    public static bool TryGetWorld(this List<DeathWorldEntry> list, PlayerController pc, out DeathWorldEntry deathWorldEntry) {
#endif
      return list.TryGetWorld(pc.world, out deathWorldEntry);
    }

    // ReSharper disable once MemberCanBePrivate.Global
#if IDE
    public static bool TryGetWorld(this List<DeathWorldEntry> list, World world, [NotNullWhen(true)] out DeathWorldEntry? deathWorldEntry) {
#else
    public static bool TryGetWorld(this List<DeathWorldEntry> list, World world, out DeathWorldEntry deathWorldEntry) {
#endif
      return list.TryGetWorld(world.Name, out deathWorldEntry);
    }

#if IDE
    public static bool TryGetWorld(this List<DeathWorldEntry> list, string worldName, [NotNullWhen(true)] out DeathWorldEntry? deathWorldEntry) {
#else
    public static bool TryGetWorld(this List<DeathWorldEntry> list, string worldName, out DeathWorldEntry deathWorldEntry) {
#endif
      deathWorldEntry = list.Find((DeathWorldEntry worldEntry) =>
        worldEntry.WorldName.Equals(worldName, StringComparison.Ordinal));
      return deathWorldEntry is not null;
    }

    #endregion

#region Death Player Entries

    // ReSharper disable once UnusedMethodReturnValue.Global
    public static DeathPlayerEntry AddPlayerOnDeath(this List<DeathWorldEntry> list, PlayerController pc,
      Vector3 position, Direction direction) {
      var deathSystem = list.GetWorld(pc);

      if (deathSystem.TryGetPlayer(pc.playerName, out var deathPlayerEntry)) {
        return deathPlayerEntry;
      }

      return deathSystem.AddPlayerOnDeath(pc, position, direction);
    }

    public static DeathPlayerEntry AddPlayer(this List<DeathWorldEntry> list, PlayerController pc) {
      var deathSystem = list.GetWorld(pc);

      if (deathSystem.TryGetPlayer(pc.playerName, out var deathPlayerEntry)) {
        return deathPlayerEntry;
      }

      return deathSystem.AddPlayer(pc);
    }

    public static DeathPlayerEntry GetPlayer(this List<DeathWorldEntry> list, PlayerController pc) {
      if (list.TryGetPlayer(pc, out var deathPlayerEntry)) {
        return deathPlayerEntry;
      }

      return list.AddPlayer(pc);
    }


    // ReSharper disable once MemberCanBePrivate.Global
#if IDE
    public static bool TryGetPlayer(this List<DeathWorldEntry> list, PlayerController pc, [NotNullWhen(true)] out DeathPlayerEntry? deathPlayerEntry) {
#else
    public static bool TryGetPlayer(this List<DeathWorldEntry> list, PlayerController pc, out DeathPlayerEntry deathPlayerEntry) {
#endif
      deathPlayerEntry = list.GetWorld(pc).PlayerEntries.Find((DeathPlayerEntry playerEntry) => playerEntry.PlayerName == pc.playerName);
      return deathPlayerEntry is not null;
    }


#if IDE
    public static bool TryGetPlayer(this List<DeathWorldEntry> list, string worldName, string playerName, [NotNullWhen(true)] out DeathPlayerEntry? deathPlayerEntry) {
#else
    public static bool TryGetPlayer(this List<DeathWorldEntry> list, string worldName, string playerName, out DeathPlayerEntry deathPlayerEntry) {
#endif
      deathPlayerEntry = list.GetWorld(worldName).PlayerEntries.Find((DeathPlayerEntry playerEntry) => playerEntry.PlayerName == playerName);
      return deathPlayerEntry is not null;
    }

#endregion

#region Death Entries

    // ReSharper disable once UnusedMember.Global
    public static DeathEntry AddDeathEntry(this List<DeathWorldEntry> list, PlayerController pc, Vector3 position, Direction direction) {
      var playerEntry = list.GetPlayer(pc);
      var deathEntry = DeathEntry.Create(position, direction);
      playerEntry.DeathPositions.Add(deathEntry);
      return deathEntry;
    }

    public static DeathEntry? GetLastDeathEntry(this List<DeathWorldEntry> list, PlayerController pc) {
      var playerEntry = list.GetPlayer(pc);
      return playerEntry.DeathPositions.Last();
    }

#endregion
  }
}
