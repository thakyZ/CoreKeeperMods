#nullable enable
using CoreLib.Commands;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Logger = NekoBoiNick.CoreKeeper.Common.Util.Logger;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.HomeList {
  [UpdateInGroup(typeof(SimulationSystemGroup))]
  [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
  public partial class HomeListSystemOnEepy : PugSimulationSystemBase {
    protected override void OnCreate() {
      base.OnCreate();
      RequireForUpdate<GhostPredictionSwitchingQueues>();
    }

    protected override void OnUpdate() {
      Entities.WithAll<PlayerClaimedBed, CharacterClaimedBedCD>().ForEach((
          Entity entity,
          ref PlayerClaimedBed playerClaimedBed,
          ref CharacterClaimedBedCD characterClaimedBed,
          ref ObjectDataCD objectData,
          in LocalTransform translation,
          in GhostInstance ghostInstance) => {
          var pe = entity.GetPlayerEntity();

          Logger.Info($"Entity.GetPlayerEntity() is {(pe == null ? "" : "not ")}null");
          if (pe != null) {
            MoreCommandsMod.SetBed(pe, playerClaimedBed.position);
          }
        })
        .WithBurst()
        .Schedule();

      base.OnUpdate();
    }
  }
}
