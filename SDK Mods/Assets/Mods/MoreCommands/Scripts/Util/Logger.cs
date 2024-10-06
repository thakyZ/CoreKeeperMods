#nullable enable
using CoreLogger = CoreLib.Util.Logger;

namespace MoreCommands.Util {
  public class Logger {
    private static Logger? instance;
    private CoreLogger Log { get; }
    private string ModName { get; }

    private Logger(string modName) {
      this.ModName = modName;
      this.Log = new CoreLogger(modName);
    }

    public static void Init(string modName) {
      instance = new Logger(modName);
    }

    public void InfoImpl(string message) {
      this.Log.LogInfo($"[{this.ModName}]: {message}");
    }

    public static void Info(string message) {
      instance?.InfoImpl(message);
    }

    public void ErrorImpl(string message) {
      this.Log.LogError($"[{this.ModName}]: {message}");
    }

    public static void Error(string message) {
      instance?.ErrorImpl(message);
    }
  }
}
