using System.Linq;

using CoreLib;
using CoreLib.Commands;

using MoreCommands.Chat.Commands;
using MoreCommands.Data.Configuration;

using PugMod;

using UnityEngine;

using Logger = CoreLib.Util.Logger;

namespace MoreCommands
{
  public class MoreCommandsMod : IMod
  {
    public const string VERSION = "1.0.0";
    public const string NAME = "More Commands";

    public static Logger Log = new(NAME);

    public static JsonConfigFile<Configuration> Config { get; private set; }

    public void EarlyInit()
    {
      Log.LogInfo($"[{NAME}]: Mod version: {VERSION}");

      var modInfo = GetModInfo(this);

      if (modInfo == null)
      {
        Log.LogError($"[{NAME}]: Failed to load {NAME}: mod metadata not found!");
        return;
      }

      Config = new("MoreCommands/MoreCommands.json", true, modInfo);

      CoreLibMod.LoadModule(typeof(CommandsModule));
      CommandsModule.AddCommands(modInfo.ModId, NAME);
      if (Config == null)
      {
        Log.LogInfo($"[{NAME}]: Config is null.");
      } else if (Config.Context == null)
      {
        Log.LogInfo($"[{NAME}]: Config.Context is null.");
      } else if (Config.Context.CommandsEnabled == null)
      {
        Log.LogInfo($"[{NAME}]: Config.Context.CommandsEnabled is null.");
      } else if (Config.Context.CommandsEnabled.Home != false && Config.Context.CommandsEnabled.Home != true)
      {
        Log.LogInfo($"[{NAME}]: Config.Context.CommandsEnabled.Home is null.");
      }

      if (!Config.Context.CommandsEnabled.Home)
      {
        Log.LogInfo($"[{NAME}]: Unregistered command /home");
        CommandsModule.UnregisterCommandHandler(typeof(HomeCommand));
      }

      if (!Config.Context.CommandsEnabled.Back)
      {
        Log.LogInfo($"[{NAME}]: Unregistered command /back");
        CommandsModule.UnregisterCommandHandler(typeof(BackCommand));
      }

      Debug.Log($"[{NAME}]: Mod loaded successfully");
    }

    public void Init() { }

    public void Shutdown()
    {
      Config.Save();
    }

    public void ModObjectLoaded(Object obj) { }

    public void Update() { }

    public static LoadedMod GetModInfo(IMod mod)
    {
      return API.ModLoader.LoadedMods.FirstOrDefault(modInfo => modInfo.Handlers.Contains(mod));
    }
  }
}
