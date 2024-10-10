#nullable enable
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using PugMod;
using CoreLib;
using CoreLib.Commands;
using MoreCommands.Chat.Commands;
using MoreCommands.Data.Configuration;
using Logger = NekoBoiNick.CoreKeeper.Common.Util.Logger;

namespace MoreCommands
{
  public class MoreCommandsMod : IMod
  {
    private static MoreCommandsMod? instance;
    public const string VERSION = "1.0.0";
    public const string NAME = "More Commands";
    public const string AUTHOR = "Neko Boi Nick";

    private JsonConfigFile<Configuration>? config;
    public static Configuration? Config => instance?.config?.Context;

    internal static LoadedMod? ModInfo { get; private set; }

    public void EarlyInit()
    {
      instance = this;
      Logger.Init(NAME);
      Logger.Info($"Mod version: {VERSION}");

      ModInfo = API.ModLoader.LoadedMods.FirstOrDefault(modInfo => modInfo.Handlers.Contains(this));

      if (ModInfo is null)
      {
        Logger.Error($"Failed to load {NAME}: mod metadata not found!");
        return;
      }

      config = new JsonConfigFile<Configuration>("MoreCommands/MoreCommands.json", true, ModInfo);

      CoreLibMod.LoadModule(typeof(CommandsModule));
      CommandsModule.AddCommands(ModInfo.ModId, NAME);
    }

    public void Init()
    {
      if (this.config is null)
      {
        Logger.Info("Config is null.");
      }
      else if (this.config.Context is null)
      {
        Logger.Info("Config.Context is null.");
      }
      else if (this.config.Context.CommandsEnabled is null)
      {
        Logger.Info("Config.Context.CommandsEnabled is null.");
      }
      else if (this.config.Context.CommandsEnabled.Home != false && this.config.Context.CommandsEnabled.Home != true)
      {
        Logger.Info("Config.Context.CommandsEnabled.Home is null.");
      }
      else if (this.config.Context.CommandsEnabled.Home != true)
      {
        Logger.Info("Unregistered command /home");
        CommandsModule.UnregisterCommandHandler(typeof(HomeCommand));
      }
      else if (this.config.Context.CommandsEnabled.Back != true)
      {
        Logger.Info("Unregistered command /back");
        CommandsModule.UnregisterCommandHandler(typeof(BackCommand));
      }
      else
      {
        Logger.Info("Mod loaded successfully");
        Debug.Log("Mod loaded successfully");
        return;
      }

      Logger.Info("Mod failed to load successfully");
      Debug.Log("Mod failed to load successfully");
    }

    public void Shutdown()
    {
      this.config?.Save();
    }

    public void ModObjectLoaded(Object obj) { }

    public void Update() { }
  }
}
