using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

// System that destroys bricks when their health reaches zero
[BurstCompile]
public partial struct BrickDestroySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Create an EntityCommandBuffer to record entity destruction commands
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Query all bricks with their health
        foreach (var (health, entity) in
                 SystemAPI.Query<RefRO<BrickHealth>>()
                          .WithAll<BrickTag>()
                          .WithEntityAccess())
        {
            // Destroy the brick if its health is zero or below
            if (health.ValueRO.Value <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }

        // Playback the recorded commands
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
