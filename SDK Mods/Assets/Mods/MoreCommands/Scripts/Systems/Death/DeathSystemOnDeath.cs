#nullable enable
using CoreLib.Commands;
using PlayerState;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Logger = NekoBoiNick.CoreKeeper.Common.Util.Logger;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.Death {
  [UpdateInGroup(typeof(SimulationSystemGroup))]
  [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
  public partial class DeathSystemOnDeath : PugSimulationSystemBase {
    protected override void OnUpdate() {
      Entities.WithAll<DeathStateCD, PlayerStateCD>().ForEach((
          Entity entity,
          ref ObjectDataCD objectData,
          ref PlayerStateCD playerState,
          in LocalTransform translation,
          in GhostInstance ghostInstance) => {
          var pe = entity.GetPlayerEntity();

          Logger.Info($"Entity.GetPlayerEntity() is {(pe == null ? "" : "not ")}null");
          if (pe != null) {
            MoreCommandsMod.AddPlayerOnDeath(pe);
          }
        })
        .WithBurst()
        .Schedule();

      base.OnUpdate();
    }
  }
}
