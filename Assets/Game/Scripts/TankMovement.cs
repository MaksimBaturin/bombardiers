using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TankMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;

        public void MoveTank(Vector2 direction, float speed)
        {
            Vector2 offset = direction * speed * Time.deltaTime;
            rb.MovePosition(rb.position + offset);
            
        }
    }
}
