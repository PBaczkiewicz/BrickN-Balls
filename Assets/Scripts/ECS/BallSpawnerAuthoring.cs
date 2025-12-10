using Unity.Entities;
using UnityEngine;

public class BallSpawnerAuthoring : MonoBehaviour
{
    public GameObject ballAuthoringPrefab;

    class Baker : Baker<BallSpawnerAuthoring>
    {
        public override void Bake(BallSpawnerAuthoring authoring)
        {
            var ballPrefabEntity = GetEntity(authoring.ballAuthoringPrefab,
                                             TransformUsageFlags.Dynamic);

            // Create the spawner entity
            var spawnerEntity = GetEntity(TransformUsageFlags.None);

            // Add the BallSpawnerComponent to the spawner entity
            AddComponent(spawnerEntity, new BallPrefabComponent
            {
                Value = ballPrefabEntity
            });
        }
    }
}