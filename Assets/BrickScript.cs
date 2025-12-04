using UnityEngine;

public class BrickScript : MonoBehaviour
{
    int maxHp = 3;
    int currentHp = 3;

    BrickManager brickManager;

    private void Start()
    {
        currentHp = maxHp;
        brickManager = BrickManager.Instance;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            currentHp--;
            SetColor();
            if (currentHp <= 0)
            {
                AnimateDestroy();
            }
        }
    }

    void AnimateDestroy()
    {

        Destroy(gameObject);
    }

    void SetColor()
    {
        switch (currentHp)
        {
            case 3:
                GetComponentInChildren<Renderer>().material = brickManager.colorRed;
                break;
            case 2:
                GetComponentInChildren<Renderer>().material = brickManager.colorYellow;
                break;
            case 1:
                GetComponentInChildren<Renderer>().material = brickManager.colorGreen;
                break;
            default:
                GetComponentInChildren<Renderer>().material = brickManager.colorGray;
                break;

        }
    }
}