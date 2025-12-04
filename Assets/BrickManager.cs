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

    [Header("Brick Spawning Settings")]
    public GameObject brickPrefab;
    public GameObject brickContainer;
    public Vector3 startPos = new Vector3(11.56f, 13f, -5.8f);
    public int rows = 12;
    public float xStep = 2.01f;
    public float yHalfOffset = 0.6f;

    public int fullRowCount = 12; // Number of bricks in a full row
    public int secondRowCount = 11;

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
    void SpawnBricks()
    {
        // Y vector decreasing after each line
        float currentY = startPos.y;

        for (int pair = 0; pair < rows / 2; pair++)
        {
            // 1) Rz¹d pe³ny (11 cegie³), od startPos.x w prawo
            SpawnRow(fullRowCount, new Vector3(startPos.x, currentY, startPos.z));

            // 2) Rz¹d przesuniêty w dó³ o 0.6 i X -1.005, z jedn¹ ceg³¹ mniej
            float shiftedY = currentY - yHalfOffset;
            float shiftedX = startPos.x + xStep / 2f; // 1.005 przy xStep=2.01f
            SpawnRow(secondRowCount, new Vector3(shiftedX, shiftedY, startPos.z));

            // Nastêpna para rzêdów ni¿ej o kolejne 1.2 (2 * 0.6)
            currentY -= 2f * yHalfOffset;
        }
    }

    void SpawnRow(int count, Vector2 rowStart)
    {
        for (int i = 0; i < count; i++)
        {
            float x = rowStart.x + i * xStep;
            Vector3 pos = new Vector3(x, rowStart.y, startPos.z);
            Instantiate(brickPrefab, pos, Quaternion.identity, brickContainer.transform);
        }
    }

}
