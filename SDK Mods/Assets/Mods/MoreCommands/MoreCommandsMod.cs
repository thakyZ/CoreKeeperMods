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

namespace MoreCommands {
  public class MoreCommandsMod : IMod {
    private static MoreCommandsMod? _instance;
    public const string MOD_VERSION = "1.0.0";
    public const string MOD_NAME = "More Commands";
    public const string MOD_AUTHOR = "Neko Boi Nick";
    private JsonConfigFile<Configuration>? _config;
    public static Configuration? Config => _instance?._config?.Context;
    internal static LoadedMod? ModInfo { get; private set; }

    public void EarlyInit() {
      _instance = this;
      Logger.Init(MOD_NAME);
      Logger.Info($"Loading mod {MOD_NAME} v{MOD_VERSION}...");
      ModInfo = API.ModLoader.LoadedMods.FirstOrDefault(modInfo => modInfo.Handlers.Contains(this));
      if (ModInfo is null) {
        Logger.Error($"Failed to load {MOD_NAME}: mod metadata not found!");
        return;
      }

      Debug.Log($"Finished loading mod {MOD_NAME} v{MOD_VERSION}");
      _config = new JsonConfigFile<Configuration>("MoreCommands/MoreCommands.json", true, ModInfo);
      CoreLibMod.LoadModule(typeof(CommandsModule));
      CommandsModule.AddCommands(ModInfo.ModId, MOD_NAME);
    }

    public void Init() {
    }

    public void Shutdown() {
      this._config?.Save();
    }

    public void ModObjectLoaded(Object obj) {
    }

    public void Update() {
    }
  }
}
