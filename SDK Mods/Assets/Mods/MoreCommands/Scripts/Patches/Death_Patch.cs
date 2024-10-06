﻿#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

using PlayerState;

using HarmonyLib;

using CoreLib.Commands;

using MoreCommands.Systems;
using Logger = MoreCommands.Util.Logger;

namespace MoreCommands.Patches {
#nullable enable
  [HarmonyPatch]
  public static class Death_Patch {
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony patches require a number of underscore for parameter names.")]
    [HarmonyPatch(typeof(Death), "RespawnPlayer")]
    [HarmonyPrefix]
    public static bool RespawnPlayer_Prefix(StateUpdateAspect stateUpdateAspect, SharedStateUpdateData sharedStateUpdateData, LookupStateUpdateData lookupStateUpdateData) {
      if (stateUpdateAspect.entity.GetPlayerController() is not PlayerController pc) {
        Logger.Info($"stateUpdateAspect.entity.GetPlayerController() is not PlayerController");
        return true;
      }

      try {
        if (MoreCommandsMod.Config is null) {
          Logger.Info($"MoreCommandsMod.Config  is  null");
        } else if (MoreCommandsMod.Config.Context is null) {
          Logger.Info($"MoreCommandsMod.Config.Context  is  null");
        } else if (MoreCommandsMod.Config.Context.DeathSystem is null) {
          Logger.Info($"MoreCommandsMod.Config.Context.DeathSystem  is  null");
        } else {
          if (MoreCommandsMod.Config.Context.DeathSystem.Count is 0) {
            Logger.Info($"MoreCommandsMod.Config.Context.DeathSystem.Count  is  0 - A");
          }
          MoreCommandsMod.Config.Context.DeathSystem.AddPlayerEntry(pc);
          if (MoreCommandsMod.Config.Context.DeathSystem.Count == 0) {
            Logger.Info($"MoreCommandsMod.Config.Context.DeathSystem.Count  is  0 - B");
          } else {
            Logger.Info($"MoreCommandsMod.Config.Context.DeathSystem.Count  is  {MoreCommandsMod.Config.Context.DeathSystem.Count} - C");
          }
          if (MoreCommandsMod.Config.Context.DeathSystem.GetPlayerEntry(pc.world.Name, pc).DeathPositions.Count == 0) {
            Logger.Info($"MoreCommandsMod.Config.Context.DeathSystem.GetDeathPlayerEntry(\"{pc.world.Name}\", \"{pc.playerName}\").DeathPositions.Count  is  0 - D");
          }
        }
      } catch (Exception exception) {
        Logger.Error($"Failed to add a Death Entry, for player character \"{pc.playerName}\".\n{exception.Message}\n{exception.StackTrace}");
      }
      return true;
    }
  }
#nullable disable
}
