using System.Linq;
using CoreLib.Commands;
using CoreLib.Commands.Communication;
using HarmonyLib;

using MoreCommands.Util;

using PugMod;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

#nullable enable

namespace MoreCommands.Chat.Commands {
  public class BackCommand : IServerCommandHandler {
    public CommandOutput Execute(string[] parameters, Entity sender) {
      var playerEntity = sender.GetPlayerEntity();

      return GoBackToDeath(playerEntity);
    }

    public string GetDescription() {
      return "Use /back to teleport back to death point. \n";
    }

    public string[] GetTriggerNames() {
      return new[] { "back" };
    }

    private CommandOutput GoBackToDeath(Entity playerEntity) {
      return "Soup";
    }
  }
}