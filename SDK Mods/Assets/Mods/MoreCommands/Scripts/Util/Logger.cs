#nullable enable
using System;
using MoreCommands;
using CoreLogger = CoreLib.Util.Logger;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.CoreKeeper.Common.Util {
  public sealed class Logger {
    private static Logger? instance;
    private CoreLogger Log { get; }

    private Logger(string modName) {
      this.Log = new CoreLogger(modName);
    }

    public static void Exception(Exception exception, string? message = null, bool verbose = false) {
      if (verbose && !MoreCommandsMod.Config.VerboseLogging) {
        return;
      }
      instance?.Log.LogError(message is not null
        ? $"{message}\n{exception}"
        : exception.ToString());
    }

    // ReSharper disable once UnusedMember.Global
    public static void Exception(string message, Exception? exception = null, bool verbose = false) {
      if (verbose && !MoreCommandsMod.Config.VerboseLogging) {
        return;
      }
      instance?.Log.LogError(exception is not null
        ? $"{message}\n{exception}"
        : message);
    }

    public static void Init(string modName) {
      instance = new Logger(modName);
    }

    private void InfoImpl(string message) {
      this.Log.LogInfo(message);
    }

    public static void Info(string message, bool verbose = false) {
      if (verbose && !MoreCommandsMod.Config.VerboseLogging) {
        return;
      }
      instance?.InfoImpl(message);
    }

    private void ErrorImpl(string message) {
      this.Log.LogError(message);
    }

    public static void Error(string message, bool verbose = false) {
      if (verbose && !MoreCommandsMod.Config.VerboseLogging) {
        return;
      }
      instance?.ErrorImpl(message);
    }

    private void WarnImpl(string message) {
      this.Log.LogWarning(message);
    }

    public static void Warn(string message, bool verbose = false) {
      if (verbose && !MoreCommandsMod.Config.VerboseLogging) {
        return;
      }
      instance?.WarnImpl(message);
    }
  }
}
