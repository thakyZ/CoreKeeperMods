#nullable enable
using System;
using System.Diagnostics;
using CoreLib.Commands;

using Unity.Entities;
using Unity.Mathematics;

namespace NekoBoiNick.CoreKeeper.Common.Util {
  public static class Extensions {
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

      if (!input.StartsWith('\'') && input.EndsWith('\'')) {
        failedCommand = new CommandOutput("No matching \' found at index: 0");
        return input;
      }

      return input;
    }

    public static string GetFullyQualifiedExceptionMessage(this Exception exception, string? message = null) {
      string output = '[' + exception.GetType().Name + ']';

      if (message is not null) {
        output += message + '\n';
      }

      output += exception.Message + '\n';

      if (exception.HelpLink is not null) {
        output += "Get help from: " + exception.HelpLink;
      }

      if (exception.StackTrace is not null) {
        output += exception.StackTrace;
      }

      return output;
    }

    public static string PrintExceptionLike(this StackTrace? stackTrace, string message, Type? exceptionType = null) {
      return '[' + (exceptionType ?? typeof(Exception)).Name + ']' + message + '\n' + (stackTrace?.ToString() ?? "null StackTrace");
    }

    public static bool Equals(this float3 @value, float3? other) {
      if (other is null) {
        return false;
      }

      return @value.x.Equals(other.Value.x)
             && @value.y.Equals(other.Value.y)
             && @value.z.Equals(other.Value.z);
    }

    public static bool Equals(this float3 @value, object? obj) {
      if (obj is float3 other) {
        return @value.Equals(other: other);
      }

      return false;
    }

    public static bool Equals(this float2 @value, float2? other) {
      if (other is null) {
        return false;
      }

      return @value.x.Equals(other.Value.x)
             && @value.y.Equals(other.Value.y);
    }

    public static bool Equals(this float2 @value, object? obj) {
      if (obj is float2 other) {
        return @value.Equals(other: other);
      }

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
