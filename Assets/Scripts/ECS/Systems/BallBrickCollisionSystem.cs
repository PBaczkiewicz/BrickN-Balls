using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
// System that handles collision events between balls and bricks
[BurstCompile]
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct BallBrickCollisionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var sim = SystemAPI.GetSingleton<SimulationSingleton>();

        // Set up the collision job with necessary component lookups
        var job = new BallBrickCollisionJob
        {
            BallTagLookup = SystemAPI.GetComponentLookup<BallTag>(true),
            BrickTagLookup = SystemAPI.GetComponentLookup<BrickTag>(true),
            BrickHitLookup = SystemAPI.GetComponentLookup<BrickHitEvent>(),
            BrickHealthLookup = SystemAPI.GetComponentLookup<BrickHealth>()
        };

        state.Dependency = job.Schedule(sim, state.Dependency);
    }

    // Job to process collision events
    [BurstCompile]
    struct BallBrickCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<BallTag> BallTagLookup;
        [ReadOnly] public ComponentLookup<BrickTag> BrickTagLookup;
        public ComponentLookup<BrickHitEvent> BrickHitLookup;
        public ComponentLookup<BrickHealth> BrickHealthLookup;

        public void Execute(CollisionEvent collisionEvent)
        {
            // Get the entities involved in the collision
            var a = collisionEvent.EntityA;
            var b = collisionEvent.EntityB;

            // Check if one entity is a ball and the other is a brick
            bool aIsBall = BallTagLookup.HasComponent(a);
            bool bIsBall = BallTagLookup.HasComponent(b);
            bool aIsBrick = BrickTagLookup.HasComponent(a);
            bool bIsBrick = BrickTagLookup.HasComponent(b);

            // If not a ball-brick collision, exit
            Entity brick = Entity.Null;
            if (aIsBall && bIsBrick) brick = b;
            else if (bIsBall && aIsBrick) brick = a;
            else return;

            // Update brick hit event and health
            if (BrickHitLookup.HasComponent(brick) && BrickHealthLookup.HasComponent(brick))
            {
                var ev = BrickHitLookup[brick];
                ev.HitsThisFrame += 1;
                BrickHitLookup[brick] = ev;

                var health = BrickHealthLookup[brick];
                health.Value -= 1;
                BrickHealthLookup[brick] = health;
            }
        }
    }
}
