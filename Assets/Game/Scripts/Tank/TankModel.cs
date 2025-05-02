using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Game.Scripts
{
    public class TankModel : MonoBehaviour, IHealth
    {
        [Header("Essentials")]
        [SerializeField] private TankMovementRb tankMovement;
        public TankGunRotator tankGunRotator;
        public Transform firePoint;
        
        [Header("Stats")]
        [SerializeField] private float movementSpeed;
        public float MovementSpeed{ get => movementSpeed; private set => movementSpeed = value; }
        [SerializeField] private float health;
        public float Health{ get => health; private set => health = value; }
        [SerializeField] private float fuel;
        public float Fuel { get => fuel; private set => fuel = value; }
        [SerializeField] private float fuelConsumption;

        [SerializeField] private float maxHealth;

        [SerializeField] public float MaxShootForce;

        //0 - 1
        public float CurrentShootPower;
        
        private float currentHealth;
        public float CurrentHealth { get => currentHealth; private set => currentHealth = value; }
        
        [Header("Projectiles")]
        [SerializeField] private List<Projectile> projectiles;

        [SerializeField] public Projectile selectedProjectile;

        [Header("Technical stats")] 
        [SerializeField] private float AllowedAngleToRide;
        
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!IsScopeActive)
                {
                    IsScopeActive = true;
                }else IsScopeActive = false;
            }
        }

        public void Awake()
        {
            tankMovement.AllowedAngle = AllowedAngleToRide;
            currentHealth = maxHealth;
        }
        public void MoveTank(Vector2 direction, float speed)
        {
            fuel -= fuelConsumption * Time.deltaTime;
            if (fuel <= 0) fuel = 0;
            else tankMovement.DoMove(direction, speed);
        }

        public void FlipTank()
        {
            tankMovement.FlipTank();
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0) Die();
        }

        public void Die()
        {
            //TODO эффектики
            Destroy(gameObject);
        }
        
        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }
    }
}