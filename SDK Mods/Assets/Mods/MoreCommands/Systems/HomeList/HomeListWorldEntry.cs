#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoreCommands.Systems.HomeList
{
  [Serializable]
  public class HomeListWorldEntry {
    [JsonPropertyName("world_name")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string WorldName { get; set; } = string.Empty;

    [JsonPropertyName("player_entries")]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public List<HomeListPlayerEntry> PlayerEntries { get; set; } = new();

    public HomeListWorldEntry(string worldName, List<HomeListPlayerEntry>? playerEntries) {
      this.WorldName     = worldName;
      this.PlayerEntries = playerEntries ?? new List<HomeListPlayerEntry>();
    }

    [JsonConstructor]
    public HomeListWorldEntry() { }

    public override string ToString()
      => JsonSerializer.Serialize(this);

    public bool HasPlayerEntry(PlayerController pc)
      => PlayerEntries.Exists(x => x.PlayerName == pc.playerName);

    public HomeListPlayerEntry AddPlayer(PlayerController pc) {
      if (this.HasPlayerEntry(pc)) {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }

      var playerEntry = new HomeListPlayerEntry(pc);

      this.PlayerEntries.Add(playerEntry);
      return playerEntry;
    }

    public HomeListEntry AddHouse(string label, PlayerController pc) {
      var entry = this.AddPlayer(pc);
      if (!entry.HasHouse(label)) {
        return entry.AddHouse(label, pc);
      }
      return entry.GetHouse(label, pc) ?? throw new Exception($"Failed when getting the house with label \"{label}\", this shouldn't happen.");
    }

    public bool TryGetPlayerEntry(string playerName, [NotNullWhen(true)] out HomeListPlayerEntry? housingPlayerEntry) {
      housingPlayerEntry = this.PlayerEntries.Find(x => x.PlayerName.Equals(playerName, StringComparison.Ordinal));
      return housingPlayerEntry is not null;
    }
  }
}
