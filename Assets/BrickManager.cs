using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    public Material colorGreen;
    public Material colorYellow;
    public Material colorRed;
    public Material colorGray;

    public static BrickManager Instance;

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

}
