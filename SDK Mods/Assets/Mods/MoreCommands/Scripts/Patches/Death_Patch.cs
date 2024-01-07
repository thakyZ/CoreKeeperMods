using System;

using CoreLib.Util;

using HarmonyLib;

using MoreCommands.Systems;

namespace MoreCommands.Patches {
#nullable enable
  [HarmonyPatch()]
  public static class Death_Patch {
    [HarmonyPatch(typeof(PlayerState.Death), "RespawnPlayer")]
    [HarmonyPrefix()]
    public static bool RespawnPlayer_Prefix(PlayerState.Death __instance) {
      try {
        if (MoreCommandsMod.Config == null) {
          MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config  is  null");
        } else if (MoreCommandsMod.Config.Context == null) {
          MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context  is  null");
        } else if (MoreCommandsMod.Config.Context.DeathSystem == null) {
          MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context.DeathSystem  is  null");
        } else {
          if (MoreCommandsMod.Config.Context.DeathSystem.Count == 0) {
            MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context.DeathSystem.Count  is  0");
          } else {
            MoreCommandsMod.Config.Context.DeathSystem.AddEntry(__instance.pc);
          }
          if (MoreCommandsMod.Config.Context.DeathSystem.Count == 0) {
            MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context.DeathSystem.Count  is  0");
          } else {
            MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context.DeathSystem.Count  is  {MoreCommandsMod.Config.Context.DeathSystem.Count}");
          }
        }
      } catch (Exception exception) {
        MoreCommandsMod.Log.LogError($"[{MoreCommandsMod.NAME}]: Failed to add a Death Entry, for player character \"{__instance.pc.playerName}\".\n{exception.Message}\n{exception.StackTrace}");
      }
      return true;
    }
  }
#nullable disable
}
