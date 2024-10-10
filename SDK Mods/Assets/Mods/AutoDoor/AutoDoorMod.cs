﻿#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PugMod;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
namespace Lever.AutoDoors
{
    public class AutoDoorMod : IMod
    {
        public const string MOD_NAME = "AutoDoors";
        public const string MOD_VERSION = "1.1.1";
        private LoadedMod? ModInfo { get; set; }

        public void EarlyInit()
        {
            Debug.Log($"Loading mod {MOD_NAME} version {MOD_VERSION}...");

            ModInfo = API.ModLoader.LoadedMods.FirstOrDefault(modInfo => modInfo.Handlers.Contains(this));

            Debug.Log($"Finished loading mod {MOD_NAME} v{MOD_VERSION}");
        }

        public void Init()
        {
            //throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            //throw new System.NotImplementedException();
        }

        public void ModObjectLoaded(Object obj)
        {
            //throw new System.NotImplementedException();
        }

        public void Update()
        {
            //throw new System.NotImplementedException();
        }
    }

    public partial class DoorSwitchSystem : PugSimulationSystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<GhostPredictionSwitchingQueues>();
        }

        protected override void OnUpdate()
        {
            if (Manager.main.player == null)
            {
              return;
            }

            const double DistanceToTriggerLocal = 0.95;

            var playerPosition = Manager.main.player.WorldPosition;

            var switchingQueues = SystemAPI.GetSingletonRW<GhostPredictionSwitchingQueues>().ValueRW;

            Entities.WithAll<DoorCD>().ForEach((
                    Entity entity,
                    ref ObjectDataCD objectData,
                    in LocalTransform translation,
                    in GhostInstance ghostInstance) =>
                {
                    if (ghostInstance.ghostType < 0)
                    {
                      return;
                    }

                    Debug.Log("objectData.objectID         = " + objectData.objectID);
                    Debug.Log("entity                      = " + entity);
                    Debug.Log("entity (FixedString)        = " + entity.ToFixedString());
                    Debug.Log("ghostInstance               = " + ghostInstance);
                    try {
                        Debug.Log("ghostInstance (FixedString) = " + ghostInstance.ToFixedString());
                    } catch (Exception exception) {
                      Debug.Log("ghostInstance (FixedString) = !EXCEPTION!");
                      Debug.Log('[' + exception.GetType().Name + "] " + exception.Message + '\n' + (exception.HelpLink is not null ? "Get Help From: " + exception.HelpLink + '\n' : "") + (exception.StackTrace ?? ""));
                    }

                    var distance = math.distancesq(translation.Position, playerPosition);
                    var playerNearby = distance <= DistanceToTriggerLocal;

                    var isPredicted = SystemAPI.HasComponent<PredictedGhost>(entity);

                    if (playerNearby && !isPredicted)
                    {
                        switchingQueues.ConvertToPredictedQueue.Enqueue(new ConvertPredictionEntry
                        {
                            TargetEntity = entity,
                            TransitionDurationSeconds = 1f,
                        });
                        return;
                    }

                    if (!playerNearby && isPredicted)
                    {
                        switchingQueues.ConvertToInterpolatedQueue.Enqueue(new ConvertPredictionEntry
                        {
                            TargetEntity = entity,
                            TransitionDurationSeconds = 1f,
                        });
                        return;
                    }
                })
                .WithNone<SwitchPredictionSmoothing>()
                .WithoutBurst()
                .Schedule();

            base.OnUpdate();
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class DoorGateStateChecker : PugSimulationSystemBase
    {
        internal static void SetOpen(ref ObjectDataCD objectData, bool open)
        {
            // if we should open the door
            if (open)
            {
                objectData.variation = objectData.variation switch
                {
                    0 => 1,
                    2 => 3,
                    _ => objectData.variation
                };
            }
            // if we should close the door
            else
            {
                objectData.variation = objectData.variation switch
                {
                    1 => 0,
                    3 => 2,
                    _ => objectData.variation
                };
            }
        }

        protected override void OnUpdate()
        {
            const double DistanceToTriggerLocal = 0.95;

            // get and store player positions
            var playerPositions = new NativeList<float3>(World.UpdateAllocator.ToAllocator);
            Entities
                .WithAll<PlayerGhost>().ForEach((in LocalTransform translation) =>
                {
                    playerPositions.Add(translation.Position);
                })
                .Schedule();

            Entities
                .WithAll<PredictedGhost, Simulate, DoorCD>()
                .ForEach((ref ObjectDataCD objectData, in LocalTransform translation) =>
                {
                    var anyPlayerNearby = false;
                    foreach (var playerPos in playerPositions)
                    {
                        var distance = math.distancesq(translation.Position, playerPos);

                        if (distance > DistanceToTriggerLocal)
                        {
                            continue;
                        }

                        anyPlayerNearby = true;
                        //break;
                        Debug.Log($"Gate/Door ({objectData.objectID}) distance to player is {distance} and variation is {objectData.variation}");
                    }

                    SetOpen(ref objectData, anyPlayerNearby);
                })
                .WithoutBurst()
                .WithDisposeOnCompletion(playerPositions)
                .Schedule();

            base.OnUpdate();
        }
    }
}
