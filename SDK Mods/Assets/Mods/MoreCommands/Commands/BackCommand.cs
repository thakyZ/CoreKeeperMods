#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using CoreLib.Commands;
using CoreLib.Commands.Communication;
using NekoBoiNick.CoreKeeper.Common.Util;
using Logger = NekoBoiNick.CoreKeeper.Common.Util.Logger;
using MoreCommands.Systems;

namespace MoreCommands.Chat.Commands {
  public class BackCommand : IServerCommandHandler {
    public CommandOutput Execute(string[] parameters, Entity sender) {
      var playerController = sender.GetPlayerController();

      if (parameters.Length >= 1) {
        if (string.Equals(parameters[0], "worlds", StringComparison.OrdinalIgnoreCase) && parameters.Length >= 2) {
          if (string.Equals(parameters[0], "list", StringComparison.OrdinalIgnoreCase)) {
            if (MoreCommandsMod.Config?.DeathSystem is List<DeathWorldEntry?> entry) {
              var output = entry.Select((x, i) => (Index: i, DeathEntry: x, Output: "")).Aggregate((total, current) => (total.Index, total.DeathEntry, total.Output + $"[{current.Index}] \"{current.DeathEntry?.WorldName}\"\n"));
              return new CommandOutput(output.Output, status: CommandStatus.Info);
            }

            return new CommandOutput("Unable to find world entry list.", CommandStatus.Error);
          }

          if (int.TryParse(parameters[1].ToLower(), out var index)) {
            if (MoreCommandsMod.Config?.DeathSystem?[index] is DeathWorldEntry entry) {
              return new CommandOutput($"Name: \"{entry.WorldName}\"\nCount: {entry.PlayerEntries.Count}", CommandStatus.Info);
            }

            return new CommandOutput($"Unable to find world entry at index {index}.", CommandStatus.Error);
          }

          if (MoreCommandsMod.Config?.DeathSystem?.TryGetWorldEntry(parameters[1].ToLower(), out var deathWorldEntry) == true) {
            return new CommandOutput($"Name: \"{deathWorldEntry.WorldName}\"\nCount: {deathWorldEntry.PlayerEntries.Count}", CommandStatus.Info);
          }
        } else if (string.Equals(parameters[0], "players", StringComparison.OrdinalIgnoreCase) && parameters.Length >= 2) {
          if (string.Equals(parameters[1], "list", StringComparison.OrdinalIgnoreCase)) {
            if (MoreCommandsMod.Config?.DeathSystem is List<DeathWorldEntry?> outList) {
              if (outList.GetWorldEntry(playerController.world.Name)?.PlayerEntries is List<DeathPlayerEntry> entry) {
                var output = entry.Select((x, i) => (Index: i, PlayerEntry: x, Output: "")).Aggregate((total, current) => (total.Index, total.PlayerEntry, total.Output + $"[{current.Index}] \"{current.PlayerEntry.PlayerName}\" \"{current.PlayerEntry.PlayerUuid}\"\n"));
                return new CommandOutput(output.Output, CommandStatus.Info);
              }

              return new CommandOutput("Unable to find world entry list.", CommandStatus.Error);
            }
          } else if (int.TryParse(parameters[1].ToLower(), out var index)) {
            if (MoreCommandsMod.Config?.DeathSystem?.GetWorldEntry(playerController.world.Name).PlayerEntries[index] is DeathPlayerEntry entry) {
              return new CommandOutput($"Name: \"{entry.PlayerName}\"\nCount: {entry.DeathPositions.Count}", CommandStatus.Info);
            }

            return new CommandOutput($"Unable to find player entry at index {index}.", CommandStatus.Error);
          } else if (MoreCommandsMod.Config?.DeathSystem?.GetWorldEntry(playerController.world.Name).TryGetPlayerEntry(parameters[1].ToLower(), out var deathPlayerEntry) == true) {
            return new CommandOutput($"Name: \"{deathPlayerEntry.PlayerName}\"\nCount: {deathPlayerEntry.DeathPositions.Count}", CommandStatus.Info);
          }
        }
      }

      return GoBackToDeath(playerController);
    }

    public string GetDescription() {
      return "Use /back to teleport back to death point.";
    }

    public string[] GetTriggerNames() {
      return new[] { "back" };
    }

    private static CommandOutput GoBackToDeath(PlayerController playerController) {
      try {
        playerController.isDyingOrDead = false;
        var deathEntry = MoreCommandsMod.Config?.DeathSystem?.GetPlayerEntry(playerController.world.Name, playerController).DeathPositions[^1];
        if (deathEntry is null) {
          throw new Exception($"inline variable of {nameof(GoBackToDeath)}, {nameof(deathEntry)} was null.");
        }
        playerController.shadow.SetActive(true);
        playerController.SetPlayerPosition(deathEntry.Position);
        playerController.facingDirection = deathEntry.Direction;
        playerController.playerCommandSystem.RemoveLoginImmunity(playerController.entity);
        return new CommandOutput($"Returned to death position at {deathEntry.Position.ToMathString()}", CommandStatus.Info);
      } catch (Exception exception) {
        Logger.Error($"Failed to run death command.\n{exception.Message}\n{exception.StackTrace}");
        return new CommandOutput($"Failed to run death command.\n{exception.Message}", CommandStatus.Error);
      }
    }
  }
#nullable disable
}
