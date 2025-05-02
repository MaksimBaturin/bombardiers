using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.Scripts.UI
{
    public class Scope : MonoBehaviour
    {
        public TankGunRotator tankGunRotator;
        private RectTransform rectTransform;
        private GameObject gun;
        
        [SerializeField] private RectTransform pointer;
        private float initialPointerWidth;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            initialPointerWidth = pointer.sizeDelta.x;
        }

        private void Start()
        {
            gun = tankGunRotator.gun;

        }

        private void Update()
        {
            pointer.rotation = gun.transform.rotation;
            
            rectTransform.position = new Vector3(
                tankGunRotator.pivot.transform.position.x, 
                tankGunRotator.pivot.transform.position.y, 
                rectTransform.position.z
            );
            OnClick();
        }
        
        private void OnClick()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = transform.position.z;
                
                float distance = Vector3.Distance(transform.position, mouseWorldPos);
        
                float maxDistance = 27f; 
        
                float power = Mathf.Clamp01(distance / maxDistance);
                
                tankGunRotator.GetComponent<TankModel>().CurrentShootPower = power;
                
                RectTransform pointerRect = pointer.GetComponent<RectTransform>();
                pointerRect.sizeDelta = new Vector2(
                    initialPointerWidth * power, 
                    pointerRect.sizeDelta.y
                );
            }
        }
    }
}