using System;
using MoreCommands.Systems;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Text.Json;

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
    public List<HomeListWorldEntry> HomeListSystem { get; set; } = new();

    //[JsonComment("Death System Config")]
    [JsonPropertyName("death_system")]
    [JsonPropertyOrder(3)]
    [JsonRequired()]
    public List<DeathWorldEntry> DeathSystem { get; set; } = new();

    public Configuration(CommandsEnabled commands_enabled, List<HomeListWorldEntry> housing_system, List<DeathWorldEntry> death_system) {
      this.CommandsEnabled = commands_enabled;

      if (housing_system == null) {
        MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: housing_system  is  null");
        this.HomeListSystem = new();
        this.HomeListSystem.Init();
      } else {
        this.HomeListSystem = housing_system;
        this.HomeListSystem.Init();
      }

      if (death_system == null) {
        MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: death_system  is  null");
        this.DeathSystem = new();
        this.DeathSystem.Init();
      } else {
        this.DeathSystem = death_system;
        this.DeathSystem.Init();
      }

      if (this.HomeListSystem == null) {
        MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: this.HousingSystem  is  null");
      }
      if (this.DeathSystem == null) {
        MoreCommandsMod.Log.LogInfo($"[{MoreCommandsMod.NAME}]: this.DeathSystem  is  null");
      }
    }

    [JsonConstructor()]
    public Configuration() { }

    [JsonIgnore()]
    public static Configuration Default => new();

    public override string ToString() => JsonSerializer.Serialize(this);
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

    public CommandsEnabled(bool home, bool back)
    {
      this.Home = home;
      this.Back = back;
    }

    [JsonConstructor()]
    public CommandsEnabled() { }

    public override string ToString() => JsonSerializer.Serialize(this);
  }
}
