using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

// System that spawns a grid of bricks based on the BrickGridSpawner component
[BurstCompile]
public partial struct BrickGridSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BrickGridSpawner>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        var spawnerQuery = SystemAPI.QueryBuilder().WithAll<BrickGridSpawner>().Build();

        if (spawnerQuery.CalculateEntityCount() == 0) return;

        var spawnerEntity = spawnerQuery.GetSingletonEntity();
        var data = em.GetComponentData<BrickGridSpawner>(spawnerEntity);

        float currentY = data.StartPos.y;
        float z = data.StartPos.z;

        // Spawn bricks in a grid pattern
        for (int pair = 0; pair < data.Rows / 2; pair++)
        {
            SpawnRow(ref state, data.BrickPrefab, data.EvenRowCount,
                     new float2(data.StartPos.x, currentY),
                     data.XStep, z);

            float shiftedY = currentY - data.YHalfOffset;
            float shiftedX = data.StartPos.x + data.XStep * 0.5f;

            SpawnRow(ref state, data.BrickPrefab, data.OddRowCount,
                     new float2(shiftedX, shiftedY),
                     data.XStep, z);

            currentY -= 2f * data.YHalfOffset;
        }

        // Delete the spawner entity after spawning
        em.DestroyEntity(spawnerEntity);
    }

    // Spawns a single row of bricks
    void SpawnRow(ref SystemState state,
                  Entity brickPrefab, int count,
                  float2 rowStart, float xStep, float z)
    {
        var em = state.EntityManager;

        for (int i = 0; i < count; i++)
        {
            float x = rowStart.x + i * xStep;
            float y = rowStart.y;
            float3 pos = new float3(x, y, z);

            var brick = em.Instantiate(brickPrefab);
            // Set brick position based on prefab's LocalTransform
            if (em.HasComponent<LocalTransform>(brickPrefab))
            {
                var prefabLt = em.GetComponentData<LocalTransform>(brickPrefab);
                var lt = LocalTransform.FromPositionRotationScale(
                    pos,
                    prefabLt.Rotation,
                    prefabLt.Scale
                );
                em.SetComponentData(brick, lt);

                // Spawn visual brick
                BrickManager.Instance?.SpawnVisualBrick(
                    brick,
                    pos,
                    prefabLt.Rotation
                );
            }
        }
    }


}
