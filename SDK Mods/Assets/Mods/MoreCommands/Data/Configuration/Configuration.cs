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
    public CommandsEnabled CommandsEnabled { get; set; } = new CommandsEnabled();

    //[JsonComment("Housing System Config")]
    [JsonPropertyName("housing_system")]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public List<HomeListWorldEntry> HomeListSystem { get; set; } = new List<HomeListWorldEntry>();

    //[JsonComment("Death System Config")]
    [JsonPropertyName("death_system")]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public List<DeathWorldEntry> DeathSystem { get; set; } = new List<DeathWorldEntry>();

    public void Init() {
      this.HomeListSystem.Init();
      
      this.DeathSystem.Init();
    }

    public override string ToString()
      => JsonSerializer.Serialize(this, JsonBase.JsonSerializerOptions);
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
