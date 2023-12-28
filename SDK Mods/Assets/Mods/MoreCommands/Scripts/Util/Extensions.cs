using System.Collections.Generic;

using CoreLib.Commands;

using MoreCommands.Systems;

using PugMod;

using Unity.Entities;

#nullable enable

namespace MoreCommands.Util
{
  public static class Extensions
  {
    public static string TrimQuotes(this string input, out CommandOutput? failedCommand)
    {
      failedCommand = null;
      if ((input.StartsWith('"') && input.EndsWith('"')) || (input.StartsWith('\'') && input.EndsWith('\''))) {
        return input[..1].Substring(input.Length - 2, 1);
      } else if (input.StartsWith('"') && !input.EndsWith('"')) {
        failedCommand = new CommandOutput($"No matching \" found at index: {input.Length}");
        return input;
      } else if (!input.StartsWith('"') && input.EndsWith('"')) {
        failedCommand = new CommandOutput("No matching \" found at index: 0");
        return input;
      } else if (input.StartsWith('\'') && !input.EndsWith('\'')) {
        failedCommand = new CommandOutput($"No matching \' found at index: {input.Length}");
        return input;
      } else if (!input.StartsWith('\'') && input.EndsWith('\'')) {
        failedCommand = new CommandOutput("No matching \' found at index: 0");
        return input;
      }
      return input;
    }

    public static bool IsAdmin(this Entity playerEntity) {
      return playerEntity.GetPlayerController().adminPrivileges > 0;
    }

    public static void AddHousingEntry(this List<HouseEntry> entries, string label, PlayerController player) {
      var housingEntry = new HouseEntry(label, entries) {
        Position = player.WorldPosition,
        Direction = player.facingDirection
      };

      entries.Add(housingEntry);
    }

    public static void AddHousingEntry(this List<HouseEntry> entries, PlayerController player) {
      var housingEntry = new HouseEntry("bed", entries);
      if (API.Server.World.EntityManager.HasComponent<PlayerClaimedBed>(player.entity))
      {
        var claimedBed = API.Server.World.EntityManager.GetComponentData<PlayerClaimedBed>(player.entity);
        housingEntry.Position = claimedBed.position;
        housingEntry.Direction = API.Server.World.EntityManager.GetComponentObject<Bed>(claimedBed.claimedBedEntity).rotationIndex.ToDirection();
      }
      else
      {
        housingEntry.Position = PlayerController.PLAYER_SPAWN_POSITION;
        housingEntry.Direction = Direction.forward;
      }

      entries.Add(housingEntry);
    }

    public static Direction ToDirection(this int value)
    {
      return value switch
      {
        1 => Direction.left,
        2 => Direction.back,
        3 => Direction.right,
        _ => Direction.forward,
      };
    }
  }
}