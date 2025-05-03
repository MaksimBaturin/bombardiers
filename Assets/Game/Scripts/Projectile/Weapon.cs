using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Projectile projectilePrefab; 
    public Transform firePoint;

    public void FireAtAngle(float angleDegrees, float force)
    {
        if (!projectilePrefab || !firePoint)
        {
            Debug.LogError("�� ��������� ������ ������� ��� ����� ��������!");
            return;
        }

        Debug.Log($"[Weapon] �������! ����: {angleDegrees}, ����: {force}");

        Projectile newProjectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Debug.Log($"[Weapon] ������ ������ �� ������� {firePoint.position}");

        newProjectile.LaunchAtAngle(angleDegrees, force);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            float angle = 45f;  
            float force = 15f;  

            FireAtAngle(angle, force);
        }
    }
}