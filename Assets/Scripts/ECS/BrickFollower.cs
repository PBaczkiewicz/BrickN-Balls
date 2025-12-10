using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

// Bridge between ECS Brick entity and Mono BrickScript
public class BrickFollower : MonoBehaviour
{
    public void Init(Entity entity)
    {
        // Initialize with the given entity
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null || !world.IsCreated)
            return;

        var em = world.EntityManager;

        if (!em.Exists(entity))
            return;

        // Set position and rotation of visual object based on LocalTransform component of ECS entity
        if (em.HasComponent<LocalTransform>(entity))
        {
            var lt = em.GetComponentData<LocalTransform>(entity);
            transform.position = lt.Position;
            transform.rotation = lt.Rotation;
        }
    }
}
