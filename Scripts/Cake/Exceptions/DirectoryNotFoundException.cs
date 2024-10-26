using System;
using IO_DirectoryNotFoundException = System.IO.DirectoryNotFoundException;
using IO_DirectoryInfo = System.IO.DirectoryInfo;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using Cake.Core.IO;
using Cake.Core;
using Cake.Common.IO;

namespace Cake.NekoBoiNick.CoreKeeperMods.Exceptions;

[Serializable]
internal class DirectoryNotFoundException : IO_DirectoryNotFoundException {
  public DirectoryNotFoundException() {
  }

  public DirectoryNotFoundException(string? message) : base(message) {
  }

  public DirectoryNotFoundException(string? message, Exception? innerException) : base(message, innerException) {
  }

  internal static void ThrowIfNotFound(IDirectory directory, [CallerArgumentExpression(nameof(directory))] string? paramName = null) {
    if (!directory.Exists) {
      ThrowIfNotFoundException(directory, paramName);
    }
  }

  [DoesNotReturn]
  internal static void ThrowIfNotFoundException(IDirectory directory, string? paramName = null) {
    throw new DirectoryNotFoundException($"Failed to find {paramName}: {directory}");
  }

  internal static void ThrowIfNotFound(DirectoryPath directory, ICakeContext context, [CallerArgumentExpression(nameof(directory))] string? paramName = null) {
    if (!context.DirectoryExists(directory)) {
      ThrowIfNotFoundException(directory, paramName);
    }
  }

  [DoesNotReturn]
  internal static void ThrowIfNotFoundException(DirectoryPath directory, string? paramName = null) {
    throw new DirectoryNotFoundException($"Failed to find {paramName}: {directory}");
  }

  internal static void ThrowIfNotFound(IO_DirectoryInfo directory, [CallerArgumentExpression(nameof(directory))] string? paramName = null) {
    if (!directory.Exists) {
      ThrowIfNotFoundException(directory, paramName);
    }
  }

  [DoesNotReturn]
  internal static void ThrowIfNotFoundException(IO_DirectoryInfo directory, string? paramName = null) {
    throw new DirectoryNotFoundException($"Failed to find {paramName}: {directory}");
  }
}
