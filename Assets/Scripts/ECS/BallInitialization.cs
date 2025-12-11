using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Properties;
using UnityEngine;

[BurstCompile]
public partial struct BallInitialization : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        // Set ball speed
        foreach (var (velocity, speed) in
                 SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<BallSpeed>>()
                          .WithAll<BallTag>())
        {
            float3 v = velocity.ValueRO.Linear;
            float lenSq = math.lengthsq(v);

            if (lenSq < 0.0001f) continue;

            float len = math.sqrt(lenSq);
            float3 dir = v / len;

            velocity.ValueRW.Linear = dir * speed.ValueRO.Value;
        }

    }

    public void OnDestroy(ref SystemState state) { }
}
