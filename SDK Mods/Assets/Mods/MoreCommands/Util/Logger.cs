#nullable enable
using CoreLogger = CoreLib.Util.Logger;

namespace MoreCommands.Util {
  public class Logger {
    private static Logger? instance;
    private CoreLogger Log { get; }

    private Logger(string modName) {
      this.Log = new CoreLogger(modName);
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
