using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TankMovementRb : MonoBehaviour, IMoveable
    {
        [SerializeField] private Rigidbody2D rb;

        [SerializeField] private GameObject GroundCheckRight;
        [SerializeField] private GameObject GroundCheckLeft;

        [SerializeField] private GameObject AngleCheckRight;
        [SerializeField] private GameObject AngleCheckLeft;

        [SerializeField] private GameObject TurnedOverCheck;

        [SerializeField] private TankRotator tankRotator;

        [SerializeField] private float flipCooldown;
        private bool isCooldownActive = false;

        public float AllowedAngle;

        private void Update()
        {
            if (rb.transform.rotation.eulerAngles.z >= 90f || rb.transform.rotation.eulerAngles.z <= -90f)
            {
                if (IsTurnedOver())
                {
                    tankRotator.RotateToNormal();
                }
            }
        }

        public void FlipTank()
        {
            if (!isCooldownActive)
            {
                tankRotator.RotateToNormal();
                isCooldownActive = true;
                StartCoroutine(FlipCooldown());
            }
        }

        private IEnumerator FlipCooldown()
        {
            yield return new WaitForSeconds(flipCooldown);

            isCooldownActive = false;
        }

        public void DoMove(Vector2 direction, float speed)
        {
            if (IsOnGround() && CheckObstacleAngle(direction) <= AllowedAngle)
            {
                Vector2 offset = rb.transform.TransformDirection(direction) * speed * Time.deltaTime;
                rb.MovePosition(rb.position + offset);
            }
        }

        private bool IsOnGround()
        {
            RaycastHit2D hitRight;
            RaycastHit2D hitLeft;

            Debug.DrawRay(GroundCheckLeft.transform.position, GroundCheckLeft.transform.TransformDirection(Vector2.down) * 0.1f, Color.red);
            Debug.DrawRay(GroundCheckRight.transform.position, GroundCheckRight.transform.TransformDirection(Vector2.down) * 0.1f, Color.red);

            hitLeft = Physics2D.Raycast(GroundCheckLeft.transform.position, GroundCheckLeft.transform.TransformDirection(Vector2.down), 0.1f, LayerMask.GetMask("Ground"));
            hitRight = Physics2D.Raycast(GroundCheckRight.transform.position, GroundCheckRight.transform.TransformDirection(Vector2.down), 0.1f, LayerMask.GetMask("Ground"));

            return hitLeft.collider != null || hitRight.collider != null;
        }

        private float CheckObstacleAngle(Vector2 direction)
        {
            RaycastHit2D hit;
            Vector2 rayPos;
            if (direction == Vector2.left) rayPos = AngleCheckLeft.transform.position;
            else rayPos = AngleCheckRight.transform.position;

            Debug.DrawRay(rayPos, direction * 0.5f, Color.red);

            hit = Physics2D.Raycast(rayPos, direction, 0.5f, LayerMask.GetMask("Ground"));

            Debug.Log(Vector2.Angle(hit.normal, direction) - 90f);
            if (hit.collider) return Vector2.Angle(hit.normal, direction) - 90f;
            
            else return 0f;
        }

        private bool IsTurnedOver()
        {
            RaycastHit2D hit;
            Debug.DrawRay(TurnedOverCheck.transform.position, TurnedOverCheck.transform.TransformDirection(Vector2.up) * 1f, Color.red);
            hit = Physics2D.Raycast(TurnedOverCheck.transform.position, TurnedOverCheck.transform.TransformDirection(Vector2.up), 1f, LayerMask.GetMask("Ground"));
            return hit.collider != null;
        }
    }
}
