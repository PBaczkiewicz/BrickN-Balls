using Unity.Entities;
using UnityEngine;

// Bridge between ECS Brick entity and Mono BrickScript
public class BrickBridge : MonoBehaviour
{
    EntityManager _em;
    Entity _entity;
    bool _hasEntity;
    BrickScript brickScript;

    public void Init(Entity entity)
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        _entity = entity;
        _hasEntity = true;
        brickScript = GetComponent<BrickScript>();
    }

    void Update()
    {
        // If entity stoppes existing, trigger destroy animation on visual object
        if (!_hasEntity || !_em.Exists(_entity))
        {
            if (_hasEntity)
                brickScript.AnimateDestroy();
            _hasEntity = false;
            return;
        }


        // Check for BrickHitEvent and call OnHit on BrickScript
        if (_em.HasComponent<BrickHitEvent>(_entity))
        {
            var ev = _em.GetComponentData<BrickHitEvent>(_entity);
            if (ev.HitsThisFrame > 0)
            {
                brickScript.OnHit(ev.HitsThisFrame);
                ev.HitsThisFrame = 0;
                _em.SetComponentData(_entity, ev);
            }
        }
    }
}
