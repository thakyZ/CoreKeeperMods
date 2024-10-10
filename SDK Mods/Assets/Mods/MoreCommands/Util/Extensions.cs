#nullable enable
using System.Diagnostics.CodeAnalysis;
using CoreLib.Commands;

using Unity.Entities;
using Unity.Mathematics;

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

    public static bool Equals(this float3 @value, float3? other) {
      if (other is null) return false;
      return @value.x.Equals(other.Value.x)
          && @value.y.Equals(other.Value.y)
          && @value.z.Equals(other.Value.z);
    }

    public static bool Equals(this float3 @value, object? obj) {
      if (obj is null) return false;
      if (obj is float3 other) return @value.Equals(other: other);
      return false;
    }

    public static bool Equals(this float2 @value, float2? other) {
      if (other is null) return false;
      return @value.x.Equals(other.Value.x)
          && @value.y.Equals(other.Value.y);
    }

    public static bool Equals(this float2 @value, object? obj) {
      if (obj is null) return false;
      if (obj is float2 other) return @value.Equals(other: other);
      return false;
    }

    public static bool IsAdmin(this Entity playerEntity) {
      return playerEntity.GetPlayerController().adminPrivileges > 0;
    }

    public static Direction ToDirection(this int value) {
      return value switch {
        1 => Direction.left,
        2 => Direction.back,
        3 => Direction.right,
        _ => Direction.forward,
      };
    }

    public static string ToMathString(this float3 value) {
      return $"[ {value.x}, {value.y}, {value.z} ]";
    }

    public static string ToMathString(this float2 value) {
      return $"[ {value.x}, {value.y} ]";
    }
  }
}
