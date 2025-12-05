using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletBounce : MonoBehaviour
{
    public float speed = 20f;
    Rigidbody rb;

    // Bounce settings
    public float straightAngleThreshold = 5f; //Prevents straight up-down or left-right bounces
    public float maxRandomAngle = 5f; // max random angle to add to the reflection angle

    public static List<GameObject> balls = new List<GameObject>();

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
        if (balls.Count <= 0 && Player.Instance.shotsLeft<=0) GameManager.Instance.GameOver();


    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
            return;

        if (collision.transform.tag == "OutOfBounds") Destroy(gameObject);

        // Check if there are any contacts
        if (collision.contactCount == 0) return;

        // Create hit sparks on brick hit
        if (collision.gameObject.tag == "Brick")
        {
            Vector3 hitPos = collision.contacts[0].point;
            Quaternion hitRot = Quaternion.LookRotation(collision.contacts[0].normal);

            var fx = Instantiate(BrickManager.Instance.hitSparks, hitPos, hitRot);
            
            Destroy(fx, 0.5f);
        }

        // Get direction of ball
        Vector3 inDir = rb.linearVelocity;

        // Get collision contact
        Vector3 normal = collision.GetContact(0).normal;

        // Get reflection direction
        Vector3 reflectDir = Vector3.Reflect(inDir, normal);

        // Calculate angle between inDir and normal
        float angleToNormal = Vector3.Angle(inDir, normal);
        Debug.Log("Angle: " + (angleToNormal-180).ToString());
        // Checks if angle is close to 0 degrees (straight bounce) and if true then adds random angle to reflection
        if (Math.Abs(angleToNormal - 180) < straightAngleThreshold)
        {
            // Random angle between -maxRandomAngle and +maxRandomAngle
            float randomDelta = UnityEngine.Random.Range(-maxRandomAngle, maxRandomAngle);

            // Calculate new reflection direction with added random angle
            float baseAngle = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
            float newAngle = baseAngle + randomDelta;

            // Convert back to vector
            float rad = newAngle * Mathf.Deg2Rad;
            reflectDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;
        }

        // Set new velocity
        rb.linearVelocity = reflectDir;

    }
}
