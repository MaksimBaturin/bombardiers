using UnityEngine;
namespace Game.Scripts
{
    public interface IMoveable
    {
        public void DoMove(Vector2 direction, float speed);
    }
}