using Unity.Entities;
using UnityEngine;

public class KillZoneTagAuthoring : MonoBehaviour
{
    class KillZoneTagBaker : Baker<KillZoneTagAuthoring>
    {
        public override void Bake(KillZoneTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<KillZoneTag>(entity);
        }
    }
}