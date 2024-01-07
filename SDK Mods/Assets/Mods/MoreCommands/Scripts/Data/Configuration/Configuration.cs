using System;
using MoreCommands.Systems;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using System.Collections.Generic;

namespace MoreCommands.Data.Configuration {
  [Serializable()]
  public class Configuration : IConfiguration {
    //[JsonComment("Disable specific commands")]
    [JsonPropertyName("commands_enabled")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public CommandsEnabled CommandsEnabled { get; set; } = new();

    //[JsonComment("Housing System Config")]
    [JsonPropertyName("housing_system")]
    [JsonPropertyOrder(2)]
    [JsonRequired()]
    public Dictionary<string, HousingSystem> HousingSystem { get; set; } = new();

    //[JsonComment("Death System Config")]
    [JsonPropertyName("death_system")]
    [JsonPropertyOrder(3)]
    [JsonRequired()]
    public Dictionary<string, DeathSystem> DeathSystem { get; set; } = new();

    [JsonConstructor]
    public Configuration(CommandsEnabled commands_enabled, Dictionary<string, HousingSystem> housing_system, Dictionary<string, DeathSystem> death_system) {
      this.CommandsEnabled = commands_enabled;

      if (housing_system == null) {
        MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: housing_system  is  null");
        this.HousingSystem = new();
        this.HousingSystem.Init();
      } else {
        this.HousingSystem = housing_system;
        this.HousingSystem.Init();
      }

      if (death_system == null) {
        MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: death_system  is  null");
        this.DeathSystem = new();
        this.DeathSystem.Init();
      } else {
        this.DeathSystem = death_system;
        this.DeathSystem.Init();
      }

      if (this.HousingSystem == null) {
        MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: this.HousingSystem  is  null");
      }
      if (this.DeathSystem == null) {
        MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: this.DeathSystem  is  null");
      }
    }

    public Configuration()
    {
    }

    public static Configuration Default => new();
  }

  [Serializable()]
  public class CommandsEnabled {
    //[JsonComment("Enable the home command?")]
    [JsonPropertyName("home")]
    [JsonPropertyOrder(1)]
    [JsonRequired()]
    public bool Home { get; set; } = true;

    //[JsonComment("Enable the back command?")]
    [JsonPropertyName("back")]
    [JsonPropertyOrder(2)]
    [JsonRequired()]
    public bool Back { get; set; } = true;

    [JsonConstructor]
    public CommandsEnabled(bool home, bool back)
    {
      this.Home = home;
      this.Back = back;
    }

    public CommandsEnabled() { }
  }
}
