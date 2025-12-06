using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class BulletBounce : MonoBehaviour
{
    public float speed = 20f;
    Rigidbody rb;

    // Bounce settings
    public float straightAngleThreshold = 5f; //Prevents straight up-down or left-right bounces
    public float maxRandomAngle = 10f; // max random angle to add to the reflection angle
    int lastBounceFrame = -1;
    public static List<GameObject> balls = new List<GameObject>();

    public Vector3 sbl;
    int bounces = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        speed = Player.Instance.force;
        balls.Add(this.gameObject);
    }

    void FixedUpdate()
    {
        // Setting speed
        if (rb.linearVelocity.sqrMagnitude > 0.0001f)
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    private void OnDestroy()
    {

        balls.Remove(this.gameObject);
        if (balls.Count <= 0 && Player.Instance.shotsLeft <= 0) GameManager.Instance.GameOver();


    }


    private void OnCollisionEnter(Collision collision)
    {
        if (Time.frameCount == lastBounceFrame)
            return;
        lastBounceFrame = Time.frameCount;

        // Ignore collisions with other balls
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
            return;

        if (collision.transform.tag == "OutOfBounds") Destroy(gameObject);

        Vector3 bl = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);

        //if (sbl.x - bl.x < straightAngleThreshold) bl.x += UnityEngine.Random.Range(-maxRandomAngle, maxRandomAngle);
        //if (sbl.y - bl.y < straightAngleThreshold) bl.y += UnityEngine.Random.Range(-maxRandomAngle, maxRandomAngle);

        Vector3 moveDir = new Vector3(sbl.x - bl.x, sbl.y - bl.y, sbl.z);
        string brickHit = "";
        if (collision.gameObject.tag == "Brick")
        {
            Vector3 n = collision.GetContact(0).normal;
            if (Mathf.Abs(n.y) > Mathf.Abs(n.x))
            {
                if (n.y > 0f)
                {
                    brickHit = "TopBottom";
                }
                else
                {
                    brickHit = "TopBottom";
                }
            }
            else
            {
                if (n.x > 0f)
                {
                    brickHit = "LeftRight";

                }
                else
                {
                    brickHit = "LeftRight";

                }
            }
        }
        

        if (collision.gameObject.tag == "TopWall" || collision.gameObject.tag == "BottomWall" || brickHit == "TopBottom") moveDir = new Vector3(-moveDir.x, moveDir.y, sbl.z);
        else if (collision.gameObject.tag == "LeftWall" || collision.gameObject.tag == "RightWall" || brickHit == "LeftRight") moveDir = new Vector3(moveDir.x, -moveDir.y, sbl.z);

        //Calculate angle between inDir and normal
        ContactPoint contact = collision.GetContact(0);
        var inDir = rb.linearVelocity;
        Vector3 normal = contact.normal;
        float angleToNormal = Vector3.Angle(inDir, normal);
        Debug.Log("Angle: " + (angleToNormal - 180).ToString());

        // Checks if angle is close to 0 degrees (straight bounce) and if true then adds random angle to reflection
        if (Math.Abs(angleToNormal - 180) < straightAngleThreshold)
        {
            // Random angle between -maxRandomAngle and +maxRandomAngle
            float randomDelta = UnityEngine.Random.Range(-maxRandomAngle, maxRandomAngle);

            // Calculate new reflection direction with added random angle
            float baseAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            float newAngle = baseAngle + randomDelta;

            // Convert back to vector
            float rad = newAngle * Mathf.Deg2Rad;
            moveDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;
        }

        rb.linearVelocity = moveDir;
        // zaktualizuj sbl na nowy punkt odbicia
        sbl = bl;

    }


    //void OnCollisionEnter(Collision collision)
    //{
    //    // Ignore collisions with other balls
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
    //        return;

    //    if (collision.transform.tag == "OutOfBounds") Destroy(gameObject);

    //    // Check if there are any contacts
    //    if (collision.contactCount == 0) return;

    //    // Create hit sparks on brick hit
    //    if (collision.gameObject.tag == "Brick")
    //    {
    //        Vector3 hitPos = collision.GetContact(0).point;
    //        Quaternion hitRot = Quaternion.LookRotation(collision.GetContact(0).normal);

    //        var fx = Instantiate(BrickManager.Instance.hitSparks, hitPos, hitRot);

    //        Destroy(fx, 0.5f);
    //    }

    //    // Get direction of ball
    //    Vector3 inDir = rb.linearVelocity;

    //    // Get collision contact
    //    ContactPoint contact = collision.GetContact(0);

    //    if (collision.gameObject.tag =="TopWall" || collision.gameObject.tag == "LeftWall" || collision.gameObject.tag == "RightWall")
    //    {
    //        Vector3 moveDirection = new Vector2(inDir.x, inDir.y);

    //        if (collision.gameObject.tag == "TopWall")
    //        {
    //            Debug.Log($"X={moveDirection.x} Y={moveDirection.y}");
    //            moveDirection = new Vector2(moveDirection.x, -moveDirection.y);
    //            Debug.Log($"X={moveDirection.x} Y={moveDirection.y}");
    //        }
    //        else if (collision.gameObject.tag == "LeftWall" || collision.gameObject.tag == "RightWall")
    //        {
    //            moveDirection = new Vector2(-moveDirection.x, moveDirection.y);
    //        }
    //        rb.linearVelocity = moveDirection;
    //    }
    //    else
    //    {
    //        // Translate contact normal to reflection direction
    //        Vector3 normal = contact.normal;
    //        Vector3 reflectDir = Vector3.Reflect(inDir, normal);

    //        // Calculate angle between inDir and normal
    //        float angleToNormal = Vector3.Angle(inDir, normal);
    //        Debug.Log("Angle: " + (angleToNormal - 180).ToString());

    //        // Checks if angle is close to 0 degrees (straight bounce) and if true then adds random angle to reflection
    //        if (Math.Abs(angleToNormal - 180) < straightAngleThreshold)
    //        {
    //            // Random angle between -maxRandomAngle and +maxRandomAngle
    //            float randomDelta = UnityEngine.Random.Range(-maxRandomAngle, maxRandomAngle);

    //            // Calculate new reflection direction with added random angle
    //            float baseAngle = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
    //            float newAngle = baseAngle + randomDelta;

    //            // Convert back to vector
    //            float rad = newAngle * Mathf.Deg2Rad;
    //            reflectDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;
    //        }
    //        bounces++;
    //        if (bounces > 50) Destroy(gameObject);
    //        // Set new velocity
    //        rb.linearVelocity = reflectDir;
    //    }
    //    // Push ball out of the surface to prevent sticking
    //    //float pushOutDistance = 0.1f;
    //    //transform.position = contact.point + contact.normal * pushOutDistance;


    //    Vector3 n = contact.normal;

    //}
}
