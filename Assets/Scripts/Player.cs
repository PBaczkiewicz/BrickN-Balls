using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int points = 0;
    public int maximumShots = 30;
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

    private void Start()
    {
        StartGame();
    }

    // Initializes ammo and UI at the start of the game
    public void StartGame()
    {
        if (!SceneManager.GetSceneByName("UIScene").isLoaded)
        {
            SceneManager.LoadScene("UIScene", LoadSceneMode.Single);
            return;
        }
        shotsLeft = maximumShots;

        GameManager.Instance.ammoPanel.SetActive(true);
        GameManager.Instance.ammoCounter.color = Color.white;
        GameManager.Instance.ammoCounter.text = shotsLeft.ToString();
    }
    void Update()
    {
        // Read look input
        if (lookAction != null)
        {
            Vector2 look = lookAction.action.ReadValue<Vector2>();
            inputX = look.x;
        }

        // Cannon rotation
        currentAngle += inputX * rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);
        turret.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

        // Fire action
        if (fireAction != null && fireAction.action.WasPerformedThisFrame())
        {
            Shoot();
        }
    }


    // Shoots a ball if ammo is available
    void Shoot()
    {
        if (shotsLeft <= 0) return;
        var bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
        bullet.GetComponent<BulletBounce>().sbl = new Vector3(bullet.transform.position.x, bullet.transform.position.y, bullet.transform.position.z);
        // Applies ball layer to avoid self-collision
        int bulletLayer = LayerMask.NameToLayer("Ball");
        bullet.layer = bulletLayer;

        // Applies force to the ball
        var rb = bullet.GetComponent<Rigidbody>();
        Vector3 dir = muzzle.forward;
        rb.linearVelocity = dir * force;
        shotsLeft--;

        // Update ammo UI
        GameManager.Instance.ammoCounter.text = shotsLeft.ToString();
        if (shotsLeft <= 0) GameManager.Instance.ammoCounter.color = Color.red;
    }

}
