#nullable enable
using System.Linq;

using CoreLib;
using CoreLib.Commands;

using MoreCommands.Chat.Commands;
using MoreCommands.Data.Configuration;
using Logger = MoreCommands.Util.Logger;

using PugMod;

using UnityEngine;
using Object = UnityEngine.Object;

namespace MoreCommands
{
  public class MoreCommandsMod : IMod
  {
    private static MoreCommandsMod? instance;
    public const string VERSION = "1.0.0";
    public const string NAME = "More Commands";

    private JsonConfigFile<Configuration>? config;
    public static JsonConfigFile<Configuration>? Config => instance?.config;

    public void EarlyInit()
    {
      instance = this;
      Logger.Init(NAME);
      Logger.Info($"Mod version: {VERSION}");

      var modInfo = GetModInfo(this);

      if (modInfo is null)
      {
        Logger.Error($"Failed to load {NAME}: mod metadata not found!");
        return;
      }

      config = new("MoreCommands/MoreCommands.json", true, modInfo);

      CoreLibMod.LoadModule(typeof(CommandsModule));
      CommandsModule.AddCommands(modInfo.ModId, NAME);
      if (this.config is null) {
        Logger.Info("Config is null.");
      } else if (this.config.Context is null) {
        Logger.Info("Config.Context is null.");
      } else if (this.config.Context.CommandsEnabled is null) {
        Logger.Info("Config.Context.CommandsEnabled is null.");
      } else if (this.config.Context.CommandsEnabled.Home != false && this.config.Context.CommandsEnabled.Home != true) {
        Logger.Info("Config.Context.CommandsEnabled.Home is null.");
      } else if (this.config.Context.CommandsEnabled.Home != true) {
        Logger.Info("Unregistered command /home");
        CommandsModule.UnregisterCommandHandler(typeof(HomeCommand));
      } else if (this.config.Context.CommandsEnabled.Back != true) {
        Logger.Info("Unregistered command /back");
        CommandsModule.UnregisterCommandHandler(typeof(BackCommand));
      } else {
        Logger.Info("Mod loaded successfully");
        Debug.Log("Mod loaded successfully");
        return;
      }

      Logger.Info("Mod failed to load successfully");
      Debug.Log("Mod failed to load successfully");
    }

    public void Init() { }

    public void Shutdown()
    {
      this.config?.Save();
    }

    public void ModObjectLoaded(Object obj) { }

    public void Update() { }

    public static LoadedMod GetModInfo(IMod mod)
    {
      return API.ModLoader.LoadedMods.FirstOrDefault(modInfo => modInfo.Handlers.Contains(mod));
    }
  }
}
