using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    public int points = 0;
    public int shotsLeft = 30;

    public InputActionReference lookAction;
    public InputActionReference fireAction;
    public static Player Instance;

    [Header("Turret settings")]
    public Transform turret;
    public float rotationSpeed = 60f;
    public float maxAngle = 80f;

    private float currentAngle = 0f;
    private float inputX;

    [Header("Ammo settings")]
    public Transform muzzle;
    public GameObject bulletPrefab;
    public float force = 20f;

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

    void Update()
    {  
        // Odczyt osi z Look (x z Vector2)
        if (lookAction != null)
        {
            Vector2 look = lookAction.action.ReadValue<Vector2>();
            inputX = look.x;
        }

        // Obrót działa
        currentAngle += inputX * rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);
        turret.localRotation = Quaternion.Euler(0f,0f, currentAngle);

        // Strzał – przycisk Shoot
        if (fireAction != null && fireAction.action.WasPerformedThisFrame())
        {
            Shoot();
        }
    }
    void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
        var rb = bullet.GetComponent<Rigidbody>();
        Vector3 dir = muzzle.forward;
        rb.linearVelocity = dir * force; 
    }

}
