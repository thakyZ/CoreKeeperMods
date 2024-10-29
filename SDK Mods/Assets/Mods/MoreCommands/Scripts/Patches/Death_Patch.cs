#nullable enable
using System;
using HarmonyLib;
using PlayerState;
using CoreLib.Commands;
using MoreCommands.Systems.Death;
using Logger = NekoBoiNick.CoreKeeper.Common.Util.Logger;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Patches {
  [HarmonyPatch]
  // ReSharper disable once InconsistentNaming
  public static class Death_Patch {
    [HarmonyPatch(typeof(Death), "RespawnPlayer")]
    [HarmonyPrefix]
    public static bool RespawnPlayer_Prefix(StateUpdateAspect stateUpdateAspect,
      SharedStateUpdateData sharedStateUpdateData, LookupStateUpdateData lookupStateUpdateData) {
      if (stateUpdateAspect.entity.GetPlayerController() is not PlayerController pc) {
        Logger.Info("stateUpdateAspect.entity.GetPlayerController() is not PlayerController");
        return true;
      }

      try {
        MoreCommandsMod.Config.DeathSystem.AddPlayer(pc);
        Logger.Info(MoreCommandsMod.Config.DeathSystem.Count == 0
          ? "MoreCommandsMod.Config.DeathSystem.Count  is  0 - B"
          : $"MoreCommandsMod.Config.DeathSystem.Count  is  {MoreCommandsMod.Config.DeathSystem.Count} - C");
        if (MoreCommandsMod.Config.DeathSystem.GetPlayer(pc).DeathPositions.Count == 0) {
          Logger.Info(
            $"MoreCommandsMod.Config.DeathSystem.GetDeathPlayerEntry(\"{pc.world.Name}\", \"{pc.playerName}\").DeathPositions.Count  is  0 - D");
        }
      } catch (Exception exception) {
        Logger.Error(
          $"Failed to add a Death Entry, for player character \"{pc.playerName}\".\n{exception.Message}\n{exception.StackTrace}");
      }

      return true;
    }
  }
}
