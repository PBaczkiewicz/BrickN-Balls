using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// Object tags
public struct BallTag : IComponentData { }
public struct BrickTag : IComponentData { }
public struct BallPrefabComponent : IComponentData { public Entity Value; }
public struct BallSpeed : IComponentData { public float Value; }
public struct KillZoneTag : IComponentData { }


// Components with data
//
// Spawner for bricks in a grid pattern
public struct BrickGridSpawner : IComponentData
{
    public Entity BrickPrefab;
    public float3 StartPos;
    public int Rows;
    public float XStep;
    public float YHalfOffset;
    public int EvenRowCount;
    public int OddRowCount;
}
// Records number of hits a brick has taken in the current frame
public struct BrickHitEvent : IComponentData
{
    public int HitsThisFrame;
}
// Health of a brick
public struct BrickHealth : IComponentData
{
    public int Value;
}
// Kill zone definition
public struct KillZone : IComponentData
{
    public float3 Center;
    public float3 HalfExtents;
}
