using UnityEngine;

namespace Game.Scripts
{
    public class TankController : MonoBehaviour
    {
        public TankModel tank;

        private void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                tank.MoveTank(Vector2.right, tank.MovementSpeed);
            }else if (Input.GetKey(KeyCode.A))
            {
                tank.MoveTank(Vector2.left, tank.MovementSpeed);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = mousePos - (Vector2)tank.tankRotator.pivot.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                tank.tankRotator.RotateGun(angle);
            }
        }
    }
}