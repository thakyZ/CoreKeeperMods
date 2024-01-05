// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Assets.Mods.MoreCommands.Scripts.Util;
using MoreCommands.Systems;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Assets.Mods.MoreCommands.Scripts {
  [Serializable]
  public class Configuration : IConfiguration {
    //[JsonComment("Disable specific commands")]
    [JsonPropertyName("commands_enabled")]
    [JsonRequired]
    public CommandsEnabled CommandsEnabled { get; set; } = new();

    //[JsonComment("Housing System Config")]
    [JsonPropertyName("housing_system")]
    [JsonRequired]
    public HousingSystemModule HousingSystem { get; set; } = new();

    //[JsonComment("Death System Config")]
    [JsonPropertyName("death_system")]
    [JsonRequired]
    public DeathSystemModule DeathSystem { get; set; } = new();
  }

  [Serializable]
  public class CommandsEnabled {
    //[JsonComment("Enable the home command?")]
    [JsonPropertyName("home")]
    [JsonRequired]
    public bool Home { get; set; } = true;

    //[JsonComment("Enable the back command?")]
    [JsonPropertyName("back")]
    [JsonRequired]
    public bool Back { get; set; } = true;
  }
}
