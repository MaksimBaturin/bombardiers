using UnityEngine;
using System.Collections.Generic;
using Game.Scripts;
[System.Serializable]
public struct LaunchData
{
    public Vector2 Direction;     // Нормализованный вектор направления
    public float Force;          // Абсолютная сила выстрела
    public float CustomGravity;
    public bool UseWorldSpace;
}

public abstract class Projectile : MonoBehaviour
{
    [Header("Base Projectile Settings")]
    public float damage = 10f;
    public float explosionRadius = 1f;
    public float maxForce = 100f;          
    public float minForce = 1f;          
    public GameObject explosionEffect;
    public LayerMask collisionMask;

    [Header("Physics Settings")]
    public float mass = 1f;
    public float gravityScale = 1f;
    public float airResistance = 0.1f;
    public float bounceFactor = 0.3f;    // Коэффициент отскока
    public int maxBounces = 2;           // Максимальное количество отскоков

    [Header("Custom Physics Settings")]
    public Vector2 customGravity = new Vector2(0f, -9.81f); // Custom gravity vector
    public bool useCustomGravity = true; // Toggle between custom and Unity physics
    public float timeScale = 1f; // Time scale for physics simulation

    protected const float PHYSICS_UPDATE_FREQUENCY = 60f;
    protected float physicsTimeStep;
    protected float simulationTime;
    protected Vector2 currentPosition;
    protected Vector2 currentVelocity;
    protected int bounceCount = 0;
    protected bool isLaunched = false;
    protected List<Vector2> previousPositions = new List<Vector2>();
    protected const int MAX_POSITION_HISTORY = 10;

    [Header("Verlet Physics Settings")]
    public bool useVerletIntegration = true;
    protected Vector2 previousPosition; // For Verlet
    protected Vector2 currentAcceleration; // prev a

    protected virtual void Awake()
    {
        physicsTimeStep = 1f / PHYSICS_UPDATE_FREQUENCY;
        currentPosition = transform.position;
        currentVelocity = Vector2.zero;
        
        previousPosition = currentPosition - currentVelocity * physicsTimeStep;
        currentAcceleration = Vector2.zero;
    }

    #region Launch Methods
    public void LaunchAtAngle(float angleDegrees, float force)
    {
        force = Mathf.Clamp(force, minForce, maxForce);
        Vector2 direction = AngleToDirection(angleDegrees); 
        currentVelocity = direction * force;
        simulationTime = 0f;
        isLaunched = true;
        bounceCount = 0;
        previousPositions.Clear();
    }

    public void Launch(Vector2 direction, float force)
    {
        force = Mathf.Clamp(force, minForce, maxForce);
        currentVelocity = direction.normalized * force;
        simulationTime = 0f;
        isLaunched = true;
        bounceCount = 0;
        previousPositions.Clear();
    }

    public void Launch(LaunchData launchData)
    {
        Vector2 direction = launchData.Direction.normalized;
        float force = Mathf.Clamp(launchData.Force, minForce, maxForce);

        if (launchData.UseWorldSpace)
        {
            direction = (direction - (Vector2)transform.position).normalized;
        }

        currentVelocity = direction * force;

        if (launchData.CustomGravity != 0)
        {
            gravityScale = launchData.CustomGravity;
        }

        simulationTime = 0f;
        isLaunched = true;
        bounceCount = 0;
        previousPositions.Clear();
    }
    #endregion

    #region Custom Physics Implementation
    protected virtual void Update()
    {
        if (!isLaunched) return;

        // Store previous positions for collision detection
        if (previousPositions.Count >= MAX_POSITION_HISTORY)
        {
            previousPositions.RemoveAt(0);
        }
        previousPositions.Add(currentPosition);

        float deltaTime = Time.deltaTime * timeScale;
        while (deltaTime > 0f)
        {
            float step = Mathf.Min(deltaTime, physicsTimeStep);
            UpdatePhysics(step);
            deltaTime -= step;
        }

        transform.position = currentPosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, currentVelocity.normalized);
    }

    protected virtual void UpdatePhysics(float deltaTime)
    {
        // Условие должно учитывать метод интегрирования
        if ((!useVerletIntegration && currentVelocity.sqrMagnitude < 0.01f) ||
            (useVerletIntegration && (currentPosition - previousPosition).sqrMagnitude < 0.0001f))
        {
            return;
        }

        // Расчет сил (одинаков для всех методов)
        Vector2 gravityForce = useCustomGravity ?
            customGravity * mass * gravityScale :
            Physics2D.gravity * mass * gravityScale;

        Vector2 airResistanceForce = -airResistance * currentVelocity.sqrMagnitude * currentVelocity.normalized;
        currentAcceleration = (gravityForce + airResistanceForce) / mass;

        if (useVerletIntegration)
        {
            UpdateVerlet(deltaTime);
        }
        else
        {
            UpdateEuler(deltaTime);
        }

        simulationTime += deltaTime;
    }
    protected virtual void UpdateVerlet(float deltaTime)
    {
        // Сохраняем предыдущую позицию для следующего шага
        Vector2 tempPosition = currentPosition;

        // Основная формула Верле:
        // x(t+dt) = 2*x(t) - x(t-dt) + a(t)*dt²
        currentPosition = 2 * currentPosition - previousPosition + currentAcceleration * deltaTime * deltaTime;

        // Обновляем скорость (для корректного отображения и других расчетов)
        currentVelocity = (currentPosition - previousPosition) / deltaTime;

        // Сохраняем позицию для следующего шага
        previousPosition = tempPosition;

        // Проверка коллизий
        if (CheckCollision(previousPosition, currentPosition, out RaycastHit2D hit))
        {
            HandleCollision(hit);
        }
    }
    protected virtual void UpdateEuler(float deltaTime)
    {
        Vector2 newVelocity = currentVelocity + currentAcceleration * deltaTime;
        Vector2 newPosition = currentPosition + currentVelocity * deltaTime;

        if (CheckCollision(currentPosition, newPosition, out RaycastHit2D hit))
        {
            HandleCollision(hit);
        }
        else
        {
            currentVelocity = newVelocity;
            currentPosition = newPosition;
        }
    }

    protected bool CheckCollision(Vector2 from, Vector2 to, out RaycastHit2D hit)
    {
        // First check the immediate path
        hit = Physics2D.Linecast(from, to, collisionMask);
        if (hit.collider != null) return true;

        // If we have enough history, check for possible missed collisions
        if (previousPositions.Count > 1)
        {
            for (int i = 1; i < previousPositions.Count; i++)
            {
                Vector2 segmentStart = previousPositions[i - 1];
                Vector2 segmentEnd = previousPositions[i];

                // Check if we're moving fast enough to miss collisions
                if ((segmentEnd - segmentStart).sqrMagnitude > 0.25f)
                {
                    hit = Physics2D.Linecast(segmentStart, segmentEnd, collisionMask);
                    if (hit.collider != null) return true;
                }
            }
        }

        return false;
    }
    protected virtual void HandleCollision(RaycastHit2D hit)
    {
        if (bounceCount >= maxBounces)
        {
            Explode();
            Destroy(gameObject);
            return;
        }

        Vector2 reflectDirection = Vector2.Reflect(currentVelocity.normalized, hit.normal);
        float newSpeed = currentVelocity.magnitude * bounceFactor;

        if (useVerletIntegration)
        {
            // Для Верле нужно корректировать previousPosition
            previousPosition = currentPosition - reflectDirection * 0.01f;
        }

        currentVelocity = reflectDirection * newSpeed;
        currentPosition = hit.point + hit.normal * 0.01f;
        bounceCount++;
        OnBounce(hit);
    }

    protected virtual void OnBounce(RaycastHit2D hit)
    {
        // Bounce effect should add, maybe it would be Maksim or Asan
    }
    #endregion

    #region Utility Methods
    public Vector2 AngleToDirection(float angleDegrees)
    {
        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
    }

    public List<Vector2> PredictTrajectory(Vector2 startPosition, Vector2 direction, float force, int maxSteps = 1000)
    {
        List<Vector2> points = new List<Vector2>();
        Vector2 position = startPosition;
        Vector2 prevPos = position;
        Vector2 velocity = direction.normalized * Mathf.Clamp(force, minForce, maxForce);
        Vector2 acceleration = Vector2.zero;

        float simulatedTime = 0f;
        float maxSimulationTime = 10f;
        int simulatedBounces = 0;

        points.Add(position);

        while (simulatedTime < maxSimulationTime && points.Count < maxSteps && simulatedBounces <= maxBounces)
        {
            // Расчет сил
            Vector2 gravityForce = useCustomGravity ?
                customGravity * mass * gravityScale :
                Physics2D.gravity * mass * gravityScale;

            Vector2 airResistanceForce = -airResistance * velocity.sqrMagnitude * velocity.normalized;
            acceleration = (gravityForce + airResistanceForce) / mass;

            Vector2 newPosition;
            if (useVerletIntegration)
            {
                // Верле для предсказания
                Vector2 temp = position;
                newPosition = 2 * position - prevPos + acceleration * physicsTimeStep * physicsTimeStep;
                prevPos = temp;
                velocity = (newPosition - prevPos) / physicsTimeStep;
            }
            else
            {
                // Эйлер для предсказания
                velocity += acceleration * physicsTimeStep;
                newPosition = position + velocity * physicsTimeStep;
            }

            // Проверка столкновений
            RaycastHit2D hit = Physics2D.Linecast(position, newPosition, collisionMask);
            if (hit.collider != null)
            {
                if (simulatedBounces >= maxBounces)
                {
                    points.Add(hit.point);
                    break;
                }

                Vector2 reflectDirection = Vector2.Reflect(velocity.normalized, hit.normal);
                velocity = reflectDirection * velocity.magnitude * bounceFactor;
                newPosition = hit.point + hit.normal * 0.01f;
                prevPos = newPosition; // Важно для Верле после отскока
                simulatedBounces++;
            }

            position = newPosition;
            points.Add(position);
            simulatedTime += physicsTimeStep;
        }

        return points;
    }
    #endregion

    #region Collision and Effects
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if ((collisionMask.value & (1 << other.gameObject.layer)) != 0)
        {
            Explode();
            Destroy(gameObject);
        }
    }

    protected virtual void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(currentPosition, explosionRadius, collisionMask);

        foreach (var hit in hits)
        {
            IHealth health = hit.GetComponent<IHealth>();
            if (health != null)
            {
                float distance = Vector2.Distance(currentPosition, hit.transform.position);
                float damageFalloff = 1 - Mathf.Clamp01(distance / explosionRadius);
                health.TakeDamage(damage * damageFalloff);
            }
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, currentPosition, Quaternion.identity);
        }
    }
    #endregion
}