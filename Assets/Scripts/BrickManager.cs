using UnityEditor.U2D.Aseprite;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.Rendering;

public class BrickManager : MonoBehaviour
{
    public Material colorGreen;
    public Material colorYellow;
    public Material colorRed;
    public Material colorGray;
    public static BrickManager Instance;
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

    private void Start()
    {
        SpawnBricks();
    }

    // Spawns brick in game area
    void SpawnBricks()
    {
        // Y vector decreasing after each line
        float currentY = startPos.y;

        for (int pair = 0; pair < rows / 2; pair++)
        {
            // Even row spawning
            SpawnRow(evenRowCount, new Vector3(startPos.x, currentY, startPos.z));

            float shiftedY = currentY - yHalfOffset;
            float shiftedX = startPos.x + xStep / 2f; // Half offset for odd rows

            // Odd row spawning
            SpawnRow(oddRowCount, new Vector3(shiftedX, shiftedY, startPos.z));

            // Next pair of rows
            currentY -= 2f * yHalfOffset;
        }
    }

    void SpawnRow(int count, Vector2 rowStart)
    {
        // Spawning bricks in a single row
        for (int i = 0; i < count; i++)
        {
            float x = rowStart.x + i * xStep;
            Vector3 pos = new Vector3(x, rowStart.y, startPos.z);
            Instantiate(brickPrefab, pos, Quaternion.identity, brickContainer.transform);
        }
    }

}
