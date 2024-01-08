using System;

using HarmonyLib;

using MoreCommands.Systems;

namespace MoreCommands.Patches {
#nullable enable
  [HarmonyPatch()]
  public static class Death_Patch {
    [HarmonyPatch(typeof(PlayerState.Death), "RespawnPlayer")]
    [HarmonyPrefix()]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony patches require a number of underscore for parameter names.")]
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
            MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context.DeathSystem.Count  is  0 - A");
          }
          MoreCommandsMod.Config.Context.DeathSystem.AddPlayerEntry(__instance.pc);
          if (MoreCommandsMod.Config.Context.DeathSystem.Count == 0) {
            MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context.DeathSystem.Count  is  0 - B");
          } else {
            MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context.DeathSystem.Count  is  {MoreCommandsMod.Config.Context.DeathSystem.Count} - C");
          }
          if (MoreCommandsMod.Config.Context.DeathSystem.GetPlayerEntry(__instance.pc.world.Name, __instance.pc).DeathPositions.Count == 0) {
            MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: MoreCommandsMod.Config.Context.DeathSystem.GetDeathPlayerEntry(\"{__instance.pc.world.Name}\", \"{__instance.pc.playerName}\").DeathPositions.Count  is  0 - D");
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
