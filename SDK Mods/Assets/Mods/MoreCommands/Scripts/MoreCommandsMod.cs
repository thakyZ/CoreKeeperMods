// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;

using CoreLib;
using CoreLib.Commands;
using CoreLib.Data.Configuration;

using MoreCommands.Chat.Commands;
using MoreCommands.Systems;

using PugMod;

using UnityEngine;

namespace MoreCommands
{
  public class MoreCommandsMod : IMod
  {
    public const string VERSION = "1.0.0";
    public const string NAME = "More Commands";

    public static ConfigFile Config { get; private set; }

    internal static HousingSystem HousingSystem { get; private set; }

    #region Config Options
    public static ConfigEntry<bool> HouseEnabled { get; private set; }
    public static ConfigEntry<bool> BackEnabled { get; private set; }
    #endregion

    public void EarlyInit()
    {
      Debug.Log($"[{NAME}]: Mod version: {VERSION}");

      var modInfo = GetModInfo(this);

      if (modInfo == null)
      {
        Debug.Log($"[{NAME}]: Failed to load {NAME}: mod metadata not found!");
        return;
      }

      Config = new ConfigFile("MoreCommands/MoreCommands.cfg", true, modInfo);
      HouseEnabled = Config.Bind("Commands Enabled", "house", true, new ConfigDescription("Enable the house command?", new AcceptableValueList<bool>(true, false)));
      BackEnabled = Config.Bind("Commands Enabled", "back", true, new ConfigDescription("Enable the back command?", new AcceptableValueList<bool>(true, false)));

      CoreLibMod.LoadModule(typeof(CommandsModule));
      CommandsModule.AddCommands(modInfo.ModId, NAME);
      if (!HouseEnabled.Value)
      {
        Debug.Log($"[{NAME}]: Unregistered command /home");
        CommandsModule.UnregisterCommandHandler(typeof(HomeCommand));
      }
      else
      {
        foreach (var file in API.ConfigFilesystem.GetFiles("MoreCommands")) {
          Debug.Log($"[{NAME}]: {file}");
        }

      }
      if (!BackEnabled.Value)
      {
        Debug.Log($"[{NAME}]: Unregistered command /back");
        CommandsModule.UnregisterCommandHandler(typeof(BackCommand));
      }
      else
      {
      }

      Debug.Log($"[{NAME}]: Mod loaded successfully");
    }

    public void Init() { }

    public void Shutdown() { }

    public void ModObjectLoaded(Object obj) { }

    public void Update() { }

    public static LoadedMod GetModInfo(IMod mod)
    {
      return API.ModLoader.LoadedMods.FirstOrDefault(modInfo => modInfo.Handlers.Contains(mod));
    }
  }
}
