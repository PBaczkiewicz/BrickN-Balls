using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletBounce : MonoBehaviour
{
    public float speed = 20f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        speed = Player.Instance.force;
    }

    void Start()
    {
        // Upewnij siê, ¿e startowa prêdkoœæ ma w³aœciw¹ d³ugoœæ
        if (rb.linearVelocity.sqrMagnitude > 0.0001f)
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    void FixedUpdate()
    {
        // utrzymuj sta³¹ prêdkoœæ w 2D
    Vector3 v = rb.linearVelocity;
    v.z = 0f;

        // Tylko pilnuj d³ugoœci wektora
        if (rb.linearVelocity.sqrMagnitude > 0.0001f)
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount == 0) return;

        Vector3 inDir = rb.linearVelocity;
        if (inDir.sqrMagnitude < 0.0001f)
            return;

        inDir = inDir.normalized;

        Vector3 normal = collision.contacts[0].normal;

        // Dla œcian w osi X (lewa/prawa) i Y (górna)
        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        {
            // pionowe œciany: normalny wektor tylko w X
            normal = new Vector3(Mathf.Sign(normal.x), 0f, 0f);
        }
        else
        {
            // górna œciana: normalny wektor tylko w Y
            normal = new Vector3(0f, Mathf.Sign(normal.y), 0f);
        }

        Vector3 reflectDir = Vector3.Reflect(inDir, normal);
        rb.linearVelocity = reflectDir * speed;

        Debug.Log(
    $"wall={collision.collider.name}, in={inDir}, rawN={collision.contacts[0].normal}, snapN={normal}, out={reflectDir}"
);
    }
}
