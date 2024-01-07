using System;
using System.Linq;
using CoreLib.Commands;
using CoreLib.Commands.Communication;
using Unity.Entities;
using UnityEngine;
using MoreCommands.Util;
using MoreCommands.Systems;

namespace MoreCommands.Chat.Commands {
#nullable enable
  public class BackCommand : IServerCommandHandler {
    public CommandOutput Execute(string[] parameters, Entity sender) {
      var playerController = sender.GetPlayerController();

      return GoBackToDeath(playerController);
    }

    public string GetDescription() {
      return "Use /back to teleport back to death point. \n";
    }

    public string[] GetTriggerNames() {
      return new[] { "back" };
    }

    private CommandOutput GoBackToDeath(PlayerController playerController) {
      try {
        playerController.isDyingOrDead = false;
        var deathEntry = MoreCommandsMod.Config.Context.DeathSystem.GetDeathPlayerEntry(playerController.world, playerController).DeathPositions.Last();
        playerController.shadow.SetActive(true);
        playerController.SetPlayerPosition(deathEntry.Position);
        playerController.facingDirection = deathEntry.Direction;
        playerController.TeleportAnyLeashedCattleToPosition(deathEntry.Position + new Vector3(0f, 0f, -0.1f));
        return new CommandOutput($"Returned to death position at {deathEntry.Position.ToMathString()}", CommandStatus.Info);
      } catch (Exception exception) {
        MoreCommandsMod.Log.LogError($"Failed to run death command\n{exception.Message}\n{exception.StackTrace}");
        return new CommandOutput($"Failed to run death command\n{exception.Message}", CommandStatus.Error);
      }
    }
  }
#nullable disable
}
