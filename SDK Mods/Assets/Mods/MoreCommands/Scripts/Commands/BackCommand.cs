using System;
using System.Linq;
using CoreLib.Commands;
using CoreLib.Commands.Communication;
using Unity.Entities;
using UnityEngine;
using MoreCommands.Util;
using MoreCommands.Systems;
using System.Text;

namespace MoreCommands.Chat.Commands {
#nullable enable
  public class BackCommand : IServerCommandHandler {
    public CommandOutput Execute(string[] parameters, Entity sender) {
      var playerController = sender.GetPlayerController();

      if (parameters.Length >= 1) {
        if (string.Equals(parameters[0], "worlds", StringComparison.OrdinalIgnoreCase)) {
          if (parameters.Length >= 2) {
            if (string.Equals(parameters[0], "list", StringComparison.OrdinalIgnoreCase)) {
              var entry = MoreCommandsMod.Config.Context.DeathSystem;
              var output = new StringBuilder();
              foreach (var (index, worldEntry) in entry.Select((v, i) => (i, v))) {
                output.Append('[').Append(index).Append("] \"").Append(worldEntry.WorldName).AppendLine("\"");
              }
              return new CommandOutput(output.ToString(), CommandStatus.Info);
            } else if (int.TryParse(parameters[1].ToLower(), out var index)) {
              var entry = MoreCommandsMod.Config.Context.DeathSystem[index];
              return new CommandOutput($"Name: \"{entry.WorldName}\"\nCount: {entry.PlayerEntries.Count}", CommandStatus.Info);
            } else if (MoreCommandsMod.Config.Context.DeathSystem.TryGetWorldEntry(parameters[1].ToLower(), out var deathWorldEntry)) {
              return new CommandOutput($"Name: \"{deathWorldEntry.WorldName}\"\nCount: {deathWorldEntry.PlayerEntries.Count}", CommandStatus.Info);
            }
          }
        } else if (string.Equals(parameters[0], "players", StringComparison.OrdinalIgnoreCase)) {
          if (parameters.Length >= 2) {
            if (string.Equals(parameters[1], "list", StringComparison.OrdinalIgnoreCase)) {
              var entry = MoreCommandsMod.Config.Context.DeathSystem.GetWorldEntry(playerController.world.Name).PlayerEntries;
              var output = new StringBuilder();
              foreach (var (index, playerEntry) in entry.Select((v, i) => (i, v))) {
                output.Append('[').Append(index).Append("] \"").Append(playerEntry.PlayerName).Append("\" \"").Append(playerEntry.PlayerUuid).AppendLine("\"");
              }
              return new CommandOutput(output.ToString(), CommandStatus.Info);
            } else if (int.TryParse(parameters[1].ToLower(), out var index)) {
              var entry = MoreCommandsMod.Config.Context.DeathSystem.GetWorldEntry(playerController.world.Name).PlayerEntries[index];
              return new CommandOutput($"Name: \"{entry.PlayerName}\"\nCount: {entry.DeathPositions.Count}", CommandStatus.Info);
            } else if (MoreCommandsMod.Config.Context.DeathSystem.GetWorldEntry(playerController.world.Name).TryGetPlayerEntry(parameters[1].ToLower(), out var deathPlayerEntry)) {
              return new CommandOutput($"Name: \"{deathPlayerEntry.PlayerName}\"\nCount: {deathPlayerEntry.DeathPositions.Count}", CommandStatus.Info);
            }
          }
        }
      }

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
        var deathEntry = MoreCommandsMod.Config.Context.DeathSystem.GetPlayerEntry(playerController.world.Name, playerController).DeathPositions.Last();
        playerController.shadow.SetActive(true);
        playerController.SetPlayerPosition(deathEntry.Position);
        playerController.facingDirection = deathEntry.Direction;
        playerController.TeleportAnyLeashedCattleToPosition(deathEntry.Position + new Vector3(0f, 0f, -0.1f));
        return new CommandOutput($"Returned to death position at {deathEntry.Position.ToMathString()}", CommandStatus.Info);
      } catch (Exception exception) {
        MoreCommandsMod.Log.LogError($"[{MoreCommandsMod.NAME}]: Failed to run death command.\n{exception.Message}\n{exception.StackTrace}");
        return new CommandOutput($"Failed to run death command.\n{exception.Message}", CommandStatus.Error);
      }
    }
  }
#nullable disable
}
