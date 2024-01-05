using System.Collections.Generic;

using CoreLib;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;
using Rewired;
using MoreCommands.Util;
using System.Linq;

namespace MoreCommands.Systems
{
  public class DeathEntry
  {
    public Vector3 Position { get; set; } = PlayerController.PLAYER_SPAWN_POSITION;
    public Direction Direction { get; set; } = Direction.forward;
    private static List<DeathEntry> _deathEntryInstance;

    public DeathEntry(Vector3 position, Direction direction, List<DeathEntry> parent)
    {
      _deathEntryInstance = parent;
      Position = position;
      Direction = direction;
    }

    public int GetIndex()
    {
      return _deathEntryInstance.FindIndex(x => x.Equals(this));
    }

    public static bool Equals(DeathEntry x, DeathEntry y)
    {
      return x.Position == y.Position && x.Direction == y.Direction;
    }

    public bool Equals(DeathEntry obj)
    {
      return obj.Position == this.Position && obj.Direction == this.Direction;
    }

    public new bool Equals(object obj)
    {
      if (obj.GetType() == typeof(DeathEntry))
      {
        return Equals((DeathEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(DeathEntry obj)
    {
      return HashCode.Combine(obj.Direction.GetHashCode(), obj.Position.GetHashCode());
    }

    public new int GetHashCode()
    {
      return HashCode.Combine(this.Direction.GetHashCode(), this.Position.GetHashCode());
    }
  }

  public class DeathPlayerEntry
  {
    public string PlayerUuid { get; set; }
    public string PlayerName { get; set; }
    public List<DeathEntry> DeathPositions { get; } = new();
    private static List<DeathPlayerEntry> _playerEntryInstance;

    public DeathPlayerEntry(PlayerController player, List<DeathPlayerEntry> parent)
    {
      _playerEntryInstance = parent;
      PlayerName = player.playerName;
    }

    public int GetIndex() {
      return _playerEntryInstance.FindIndex(x => x.Equals(this));
    }

    public static bool Equals(DeathPlayerEntry x, DeathPlayerEntry y) {
      return x.PlayerName == y.PlayerName;
    }

    public bool Equals(DeathPlayerEntry obj) {
      return obj.PlayerName == this.PlayerName;
    }

    public new bool Equals(object obj) {
      if (obj.GetType() == typeof(DeathPlayerEntry)) {
        return Equals((DeathPlayerEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(DeathPlayerEntry obj) {
      return obj.PlayerName.GetHashCode();
    }

    public new int GetHashCode() {
      return this.PlayerName.GetHashCode();
    }
  }

  public class DeathSystem {
    public List<DeathPlayerEntry> PlayerEntries { get; } = new();

    internal DeathPlayerEntry AddEntry(PlayerController pc) {
      if (PlayerEntries.Any(x => x.PlayerName == pc.playerName)) {
        return PlayerEntries.First(x => x.PlayerName == pc.playerName);
      }
      return PlayerEntries.AddEntry(pc);
    }

    public bool TryGetPlayerEntry(string playerName, out DeathPlayerEntry deathPlayerEntry) {
      foreach (var entry in PlayerEntries) {
        if (entry.PlayerName == playerName) {
          deathPlayerEntry = entry;
          return true;
        }
      }
      deathPlayerEntry = null;
      return false;
    }
  }

  public class DeathSystemModule : BaseSubmodule
  {
    private readonly Dictionary<string, DeathSystem> _worldDeathSystems = new();

    public DeathSystemModule() { }

    public DeathSystem GetDeathSystem(World world)
    {
      if (_worldDeathSystems.TryGetValue(world.Name, out var deathSystem))
      {
        return deathSystem;
      }

      _worldDeathSystems.Add(world.Name, new DeathSystem());
      return _worldDeathSystems[world.Name];
    }

    public DeathPlayerEntry GetDeathPlayerEntry(World world, PlayerController pc)
    {
      var deathSystem = GetDeathSystem(world);

      if (deathSystem.TryGetPlayerEntry(pc.playerName, out var deathPlayerEntry))
      {
        return deathPlayerEntry;
      }

      return deathSystem.AddEntry(pc);
    }

    public void AddEntry(PlayerController pc) {
      GetDeathSystem(pc.world).PlayerEntries.AddEntry(pc);
    }
  }
}
