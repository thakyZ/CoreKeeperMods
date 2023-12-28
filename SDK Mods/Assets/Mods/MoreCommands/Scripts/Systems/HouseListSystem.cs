using System.Collections.Generic;

using CoreLib;

using Unity.Entities;
using Unity.Mathematics;

namespace MoreCommands.Systems
{
  public class HouseEntry
  {
    public string Label { get; set; }
    public float3 Position { get; set; }
    public Direction Direction { get; set; }
    private static List<HouseEntry> _houseEntryInstance;

    public HouseEntry(string label, List<HouseEntry> parent)
    {
      _houseEntryInstance = parent;
      Label = label;
    }

    public int GetIndex()
    {
      return _houseEntryInstance.FindIndex(x => x.Equals(this));
    }

    public static bool Equals(HouseEntry x, HouseEntry y)
    {
      return x.Label == y.Label;
    }

    public bool Equals(HouseEntry obj)
    {
      return obj.Label == this.Label;
    }

    public new bool Equals(object obj)
    {
      if (obj.GetType() == typeof(HouseEntry))
      {
        return Equals((HouseEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(HouseEntry obj)
    {
      return obj.Label.GetHashCode();
    }

    public new int GetHashCode()
    {
      return this.Label.GetHashCode();
    }
  }

  public class PlayerEntry
  {
    public int PlayerIndex { get; set; }
    public string PlayerName { get; set; }
    public List<HouseEntry> ListOfHouses { get; } = new();
    private static List<PlayerEntry> _playerEntryInstance;

    public PlayerEntry(PlayerController player, List<PlayerEntry> parent)
    {
      _playerEntryInstance = parent;
      PlayerName = player.playerName;
      PlayerIndex = player.playerIndex;
    }

    public int GetIndex()
    {
      return _playerEntryInstance.FindIndex(x => x.Equals(this));
    }

    public static bool Equals(HouseEntry x, HouseEntry y)
    {
      return x.Label == y.Label;
    }

    public bool Equals(PlayerEntry obj)
    {
      return obj.PlayerName == this.PlayerName;
    }

    public new bool Equals(object obj)
    {
      if (obj.GetType() == typeof(PlayerEntry))
      {
        return Equals((PlayerEntry)obj);
      }

      return false;
    }

    public static int GetHashCode(PlayerEntry obj)
    {
      return obj.PlayerName.GetHashCode();
    }

    public new int GetHashCode()
    {
      return this.PlayerName.GetHashCode();
    }
  }

  public class HousingSystem
  {
    List<PlayerEntry> PlayerEntries { get; } = new();

    public HousingSystem() { }
  }

  public class HousingSystemModule : BaseSubmodule
  {
    private readonly Dictionary<string, HousingSystem> _worldHousingSystems = new();

    public HousingSystemModule() { }

    public HousingSystem GetHousingSystem(World world)
    {
      if (_worldHousingSystems.TryGetValue(world.Name, out var housingSystem))
      {
        return housingSystem;
      }

      _worldHousingSystems.Add(world.Name, new HousingSystem());
      return GetHousingSystem(world);
    }
  }
}
