using MoreCommands.Chat.Commands;
using HarmonyLib;

#nullable enable

namespace MoreCommands.Chat {
    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "<Pending>")]
    public static class Death_Patch {
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.AE_FootStep))]
        [HarmonyPrefix]
        public static bool AE_FootStep() {
            return false;
        }
    }
}