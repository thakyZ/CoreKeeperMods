#nullable enable
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using PugMod;
using CoreLib;
using CoreLib.Commands;
using MoreCommands.Data.Configuration;
using MoreCommands.Systems.Death;
using MoreCommands.Systems.HomeList;
using NekoBoiNick.CoreKeeper.Common.Util;
using Unity.Entities;
using Unity.Mathematics;
using Logger = NekoBoiNick.CoreKeeper.Common.Util.Logger;

// ReSharper disable once CheckNamespace
namespace MoreCommands {
  // ReSharper disable once ClassNeverInstantiated.Global
  public class MoreCommandsMod : IMod {
    // ReSharper disable once InconsistentNaming
    private static MoreCommandsMod _instance = null!;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public const string MOD_VERSION = "1.0.0";

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public const string MOD_NAME = "More Commands";

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public const string MOD_AUTHOR = "Neko Boi Nick";

    // ReSharper disable once InconsistentNaming
    private JsonConfigFile<Configuration> _config = null!;
    public static Configuration Config => _instance._config.Context;

    // ReSharper disable once MemberCanBePrivate.Global
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
      _config = new JsonConfigFile<Configuration>("MoreCommands/MoreCommands.json", true, ModInfo, () => new Configuration());
      CoreLibMod.LoadModule(typeof(CommandsModule));
      CommandsModule.AddCommands(ModInfo.ModId, MOD_NAME);
    }

    public void Init() {
      // throw new System.NotImplementedException();
    }

    public void Shutdown() {
      this._config.Save();
    }

    public void ModObjectLoaded(Object obj) {
      // throw new System.NotImplementedException();
    }

    public bool CanBeUnloaded() {
      this._config.Save();
      return true;
    }

    public void Update() {
      // throw new System.NotImplementedException();
    }

    public static void AddPlayerOnDeath(Entity pe) {
      Logger.Info($"Entity.GetPlayerEntity().GetPlayerName() == {pe.GetPlayerName()}");
      var pc = pe.GetPlayerControllerSafe();
      if (pc is null) {
        return;
      }
      Config.DeathSystem.AddPlayerOnDeath(pc, pc.SmoothWorldPosition, pc.facingDirection);
    }

    public static void SetBed(Entity pe, float2 playerClaimedBed) {
      Logger.Info($"Entity.GetPlayerEntity().GetPlayerName() == {pe.GetPlayerName()}");
      var pc = pe.GetPlayerControllerSafe();
      if (pc is null) {
        return;
      }
      Config.HomeListSystem.SetBed(pc, playerClaimedBed, pc.facingDirection);
    }
  }
}
