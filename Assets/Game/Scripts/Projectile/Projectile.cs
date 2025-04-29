using UnityEngine;
using System.Collections.Generic;

public abstract class Projectile : MonoBehaviour
{
    [Header("Base Projectile Settings")]
    public float damage = 10f;
    public float explosionRadius = 1f;
    public float initialSpeed = 10f;
    public GameObject explosionEffect;
    public float mass = 1f;
    public float gravityScale = 1f;
    public float airResistance = 0.1f;

    protected Rigidbody2D rb;
    protected Vector2 initialVelocity;
    protected float simulationTime;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.gravityScale = 1; 
    }

    public virtual void Launch(Vector2 direction, float forceMultiplier = 1f)
    {
        initialVelocity = direction.normalized * initialSpeed * forceMultiplier;
        rb.linearVelocity = initialVelocity;
        simulationTime = 0f;
    }

    protected virtual void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            
            Vector2 gravityForce = Physics2D.gravity * rb.mass * gravityScale;

            Vector2 airResistanceForce = -airResistance * rb.linearVelocity.sqrMagnitude * rb.linearVelocity.normalized;

            rb.AddForce(gravityForce + airResistanceForce);

            simulationTime += Time.fixedDeltaTime;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
        Destroy(gameObject);
    }

    protected virtual void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                float damageFalloff = 1 - Mathf.Clamp01(distance / explosionRadius);
                health.TakeDamage(damage * damageFalloff);
            }
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }

    //Траектория
    public List<Vector2> PredictTrajectory(Vector2 launchPosition, Vector2 launchDirection, float forceMultiplier = 1f, int maxSteps = 1000)
    {
        List<Vector2> points = new List<Vector2>();
        Vector2 position = launchPosition;
        Vector2 velocity = launchDirection.normalized * initialSpeed * forceMultiplier;
        float timeStep = 0.02f;
        float simulatedTime = 0f;
        float maxSimulationTime = 10f;

        while (simulatedTime < maxSimulationTime && points.Count < maxSteps)
        {
            points.Add(position);

            Vector2 gravityForce = Physics2D.gravity * mass * gravityScale;

            Vector2 airResistanceForce = -airResistance * velocity.sqrMagnitude * velocity.normalized;

            Vector2 acceleration = (gravityForce + airResistanceForce) / mass;

            velocity += acceleration * timeStep;
            position += velocity * timeStep;

            simulatedTime += timeStep;

            if (position.y <= launchPosition.y) //Приземление
                break;
        }

        return points;
    }
}