#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

    public static DeathEntry AddDeathEntry(this List<DeathEntry> entries, PlayerController player) {
      var deathEntry = DeathEntry.Create(player.SmoothWorldPosition, player.facingDirection);

      entries.Add(deathEntry);
      return deathEntry;
    }

    public static DeathPlayerEntry AddEntryOnDeath(this List<DeathPlayerEntry> entries, PlayerController player) {
      var playerEntry = DeathPlayerEntry.CreateOnDeath(player);

      entries.Add(playerEntry);
      return playerEntry;
    }

    public static DeathWorldEntry AddEntry(this List<DeathWorldEntry> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var deathWorldEntry)) {
        return deathWorldEntry;
      }
      var temp = new DeathWorldEntry(worldName, null);
      list.Add(temp);
      return temp;
    }

    public static DeathPlayerEntry AddPlayerEntry(this List<DeathWorldEntry> list, PlayerController pc)
      => list.GetWorldEntry(pc.world.Name).PlayerEntries.AddEntryOnDeath(pc);

    public static DeathWorldEntry GetWorldEntry(this List<DeathWorldEntry> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var deathSystem)) {
        return deathSystem;
      }

      return list.AddEntry(worldName);
    }

    public static DeathPlayerEntry GetPlayerEntry(this List<DeathWorldEntry> list, PlayerController pc)
      => list.GetPlayerEntry(pc.world.Name, pc);

    public static DeathPlayerEntry GetPlayerEntry(this List<DeathWorldEntry> list, string worldName, PlayerController pc) {
      var deathSystem = list.GetWorldEntry(worldName);

      if (deathSystem.TryGetPlayerEntry(pc.playerName, out var deathPlayerEntry)) {
        return deathPlayerEntry;
      }

      return deathSystem.AddEntry(pc);
    }

    public static bool TryGetWorldEntry(this List<DeathWorldEntry> list, string worldName, [NotNullWhen(true)] out DeathWorldEntry? deathWorldEntry) {
      deathWorldEntry = list.Find(x => x?.WorldName.Equals(worldName, StringComparison.Ordinal) == true);
      return deathWorldEntry is not null;
    }
  }
}
