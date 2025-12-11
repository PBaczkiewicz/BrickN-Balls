using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BrickScript : MonoBehaviour
{
    int maxHp = 3;
    int currentHp = 3;

    BrickManager brickManager;

    [Header("Destruction Animation Settings")]
    public float fallTime = 2f;
    public float randomForce = 2f;
    public float randomTorque = 50f;

    private void Start()
    {
        currentHp = maxHp;
        brickManager = BrickManager.Instance;
        SetColor();
    }

    // Called when the brick is hit by a ball
    public void OnHit(int hits)
    {
        Player.Instance.points += hits;
        currentHp -= hits;
        SetColor();

        if (currentHp <= 0)
        {
            AnimateDestroy();
        }
        if (BrickManager.Instance.hitSparks != null)
        {
            var fx = Instantiate(BrickManager.Instance.hitSparks, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(fx, 0.5f);
        }
    }


    // Triggers destruction animation and removes brick from the scene
    public void AnimateDestroy()
    {
        SetColor(true);
        Rigidbody rb = GetComponent<Rigidbody>();
        Collider col = GetComponent<Collider>();
        rb.constraints = RigidbodyConstraints.None;

        // Disable collisions
        if (col != null)
            col.enabled = false;

        // Enable gravity and disable kinematic
        rb.useGravity = true;
        rb.isKinematic = false;

        // Randomize fall animation
        Vector3 dir = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-1.0f, -0.2f),
            Random.Range(0f, 0.3f)
        ).normalized;

        // Apply random linear and angular velocity
        rb.linearVelocity = dir * randomForce;
        rb.angularVelocity = Random.insideUnitSphere * randomTorque * Mathf.Deg2Rad;

        // Destroy after fallTime
        Destroy(gameObject, fallTime);
    }


    // Change material based on current HP
    void SetColor(bool destroyed = false)
    {
        if (destroyed)
        {
            Player.Instance.points++; // if forced destroy, add 1 point
            GetComponentInChildren<Renderer>().material = brickManager.colorGray;
            return;
        }
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