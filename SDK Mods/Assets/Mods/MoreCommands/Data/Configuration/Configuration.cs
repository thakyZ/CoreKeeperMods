#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoreCommands.Systems;
using Logger = NekoBoiNick.CoreKeeper.Common.Util.Logger;

namespace MoreCommands.Data.Configuration {
  [Serializable]
  public class Configuration : IConfiguration {
    //[JsonComment("Disable specific commands")]
    [JsonPropertyName("commands_enabled")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public CommandsEnabled CommandsEnabled { get; set; } = new();

    //[JsonComment("Housing System Config")]
    [JsonPropertyName("housing_system")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public List<HomeListWorldEntry?>? HomeListSystem { get; set; }

    //[JsonComment("Death System Config")]
    [JsonPropertyName("death_system")]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public List<DeathWorldEntry?>? DeathSystem { get; set; }

    public Configuration(CommandsEnabled commands_enabled, List<HomeListWorldEntry?>? housingSystem, List<DeathWorldEntry?>? deathSystem) {
      this.CommandsEnabled = commands_enabled;

      if (housingSystem is null) {
        Logger.Info("housing_system  is  null");
        this.HomeListSystem = new List<HomeListWorldEntry?>();
        this.HomeListSystem.Init();
      } else {
        this.HomeListSystem = housingSystem;
        this.HomeListSystem.Init();
      }

      if (deathSystem is null) {
        Logger.Info("death_system  is  null");
        this.DeathSystem = new List<DeathWorldEntry?>();
        this.DeathSystem.Init();
      } else {
        this.DeathSystem = deathSystem;
        this.DeathSystem.Init();
      }

      if (this.HomeListSystem is null) {
        Logger.Info("this.HousingSystem  is  null");
      }
      if (this.DeathSystem is null) {
        Logger.Info("this.DeathSystem  is  null");
      }
    }

    [JsonConstructor]
    public Configuration() { }

    [JsonIgnore]
    public static Configuration Default => new();

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);
  }

  [Serializable]
  public class CommandsEnabled {
    //[JsonComment("Enable the home command?")]
    [JsonPropertyName("home")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public bool Home { get; set; } = true;

    //[JsonComment("Enable the back command?")]
    [JsonPropertyName("back")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public bool Back { get; set; } = true;

    public CommandsEnabled(bool home, bool back)
    {
      this.Home = home;
      this.Back = back;
    }

    [JsonConstructor]
    public CommandsEnabled() { }

    public override string ToString() => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);
  }
}
