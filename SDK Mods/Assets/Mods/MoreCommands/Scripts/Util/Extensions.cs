using System.Numerics;

using CoreLib.Commands;

using Unity.Entities;
using Unity.Mathematics;

#nullable enable

namespace MoreCommands.Util {
  public static class Extensions {
    public static string TrimQuotes(this string input, out CommandOutput? failedCommand) {
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

    public static Vector3 ToVector3(this float3 @float3) {
      return new Vector3(@float3.x, @float3.y, @float3.z);
    }

    public static float3 ToVector3(this Vector3 vector3) {
      return new float3(vector3.X, vector3.Y, vector3.Z);
    }

    public static Direction ToDirection(this int value) {
      return value switch {
        1 => Direction.left,
        2 => Direction.back,
        3 => Direction.right,
        _ => Direction.forward,
      };
    }

    public static string ToMathString(this UnityEngine.Vector3 value) {
      return string.Format("({0}, {1}, {2})", value.x, value.y, value.z);
    }
  }
}
