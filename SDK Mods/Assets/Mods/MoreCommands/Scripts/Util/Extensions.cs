#nullable enable
using System;
using System.Linq;
using Unity.Entities;
using CoreLib.Commands;
using PugMod;
using Unity.NetCode;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.CoreKeeper.Common.Util {
  public static class Extensions {
    // ReSharper disable once UnusedMember.Global
    public static PlayerController? GetPlayerControllerSafe(this Entity sender) {
      EntityManager entityManager = API.Server.World.EntityManager;
      Entity playerEntity = sender.GetPlayerEntity();
      if (!entityManager.HasComponent<PlayerGhost>(playerEntity)) {
        Logger.Exception(new InvalidOperationException("player entity does not have a PlayerGhost component!"), verbose: true);
        return null;
      }

      PlayerGhost ghost = entityManager.GetComponentData<PlayerGhost>(playerEntity);

      return Manager.main.allPlayers.FirstOrDefault(pc => pc.world.EntityManager.GetComponentData<PlayerGhost>(pc.entity).playerGuid.Equals(ghost.playerGuid));
    }

    public static bool IsNullOrEmptyOrWhiteSpace(this string @string) {
      return string.IsNullOrWhiteSpace(@string) || string.IsNullOrEmpty(@string);
    }

    public static string TrimQuotes(this string input, out CommandOutput? failedCommand) {
      failedCommand = null;

      if (input.StartsWith('"') && input.EndsWith('"')) {
        return input.TrimEnd('"').TrimStart('"');
      }

      if (input.StartsWith('\'') && input.EndsWith('\'')) {
        return input.TrimEnd('\'').TrimStart('\'');
      }

      if (input.StartsWith('"') && !input.EndsWith('"')) {
        failedCommand = new CommandOutput($"No matching \" found at index: {input.Length}");
        return input;
      }

      if (!input.StartsWith('"') && input.EndsWith('"')) {
        failedCommand = new CommandOutput("No matching \" found at index: 0");
        return input;
      }

      if (input.StartsWith('\'') && !input.EndsWith('\'')) {
        failedCommand = new CommandOutput($"No matching \' found at index: {input.Length}");
        return input;
      }

      // ReSharper disable once InvertIf
      if (!input.StartsWith('\'') && input.EndsWith('\'')) {
        failedCommand = new CommandOutput("No matching \' found at index: 0");
        return input;
      }

      return input;
    }

    public static bool IsAdmin(this Entity playerEntity) {
      return playerEntity.GetPlayerController().adminPrivileges > 0;
    }

    // ReSharper disable once UnusedMember.Global
    public static Direction ToDirection(this int value) {
      return value switch {
        1 => Direction.left,
        2 => Direction.back,
        3 => Direction.right,
        _ => Direction.forward,
      };
    }

    // ReSharper disable once UnusedMember.Global
    public static string ToMathString(this Vector3 value) {
      return $"[ {value.x}, {value.y}, {value.z} ]";
    }

    public static string ToMathString(this Vector2 value) {
      return $"[ {value.x}, {value.y} ]";
    }
  }
}
