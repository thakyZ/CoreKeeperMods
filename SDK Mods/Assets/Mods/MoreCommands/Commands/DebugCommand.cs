#nullable enable
using System;
using Unity.Entities;
using CoreLib.Commands;
using NekoBoiNick.CoreKeeper.Common.Util;

namespace MoreCommands.Chat.Commands
{
  public class DebugCommand : IServerCommandHandler
  {
    public CommandOutput Execute(string[] parameters, Entity sender)
    {
      if (parameters.Length == 1 && parameters[0].Equals("print", System.StringComparison.OrdinalIgnoreCase))
      {
        return PrintRunningConfig();
      }

      return "";
    }

    public string GetDescription()
    {
      return "Command to debug this mod.";
    }

    public string[] GetTriggerNames()
    {
      return new[] { "mc_debug" };
    }

    public CommandOutput PrintRunningConfig() {
      try {
        Logger.Info("MoreCommandsMod.Config = \n" + MoreCommandsMod.Config ?? "null");
        return new CommandOutput("Successsfuly printed to console.");
      } catch (Exception exception) {
        const string Message = "Failed to print to console.";
        Logger.Exception(exception, Message);
        return new CommandOutput(Message);
      }
    }
  }
#nullable disable
}
