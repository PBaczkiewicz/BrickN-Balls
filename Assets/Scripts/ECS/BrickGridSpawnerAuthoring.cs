using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BrickGridSpawnerAuthoring : MonoBehaviour
{
    [Header("Brick spawning settings")]
    public GameObject brickPrefab;

    [Header("Brick grid settings")]
    public Vector3 startPos = new Vector3(-10.45f, 13f, -5.8f);
    public int rows = 12;
    public float xStep = 2.01f;
    public float yHalfOffset = 0.6f;
    public int evenRowCount = 12;
    public int oddRowCount = 11;

    class Baker : Baker<BrickGridSpawnerAuthoring>
    {
        public override void Bake(BrickGridSpawnerAuthoring authoring)
        {
            if (authoring.brickPrefab == null)
            {
                Debug.LogWarning("BrickGridSpawnerAuthoring: brickPrefab is NULL");
                return;
            }

            var entity = GetEntity(TransformUsageFlags.None);
            var brickPrefabE = GetEntity(authoring.brickPrefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new BrickGridSpawner
            {
                BrickPrefab = brickPrefabE,
                StartPos = authoring.startPos,
                Rows = authoring.rows,
                XStep = authoring.xStep,
                YHalfOffset = authoring.yHalfOffset,
                EvenRowCount = authoring.evenRowCount,
                OddRowCount = authoring.oddRowCount
            });
        }
    }
}
