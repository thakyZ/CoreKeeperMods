#nullable enable
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.Death {
  public static class DeathSystem {
    // ReSharper disable once UnusedMethodReturnValue.Global
    // ReSharper disable once UnusedParameter.Global
    /// <summary>
    ///     Teleports the player to the position associated with the specified map marker entity.
    /// </summary>
    /// <param name="player">The player controller responsible for the teleportation.</param>
    /// <param name="targetPosition">The target position.</param>
    /// <param name="targetDirection">The target direction.</param>
    /// <returns><c>true</c> if the teleportation was successful; otherwise, <c>false</c>.</returns>
    public static bool TryTeleportPlayerToPosition(PlayerController player, Vector3 targetPosition, Direction targetDirection) {
      try {
        player.QueueInputAction(new UIInputActionData {
          action = UIInputAction.Teleport,
          position = targetPosition.ToFloat2(),
        });

        return true;
      } catch {
        return false;
      }
    }
  }
}
