#nullable enable
using System;
using System.Collections.Generic;
#if IDE
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.HomeList {
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
      this.WorldName = worldName;
      this.PlayerEntries = playerEntries ?? new List<HomeListPlayerEntry>();
    }

    [JsonConstructor]
    public HomeListWorldEntry() { }

    public override string ToString()
      => JsonSerializer.Serialize(this);

    public bool HasPlayer(PlayerController pc)
      => this.TryGetPlayer(pc, out _);

    public bool HasHouse(string label, PlayerController pc)
      => this.HasPlayer(pc) && this.GetPlayer(pc).TryGetHouse(label, out _);

    public HomeListPlayerEntry GetPlayer(PlayerController pc) {
      if (this.HasPlayer(pc)) {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }

      var playerEntry = new HomeListPlayerEntry(pc);

      this.PlayerEntries.Add(playerEntry);
      return playerEntry;
    }

    public HomeListEntry GetHouse(string label, PlayerController pc) {
      if (!this.HasHouse(label, pc)) {
        return this.GetPlayer(pc).AddHouse(label, pc);
      }

      return this.GetPlayer(pc).GetHouse(label, pc) ??
             throw new Exception($"Failed when getting the house with label \"{label}\", this shouldn't happen.");
    }

    public HomeListPlayerEntry AddPlayer(PlayerController pc) {
      if (this.HasPlayer(pc)) {
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

      return entry.GetHouse(label, pc) ??
             throw new Exception($"Failed when getting the house with label \"{label}\", this shouldn't happen.");
    }

#if IDE
    public bool TryGetPlayer(PlayerController pc, [NotNullWhen(true)] out HomeListPlayerEntry? housingPlayerEntry) {
#else
    public bool TryGetPlayer(PlayerController pc, out HomeListPlayerEntry? housingPlayerEntry) {
#endif
      housingPlayerEntry = this.PlayerEntries.Find(x => x.PlayerName.Equals(pc.playerName, StringComparison.Ordinal));
      return housingPlayerEntry is not null;
    }
  }
}
