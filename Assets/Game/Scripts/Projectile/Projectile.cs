using Game.Scripts;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Глобальный множитель сил")]
    public float forceScale = 5f;  // Корректнуть надо

    [Header("Физические параметры")]
    public float mass = 1f;
    public float gravityScale = 0.05f;
    public float dragCoefficient = 0.05f; // Коэффициент лобового сопротивления (Cd)
    public float crossSectionArea = 0.01f; // Площадь поперечного сечения (м²)

    [Header("Сила выстрела")]
    public float minForce = 1f;
    public float maxForce = 1000f;

    [Header("Урон")]
    public float damage = 1;

    private Vector2 currentPosition;
    private Vector2 previousPosition;
    private Vector2 externalForce;

    private bool isLaunched = false;
    private Rigidbody2D rb;

    // Параметры атмосферы
    private const float AirDensitySeaLevel = 1.225f; // кг/м³ (плотность воздуха на уровне моря)
    private const float ScaleHeight = 8500f; // м (масштабная высота)


    public event Action onDeathEvent;

    public GameObject owner;

    protected virtual void Awake()
    {
        Time.fixedDeltaTime = 0.005f;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("[Projectile] Rigidbody2D не найден!");
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
    private float GetSafeMass()
    {
        return Mathf.Max(mass, 0.01f); // Не позволит массе быть ≤0
    }

    public void LaunchAtAngle(float angleDegrees, float force)
    {
        float safeMass = GetSafeMass();
        float clampedForce = Mathf.Clamp(force, minForce, maxForce);
        float angleRad = angleDegrees * Mathf.Deg2Rad;

        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        Vector2 initialVelocity = direction * (clampedForce * forceScale / safeMass);

        currentPosition = transform.position;
        previousPosition = currentPosition - initialVelocity * Mathf.Max(Time.fixedDeltaTime, 0.0001f);

        isLaunched = true;
    }

    private float GetAirDensity(float height)
    {
        float density = AirDensitySeaLevel * Mathf.Exp(-height / ScaleHeight);
        return Mathf.Max(density, 0.01f); // Минимальная плотность
    }

    protected virtual void FixedUpdate()
    {
        if (!isLaunched) return;

        Vector2 velocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;
        Vector2 windForce = WindController.GetWindForce();

        // 1. Сила притяжения
        Vector2 gravity = new Vector2(0f, -9.81f) * gravityScale * mass;

        // 2. Физически корректное сопротивление воздуха
        float height = currentPosition.y;
        float airDensity = GetAirDensity(height);
        float speed = velocity.magnitude;
        Vector2 dragForce;
        if (speed > 20f)
        {
            dragForce = -0.5f * dragCoefficient * airDensity * speed * speed * velocity.normalized * crossSectionArea;
        }
        else
        {
            dragForce = Vector2.zero;
        }
        //Debug.Log($"Сила сопротивления {dragForce}");
        // 3. Прикладная сила 
        Vector2 netForce = gravity + dragForce + windForce;
        Vector2 acceleration = netForce / mass;

        // Верле-интеграция
        Vector2 nextPosition = currentPosition + (currentPosition - previousPosition) + acceleration * Time.fixedDeltaTime * Time.fixedDeltaTime;

        previousPosition = currentPosition;
        currentPosition = nextPosition;

        rb.MovePosition(currentPosition);
        if (velocity.magnitude < 0.01f)
        {
            acceleration += new Vector2(0, -0.5f); // небольшой "пинок" вниз
        }
        // Поворот по направлению скорости
        if (velocity.sqrMagnitude > 0.0001f)
        {
            float angleDeg = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);
        }
    }
    // Когда пуля сталкивается с любым объектом
    private void OnTriggerEnter2D(Collider2D other)
    {
        IHealth obj;

        if (other.gameObject.TryGetComponent<IHealth>(out obj) && owner.gameObject != other.gameObject)
        {
            obj.TakeDamage(damage);
        }

        onDeathEvent.Invoke();
        Destroy(gameObject);
    }

    // Если нужно, используем физическое столкновение
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IHealth obj;
        
        Debug.Log(collision.gameObject.GetInstanceID()+" " + owner.gameObject.GetInstanceID());
        if (collision.gameObject.TryGetComponent<IHealth>(out obj))
        {
            obj.TakeDamage(damage);
        }
        onDeathEvent.Invoke();
        Destroy(gameObject);
    }
}
