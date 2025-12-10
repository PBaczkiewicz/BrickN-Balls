using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

// System that destroys balls when they enter kill zones
[BurstCompile]
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct BallKillZoneSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }
    public void OnUpdate(ref SystemState state)
    {
        var sim = SystemAPI.GetSingleton<SimulationSingleton>();

        // Set up the trigger job with necessary component lookups
        var job = new BallKillZoneJob
        {
            BallLookup = SystemAPI.GetComponentLookup<BallTag>(true),
            KillZoneLookup = SystemAPI.GetComponentLookup<KillZoneTag>(true),
            ECB = new EntityCommandBuffer(Allocator.TempJob)
        };
        // Schedule the job and complete dependencies
        state.Dependency = job.Schedule(sim, state.Dependency);
        state.Dependency.Complete();
        
        job.ECB.Playback(state.EntityManager);
        job.ECB.Dispose();
    }

    // Job to process destroying balls in kill zones
    [BurstCompile]
    struct BallKillZoneJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<BallTag> BallLookup;
        [ReadOnly] public ComponentLookup<KillZoneTag> KillZoneLookup;
        public EntityCommandBuffer ECB;

        
        public void Execute(TriggerEvent triggerEvent)
        {
            // Get the entities involved in the trigger event
            var a = triggerEvent.EntityA;
            var b = triggerEvent.EntityB;
            bool aIsBall = BallLookup.HasComponent(a);
            bool bIsBall = BallLookup.HasComponent(b);
            bool aIsKillZone = KillZoneLookup.HasComponent(a);
            bool bIsKillZone = KillZoneLookup.HasComponent(b);

            Entity ball = Entity.Null;

            // Check if one entity is a ball and the other is a kill zone
            if (aIsBall && bIsKillZone) ball = a;
            else if (bIsBall && aIsKillZone) ball = b;
            else return;
            ECB.DestroyEntity(ball);
        }
    }
}
