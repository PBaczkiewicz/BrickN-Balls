using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

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

    [Header("ECS ball settings")]
    public GameObject ballVisualPrefab; // prefab z EntityFollower

    EntityManager _em;
    Entity _ballPrefabEntity;
    bool _ecsReady;


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

        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        // pobierz prefab-encjê z singletona
        var q = _em.CreateEntityQuery(typeof(BallPrefabComponent));
        if (q.CalculateEntityCount() > 0)
        {
            var data = q.GetSingleton<BallPrefabComponent>();
            _ballPrefabEntity = data.Value;
            _ecsReady = true;
            Debug.Log("ECS ball prefab set.");

        }
        else
        {
            Debug.LogWarning("No BallPrefabComponent found – check BallSpawnerAuthoring.");
        }
        //InitEcs();

        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
    }

    //public void InitEcs()
    //{
    //    if (World.DefaultGameObjectInjectionWorld == null)
    //        return;

    //    _em = World.DefaultGameObjectInjectionWorld.EntityManager;

    //    var q = _em.CreateEntityQuery(typeof(BallPrefabComponent));
    //    int count = q.CalculateEntityCount();
    //    if (count == 0)
    //    {
    //        Debug.LogWarning("InitEcs: no BallPrefabComponent in world yet");
    //        return;
    //    }

    //    var data = q.GetSingleton<BallPrefabComponent>();
    //    _ballPrefabEntity = data.Value;
    //    _ecsReady = true;

    //    Debug.Log("InitEcs: ECS ball prefab set, ECS ready");
    //}

    // Initializes ammo and UI at the start of the game
    public void StartGame()
    {
        shotsLeft = maximumShots;
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


    void Shoot()
{
    if (shotsLeft <= 0) return;
    if (!_ecsReady) return;

    var ballEntity = _em.Instantiate(_ballPrefabEntity);

    float3 pos = muzzle.position;
    quaternion rot = muzzle.rotation;

    _em.SetComponentData(ballEntity,
        LocalTransform.FromPositionRotationScale(pos, rot, bulletPrefab.transform.localScale.x)); // skala 1, collider kontroluj w PhysicsShape

    if (_em.HasComponent<PhysicsVelocity>(ballEntity))
    {
        float3 dir = muzzle.forward;
        _em.SetComponentData(ballEntity, new PhysicsVelocity
        {
            Linear = dir * force,
            Angular = float3.zero
        });
    }

    var go = Instantiate(ballVisualPrefab, pos, rot);
    var follower = go.GetComponent<EntityFollower>();
    follower.Init(ballEntity);

    shotsLeft--;
    UIScript.Instance.ammoCounter.text = shotsLeft.ToString();
    if (shotsLeft <= 0)
        UIScript.Instance.ammoCounter.color = Color.red;
}




    //// Shoots a ball if ammo is available
    //void Shoot()
    //{
    //    if (shotsLeft <= 0) return;
    //    var bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
    //    bullet.GetComponent<BulletBounce>().sbl = new Vector3(bullet.transform.position.x, bullet.transform.position.y, bullet.transform.position.z);
    //    // Applies ball layer to avoid self-collision
    //    int bulletLayer = LayerMask.NameToLayer("Ball");
    //    bullet.layer = bulletLayer;

    //    // Applies force to the ball
    //    var rb = bullet.GetComponent<Rigidbody>();
    //    Vector3 dir = muzzle.forward;
    //    rb.linearVelocity = dir * force;
    //    shotsLeft--;

    //    // Update ammo UI
    //    GameManager.Instance.ammoCounter.text = shotsLeft.ToString();
    //    if (shotsLeft <= 0) GameManager.Instance.ammoCounter.color = Color.red;
    //}

}
