using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Глобальный множитель сил")]
    public float forceScale = 2f;  // Корректнуть надо

    [Header("Физические параметры")]
    public float mass = 1f;
    public float airResistance = 0.1f;
    public float gravityScale = 1f;

    [Header("Сила выстрела")]
    public float minForce = 1f;
    public float maxForce = 100f;

    private Vector2 currentPosition;
    private Vector2 previousPosition;
    private Vector2 externalForce;

    private bool isLaunched = false;
    private Rigidbody2D rb;

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

    protected virtual void FixedUpdate()
    {
        if (!isLaunched) return;

        Vector2 velocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;

        // 1. Сила притяжения
        Vector2 gravity = new Vector2(0f, -9.81f) * gravityScale * mass;

        // 2. Сила сопротивления 
        Vector2 airDrag = -airResistance * velocity;

        // 3. Прикладная сила 
        Vector2 netForce = gravity + airDrag;
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

        //Debug.Log($"[Projectile] Позиция: {currentPosition:F3}, Скорость: {velocity:F3}, Ускорение: {acceleration:F3}");
    }
    // Когда пуля сталкивается с любым объектом
    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }

    // Если нужно, используем физическое столкновение
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
