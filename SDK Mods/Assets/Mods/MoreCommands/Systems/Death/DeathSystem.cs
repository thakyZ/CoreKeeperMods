#nullable enable
using CoreLib.Commands;
using PlayerState;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Logger = NekoBoiNick.CoreKeeper.Common.Util.Logger;

// ReSharper disable once CheckNamespace
namespace MoreCommands.Systems.Death {
  public partial class DeathSystem : PugSimulationSystemBase {
    protected override void OnCreate()
    {
      base.OnCreate();
      RequireForUpdate<GhostPredictionSwitchingQueues>();
    }

    protected override void OnUpdate() {
      Entities.WithAll<DeathStateCD>().ForEach((
        Entity entity,
        ref ObjectDataCD objectData,
        in LocalTransform translation,
        in GhostInstance ghostInstance) => {
          if (entity == null) {
            Logger.Info("Entity is null");
            return;
          }

          if (entity.GetPlayerEntity() == null) {
            Logger.Info("Entity.GetPlayerEntity() is null");
            return;
          }

          if (entity.GetPlayerController() == null) {
            Logger.Info("Entity.GetPlayerController() is null");
            return;
          }

          if (entity.GetPlayerController().isDyingOrDead) {
            MoreCommandsMod.Config?.DeathSystem.AddPlayerEntry(entity.GetPlayerController());
          }
        })
        .WithNone<SwitchPredictionSmoothing>()
        .WithoutBurst()
        .Schedule();

      base.OnUpdate();
    }
  }
}
