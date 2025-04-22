using UnityEngine;

namespace Game.Scripts
{
    public class TankGunRotator : MonoBehaviour
    {
        [SerializeField] private GameObject gun;
        [SerializeField] public Transform pivot;

        public void RotateGun(float rotateAngle)
        {
            float currentAngle = gun.transform.eulerAngles.z;
            float angleDifference = Mathf.DeltaAngle(currentAngle, rotateAngle);
            
            gun.transform.RotateAround(pivot.position, Vector3.forward, angleDifference);
        }
    }
}