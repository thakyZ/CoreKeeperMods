#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NekoBoiNick.CoreKeeper.Common.Util;
using PugMod;
using Unity.Mathematics;

namespace MoreCommands.Systems.HomeList
{
  public static class HomeListSystemExtensions {
    public static void Init(this List<HomeListWorldEntry> list) {
      for (int i = 0; i < list.Count; i++) {
        HomeListWorldEntry? value = list[i];
        if (value == null) {
          list[i] = new HomeListWorldEntry();
        }
      }
    }

    public static HomeListWorldEntry AddPlayer(this List<HomeListWorldEntry> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var homeWorldEntry)) {
        return homeWorldEntry;
      }

      var temp = new HomeListWorldEntry(worldName, new List<HomeListPlayerEntry>());
      list.Add(temp);
      return temp;
    }

    public static HomeListPlayerEntry AddPlayerEntry(this List<HomeListWorldEntry> list, string label, PlayerController pc) {
      return list.GetWorldEntry(pc.world.Name).PlayerEntries.AddEntry(label, pc);
    }

    public static HomeListWorldEntry GetWorldEntry(this List<HomeListWorldEntry> list, string worldName) {
      if (list.TryGetWorldEntry(worldName, out var housingSystem)) {
        return housingSystem;
      }

      return list.AddWorld(worldName);
    }

    public static HomeListPlayerEntry GetPlayerEntry(this List<HomeListWorldEntry> list, string label, string worldName, PlayerController pc) {
      var housingSystem = list.GetWorldEntry(worldName);

      if (housingSystem.TryGetPlayerEntry(pc.playerName, out var housingPlayerEntry)) {
        return housingPlayerEntry;
      }

      return housingSystem.AddEntry(label, pc);
    }

    public static bool TryGetWorldEntry(this List<HomeListWorldEntry> list, string worldName, [NotNullWhen(true)] out HomeListWorldEntry? homeWorldEntry) {
      homeWorldEntry = list.Find(x => x?.WorldName.Equals(worldName, StringComparison.Ordinal) == true);
      return homeWorldEntry is not null;
    }
  }
}
