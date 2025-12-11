using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using NUnit.Framework;

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

    [Header("ECS ball settings")]
    public GameObject ballVisualPrefab;

    EntityManager _em;
    Entity _ballPrefabEntity;
    bool _ecsReady;
    public int ballsInPlay = 0;
    private EntityQuery _ballPrefabQuery;

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
        _ballPrefabQuery = _em.CreateEntityQuery(typeof(BallPrefabComponent));

        StartCoroutine(WaitForEcsAndBind());

        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
    }

    private System.Collections.IEnumerator WaitForEcsAndBind()
    {
        while (_ballPrefabQuery.IsEmptyIgnoreFilter) yield return null;

        var data = _ballPrefabQuery.GetSingleton<BallPrefabComponent>();
        _ballPrefabEntity = data.Value;
        _ecsReady = true;
        Debug.Log("ECS ball prefab set (async).");
    }


    // Reset shots and balls in play
    public void StartGame()
    {
        shotsLeft = maximumShots;
        ballsInPlay = 0;
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

        _em.SetComponentData(ballEntity, LocalTransform.FromPositionRotationScale(pos, rot, bulletPrefab.transform.localScale.x));

        // Set ball flight direction
        if (_em.HasComponent<PhysicsVelocity>(ballEntity))
        {
            float3 dir = muzzle.forward;
            _em.SetComponentData(ballEntity, new PhysicsVelocity
            {
                Linear = dir,
                Angular = float3.zero
            });
        }

        // Spawn visual ball that follows the ECS ball entity
        var go = Instantiate(ballVisualPrefab, pos, rot);
        var follower = go.GetComponent<BallFollower>();
        follower.Init(ballEntity);

        // Update ammo count and balls in play
        ballsInPlay++;
        shotsLeft--;
        UIScript.Instance.ammoCounter.text = shotsLeft.ToString();
        if (shotsLeft <= 0) UIScript.Instance.ammoCounter.color = Color.red;
    }
}
