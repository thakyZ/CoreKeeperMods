using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.CoreKeeperMods.Shared.Extensions {
  /// <summary>
  /// Extensions for various or all <see langword="Exception"/>s to simplify code.
  /// </summary>
  internal static class ExceptionExtensions {
    [SuppressMessage("ReSharper", "RedundantLinebreak"), SuppressMessage("ReSharper", "ArrangeMethodOrOperatorBody")]
    public static string GetFullyQualifiedExceptionText(this Exception exception) {
      StringBuilder sb = new StringBuilder()
       .Append('[')
       .Append(exception.GetType().Name)
       .Append("] ")
       .AppendLine(exception.Message);

      if (exception.HelpLink is not null) {
        sb.Append("Get help at: ")
         .Append(exception.HelpLink);
      }

      if (exception.StackTrace is not null) {
        sb.AppendLine(exception.StackTrace);
      }

      if (exception.InnerException is not null) {
        sb.AppendLine(exception.InnerException.GetFullyQualifiedExceptionText());
      }

      return sb.ToString();
    }

    [DoesNotReturn]
    private static void ThrowGreaterEqual<T>(T value, T other, string? paramName) =>
      throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be less than '{other}'.");

    [DoesNotReturn]
    private static void ThrowLess<T>(T value, T other, string? paramName) =>
      throw new ArgumentOutOfRangeException(paramName, value,
        $"{paramName} ('{value}') must be greater than or equal to '{other}'.");

    /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal <paramref name="other"/>.</summary>
    /// <param name="value">The argument to validate as less than <paramref name="other"/>.</param>
    /// <param name="other">The value to compare with <paramref name="value"/>.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfGreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
      if (value.CompareTo(other) >= 0) {
        ThrowGreaterEqual(value, other, paramName);
      }
    }

    /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.</summary>
    /// <param name="value">The argument to validate as greater than or equal than <paramref name="other"/>.</param>
    /// <param name="other">The value to compare with <paramref name="value"/>.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
      if (value.CompareTo(other) < 0) {
        ThrowLess(value, other, paramName);
      }
    }
  }
}
