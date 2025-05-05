using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Game.Scripts
{
    public class TankModel : MonoBehaviour, IHealth
    {
        [Header("Essentials")]
        public TankMovementRb tankMovement { get => tankMovement; private set => tankMovement = value; }
        public TankGunRotator tankGunRotator { get => tankGunRotator; private set => tankGunRotator = value; }
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

        public event Action OnDeath;

        //0 - 1
        public float CurrentShootPower;
        
        private float currentHealth;
        public float CurrentHealth { get => currentHealth; private set => currentHealth = value; }
        
        [Header("Projectiles")]
        [SerializeField] private List<Projectile> projectiles;

        [SerializeField] public Projectile selectedProjectile;

        [Header("Technical stats")] 
        [SerializeField] private float AllowedAngleToRide;

        public void Awake()
        {
            tankMovement.AllowedAngle = AllowedAngleToRide;
            currentHealth = maxHealth;
        }
        
        public void ConsumeFuel()
        {
            fuel -= fuelConsumption * Time.deltaTime;
            if (fuel <= 0) fuel = 0;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            Debug.Log($"damage: {damage}, health: {currentHealth}");
            if (currentHealth <= 0) Die();
        }

        public void Die()
        {
            
            Destroy(gameObject);
        }
        
        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }
    }
}