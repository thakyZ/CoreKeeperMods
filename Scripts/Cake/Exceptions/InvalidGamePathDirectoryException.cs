using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using NekoBoiNick.CoreKeeperMods.Shared.Extensions;

namespace Cake.NekoBoiNick.CoreKeeperMods.Exceptions;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("Roslynator", "RCS1085")]
internal sealed class InvalidGamePathDirectoryException : Exception {
  private const    string  InvalidGamePathDirectoryGeneric = "Invalid Game Path Provided";
  private const    string  ArgArgumentException            = "Value does not fall within the expected range.";
  private const    string  ArgParamNameName                = "(Parameter '{0}', Value '{1}')";
  private readonly string? _paramName;
  private readonly string? _promptValue;
  private          string? _message;
  // ReSharper disable ConvertToAutoPropertyWithPrivateSetter
  public string? ParamName   => _paramName;
  public string? PromptValue => _promptValue;
  // ReSharper restore ConvertToAutoPropertyWithPrivateSetter
  public override string Message {
    get {
      SetMessageField();

      var @string = base.Message;
      if (!string.IsNullOrEmpty(_paramName) && !string.IsNullOrEmpty(_promptValue)) {
        @string += $" {string.Format(ArgParamNameName, _paramName, _promptValue)}";
      }

      return @string;
    }
  }

  public InvalidGamePathDirectoryException() : base(InvalidGamePathDirectoryGeneric) {
    _message = base.Message;
  }

  public InvalidGamePathDirectoryException(string? promptValue, string? paramName) : base(InvalidGamePathDirectoryGeneric) {
    _message     = base.Message;
    _promptValue = promptValue;
    _paramName   = paramName;
    HResult      = unchecked((int)0x80070057); /* COR_E_ARGUMENT */
  }

  public InvalidGamePathDirectoryException(string? message) : base(message ?? InvalidGamePathDirectoryGeneric) {
    _message = base.Message;
    HResult  = unchecked((int)0x80070057); /* COR_E_ARGUMENT */
  }

  public InvalidGamePathDirectoryException(string? message, string? promptValue, string? paramName) : base(message ?? InvalidGamePathDirectoryGeneric) {
    _message     = base.Message;
    _promptValue = promptValue;
    _paramName   = paramName;
    HResult      = unchecked((int)0x80070057); /* COR_E_ARGUMENT */
  }

  public InvalidGamePathDirectoryException(string? message, Exception? innerException) : base(message ?? InvalidGamePathDirectoryGeneric, innerException) {
    _message = base.Message;
    HResult  = unchecked((int)0x80070057); /* COR_E_ARGUMENT */
  }

  public InvalidGamePathDirectoryException(string? promptValue, string? paramName, Exception? innerException) : base(InvalidGamePathDirectoryGeneric, innerException) {
    _message     = base.Message;
    _promptValue = promptValue;
    _paramName   = paramName;
    HResult      = unchecked((int)0x80070057); /* COR_E_ARGUMENT */
  }

  public InvalidGamePathDirectoryException(string? message, string? promptValue, string? paramName, Exception? innerException) : base(message ?? InvalidGamePathDirectoryGeneric, innerException) {
    _message     = base.Message;
    _promptValue = promptValue;
    _paramName   = paramName;
    HResult      = unchecked((int)0x80070057); /* COR_E_ARGUMENT */
  }

  private void SetMessageField() {
    if (_message is null && HResult == unchecked((int)0x80070057) /* COR_E_ARGUMENT */) {
      _message = ArgArgumentException;
    }
  }

  internal static void ThrowPromptNull([NotNull] string? promptValue, [CallerArgumentExpression(nameof(promptValue))] string? paramName = null) {
    if (promptValue is null) {
      Throw(promptValue, paramName);
    }
  }

  private static bool TestIfIsSystemPath(string value) {
    var output = false;
    try {
      output = new DirectoryInfo(value) is { Exists: true };
    } catch (Exception exception) {
      Debug.WriteLineIf(Environment.GetEnvironmentVariable("DEBUG") is string @string
                     && @string.Equals("true", StringComparison.OrdinalIgnoreCase),
        exception.GetFullyQualifiedExceptionText());
    }
    return output;
  }

  internal static void ThrowPromptInvalidSystemPath([NotNull] string? promptValue, [CallerArgumentExpression(nameof(promptValue))] string? paramName = null) {
    if (promptValue is null || TestIfIsSystemPath(promptValue)) {
      Throw(promptValue, paramName);
    }
  }

  [DoesNotReturn()]
  internal static void Throw(string? promptValue, string? paramName) =>
    throw new InvalidGamePathDirectoryException(promptValue, paramName);
}
