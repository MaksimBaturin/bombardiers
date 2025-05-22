using Game.Scripts;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Глобальный множитель сил")]
    public float forceScale = 2f;  // Корректнуть надо

    [Header("Физические параметры")]
    public float mass = 1f;
    public float gravityScale = 1f;
    public float dragCoefficient = 0.47f; // Коэффициент лобового сопротивления (Cd)
    public float crossSectionArea = 0.1f; // Площадь поперечного сечения (м²)

    [Header("Сила выстрела")]
    public float minForce = 1f;
    public float maxForce = 100f;

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

    public void LaunchAtAngle(float angleDegrees, float force)
    {
        float clampedForce = Mathf.Clamp(force, minForce, maxForce);
        float angleRad = angleDegrees * Mathf.Deg2Rad;

        
        externalForce = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * (clampedForce * forceScale);

        Vector2 initialVelocity = externalForce / mass;
        currentPosition = transform.position;
        previousPosition = currentPosition - initialVelocity * Time.fixedDeltaTime;

        isLaunched = true;

        //Debug.Log($"[Projectile] Запуск: сила={clampedForce}, масштаб={forceScale}, импульс={externalForce}");
    }

    private float GetAirDensity(float height)
    {
        // Экспоненциальное убывание плотности воздуха с высотой
        return AirDensitySeaLevel * Mathf.Exp(-height / ScaleHeight);
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
        Vector2 dragForce = -0.5f * dragCoefficient * airDensity * speed * velocity * crossSectionArea;
        //Debug.Log($"Сила сопротивления {dragForce}");
        // 3. Прикладная сила 
        Vector2 netForce = gravity + dragForce + windForce;
        Vector2 acceleration = netForce / mass;

        // Верле-интеграция
        Vector2 nextPosition = currentPosition + (currentPosition - previousPosition) + acceleration * Time.fixedDeltaTime * Time.fixedDeltaTime;

        previousPosition = currentPosition;
        currentPosition = nextPosition;

        rb.MovePosition(currentPosition);

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
