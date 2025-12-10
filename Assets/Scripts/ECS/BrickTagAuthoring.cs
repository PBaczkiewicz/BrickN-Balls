using Unity.Entities;
using UnityEngine;

public class BrickTagAuthoring : MonoBehaviour
{
    class BrickTagBaker : Baker<BrickTagAuthoring>
    {
        public override void Bake(BrickTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BrickTag>(entity);
            AddComponent<BrickHitEvent>(entity);
            AddComponent(entity, new BrickHealth { Value = 3 });
        }
    }
}
