using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    public static BrickManager Instance;
    [Header("Brick Materials")]
    public Material colorGreen;
    public Material colorYellow;
    public Material colorRed;
    public Material colorGray;
    [Header("Hit Sparks Effect")]
    public GameObject hitSparks;

    [Header("Visual brick settings")]
    public GameObject brickVisualPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Spawns a visual brick GameObject that follows the given ECS brick entity
    public void SpawnVisualBrick(Entity brickEntity, float3 pos, quaternion rot)
    {
        if (brickVisualPrefab == null) return;

        var go = Instantiate(brickVisualPrefab, pos, rot);
        go.GetComponent<BrickBridge>().Init(brickEntity);
        var follower = go.GetComponent<BallFollower>();
        if (follower != null)
        {
            follower.Init(brickEntity);
        }
    }
}
