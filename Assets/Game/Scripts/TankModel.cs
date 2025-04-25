using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class TankModel : MonoBehaviour
    {
        [Header("Essentials")]
        public IMoveable tankMovement;
        public TankGunRotator tankRotator;
        
        [Header("Stats")]
        [SerializeField] private float movementSpeed;
        public float MovementSpeed{ get => movementSpeed; private set => movementSpeed = value; }
        [SerializeField] private float health;
        public float Health{ get => health; private set => health = value; }
        [SerializeField] private float fuel;
        public float Fuel { get => fuel; private set => fuel = value; }
        [SerializeField] private float fuelConsumption;
        
        [Header("Bullet list")]
        [SerializeField] private List<GameObject> bullets;

        public void Awake()
        {
            tankMovement = GetComponent<IMoveable>();
        }
        public void MoveTank(Vector2 direction, float speed)
        {
            fuel -= fuelConsumption * Time.deltaTime;
            if (fuel <= 0) fuel = 0;
            else tankMovement.DoMove(direction, speed);
        }
    }
}