using UnityEngine;

namespace Game.Scripts
{
    public class TankGunRotator : MonoBehaviour
    {
        [SerializeField] public GameObject gun;
        [SerializeField] public Transform pivot;
        
        public float currentAngle;
        
        public void RotateGun(float rotateAngle)
        {
            currentAngle = gun.transform.eulerAngles.z;
            float angleDifference = Mathf.DeltaAngle(currentAngle, rotateAngle);
            
            gun.transform.RotateAround(pivot.position, Vector3.forward, angleDifference);
        }
    }
}