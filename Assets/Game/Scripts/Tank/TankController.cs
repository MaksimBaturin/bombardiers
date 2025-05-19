using UnityEngine;
using System;

namespace Game.Scripts
{
    public class TankController : MonoBehaviour
    {
        [SerializeField] private TankModel tank;

        public TankModel Tank {
            get { return tank; } 
            set 
            {
                if (IsScopeActive) IsScopeActive = false;
            
                tank = value;
                tankMovement = tank.TankMovement;
                tankGunRotator = tank.TankGunRotator;
                EnableAction = true;
                IsScopeActive = true;
            }
        }

        private TankMovement tankMovement;
        private TankGunRotator tankGunRotator;

        public Transform UITransform; 

        private bool isScopeActive = false;
        private bool IsScopeActive
        {
            get => isScopeActive;
            set
            {
                isScopeActive = value;
                if (value)
                {
                    scope = Instantiate(scopePrefab, UITransform);
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

        public static TankController Instance { get; private set; }

        private bool EnableAction = true;
        
        private bool IsPaused = false;
        private void Awake()
        {
            Instance = this;
        }
        private void Update()
        {
            if (EnableAction && !IsPaused)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    MoveTank(Vector2.right, tank.MovementSpeed);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    MoveTank(Vector2.left, tank.MovementSpeed);
                }

                if (Input.GetMouseButton(0))
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 direction = mousePos - (Vector2)tankGunRotator.pivot.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    tankGunRotator.RotateGun(angle);
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
                    newProjectile.owner = tank.gameObject;
                    newProjectile.LaunchAtAngle(tankGunRotator.gun.transform.rotation.eulerAngles.z, tank.MaxShootForce * tank.CurrentShootPower);
                    newProjectile.transform.parent = transform;
                    EnableAction = false;
                    IsScopeActive = false;
                }
            }
        }

        public void MoveTank(Vector2 direction, float speed)
        {
            if (tank.Fuel > 0)
            {
                tank.ConsumeFuel();
                tankMovement.DoMove(direction, speed);
            }
        }

        public void OnPause(bool pause)
        {
            IsPaused = pause;
            if (scope) scope.enabled = !pause;
        }

        private void onBulletDeath()
        {
            GameController.Instance.changePlayerTurn();
        }
        
    }
}