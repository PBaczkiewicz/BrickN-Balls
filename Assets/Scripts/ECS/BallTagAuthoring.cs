using Unity.Entities;
using UnityEngine;

public class BallTagAuthoring : MonoBehaviour
{
    public float ballSpeed = 20f;
    class BallTagBaker : Baker<BallTagAuthoring>
    {
        public override void Bake(BallTagAuthoring authoring)
        {
            UnityEngine.Debug.Log($"BAKE BallTag for {authoring.gameObject.name}");
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BallTag>(entity);
            AddComponent(entity, new BallSpeed { Value = authoring.ballSpeed });
        }
    }
}
