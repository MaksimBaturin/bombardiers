using UnityEngine;
using System;

namespace Game.Scripts
{
    public class TankController : MonoBehaviour
    {
        private TankModel tank;

        public TankModel Tank {
            get { return tank; } 
            set 
            {
                if (IsScopeActive) IsScopeActive = false;
            
                tank = value;
                tankMovement = tank.tankMovement;
                tankGunRotator = tank.tankGunRotator;
                tank.OnDeath += onTankDeath;

                IsScopeActive = true;
            }
        }

        private TankMovementRb tankMovement;
        private TankGunRotator tankGunRotator;


        private bool isScopeActive = false;
        private bool IsScopeActive
        {
            get => isScopeActive;
            set
            {
                isScopeActive = value;
                if (value)
                {
                    scope = Instantiate(scopePrefab, GameObject.Find("InGame UI Canvas").transform);
                    scope.tankGunRotator = tankGunRotator;
                }
                else
                {
                    if (scope)
                    {
                        Destroy(scope.gameObject);
                        scope = null;
                    }
                }
            }
        }

        [Header("Prefabs")]
        [SerializeField] private UI.Scope scopePrefab;
        private UI.Scope scope;


        public event Action OnTurnEnd;

        private void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                MoveTank(Vector2.right, tank.MovementSpeed);
            }else if (Input.GetKey(KeyCode.A))
            {
                MoveTank(Vector2.left, tank.MovementSpeed);
            }
            
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = mousePos - (Vector2)tank.tankGunRotator.pivot.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                tank.tankGunRotator.RotateGun(angle);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                tankMovement.FlipTank();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Projectile newProjectile = Instantiate(
                    tank.selectedProjectile,
                    tank.firePoint.position,
                    Quaternion.identity
                );
                newProjectile.onDeathEvent += onBulletDeath;
                newProjectile.LaunchAtAngle(tank.tankGunRotator.gun.transform.rotation.eulerAngles.z, tank.MaxShootForce * tank.CurrentShootPower);
            }

        }

        public void MoveTank(Vector2 direction, float speed)
        {
            if (tank.Fuel > 0) tankMovement.DoMove(direction, speed);
        }

        public void FlipTank()
        {
            tankMovement.FlipTank();
        }

        private void onBulletDeath()
        {
            OnTurnEnd.Invoke();
        }

        private void onTankDeath()
        {
            return;
        }
    }
}