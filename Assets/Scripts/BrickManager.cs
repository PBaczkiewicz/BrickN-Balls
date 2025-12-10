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

    [Header("Brick Spawning Settings")]
    public GameObject brickPrefab;
    public GameObject brickContainer;
    public Vector3 startPos = new Vector3(11.56f, 13f, -5.8f);
    public int rows = 12;
    public float xStep = 2.01f;
    public float yHalfOffset = 0.6f;

    public int evenRowCount = 12; // Number of bricks in even row
    public int oddRowCount = 11; // Number of bricks in odd row 

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
        var follower = go.GetComponent<EntityFollower>();
        if (follower != null)
        {
            follower.Init(brickEntity);
        }
    }

}
