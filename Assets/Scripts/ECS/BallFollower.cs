using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

// Bridge between ECS ball and MonoBehaviour to follow its position and rotation
public class BallFollower : MonoBehaviour
{
    public Entity EntityToFollow;
    bool _initialized;

    public void Init(Entity e)
    {
        EntityToFollow = e;
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized) return;

        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null || !world.IsCreated) return;

        var em = world.EntityManager;

        // If the entity no longer exists, destroy this GameObject
        if (!em.Exists(EntityToFollow))
        {
            Destroy(gameObject);
            _initialized = false;
            Player.Instance.ballsInPlay--;

            // Check for game over
            if (Player.Instance.ballsInPlay <= 0 && Player.Instance.shotsLeft <= 0)
            {
                UIScript.Instance.GameOver();
            }
            return;
        }
        // Update position and rotation based on LocalTransform component
        var lt = em.GetComponentData<LocalTransform>(EntityToFollow);
        transform.position = lt.Position;
        transform.rotation = lt.Rotation;
    }
}