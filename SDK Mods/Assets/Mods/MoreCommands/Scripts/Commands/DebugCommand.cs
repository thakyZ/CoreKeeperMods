#nullable enable
using System;
using Unity.Entities;
using CoreLib.Commands;
using NekoBoiNick.CoreKeeper.Common.Util;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Chat.Commands {
  // ReSharper disable once UnusedType.Global
  public class DebugCommand : IServerCommandHandler {
    public CommandOutput Execute(string[] parameters, Entity sender) {
      if (parameters.Length != 1) {
        return GetDescription();
      }

      if (parameters[0].Equals("print", StringComparison.OrdinalIgnoreCase)) {
        return PrintRunningConfig();
      }
      // ReSharper disable once InvertIf
      if (parameters[0].Equals("kill", StringComparison.OrdinalIgnoreCase)) {
        if (sender.IsAdmin()) {
          sender.GetPlayerController().KillThroughStuckOption();
        }
      }

      return "";
    }

    public string GetDescription() {
      return "Command to debug this mod.";
    }

    public string[] GetTriggerNames() {
      return new[] { "mc_debug" };
    }

    private static CommandOutput PrintRunningConfig() {
      try {
        Logger.Info("MoreCommandsMod.Config = \n" + MoreCommandsMod.Config);
        return new CommandOutput("Successfully printed to console.");
      } catch (Exception exception) {
        const string Message = "Failed to print to console.";
        Logger.Exception(exception, Message);
        return new CommandOutput(Message);
      }
    }
  }
}
