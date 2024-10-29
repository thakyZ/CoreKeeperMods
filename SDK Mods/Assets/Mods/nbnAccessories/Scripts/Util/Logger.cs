#nullable enable
using System;
using System.Diagnostics;
using CoreLogger = CoreLib.Util.Logger;

namespace NekoBoiNick.CoreKeeper.Common.Util {
  public sealed class Logger {
    private static Logger? instance;
    private CoreLogger Log { get; }

    private Logger(string modName) {
      this.Log = new CoreLogger(modName);
    }

    public static void Exception(Exception exception, string? message = null) {
      instance?.Log.LogError(exception.GetFullyQualifiedExceptionMessage(message));
    }

    public static void Exception(string message, Exception? exception = null) {
      StackTrace? stackTrace = null;
      if (exception is null) {
        stackTrace = new StackTrace(1);
      }
      instance?.Log.LogError(exception is not null ? exception.GetFullyQualifiedExceptionMessage(message) : stackTrace.PrintExceptionLike(message));
    }

    public static void Init(string modName) {
      instance = new Logger(modName);
    }

    public void InfoImpl(string message) {
      this.Log.LogInfo(message);
    }

    public static void Info(string message) {
      instance?.InfoImpl(message);
    }

    public void ErrorImpl(string message) {
      this.Log.LogError(message);
    }

    public static void Error(string message) {
      instance?.ErrorImpl(message);
    }

    public void WarnImpl(string message) {
      this.Log.LogWarning(message);
    }

    public static void Warn(string message) {
      instance?.WarnImpl(message);
    }
  }
}
