using DG.Tweening;
using UnityEngine;
using System.Collections;

namespace Game.Scripts
{
    public class TankRotator: MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;

        public void RotateToNormal()
        {
            StartCoroutine(DoRotate());
            transform.DOJump(new Vector3(rb.transform.position.x, rb.transform.position.y + 4f), 0.8f, 1, 0.6f);
        }

        private IEnumerator DoRotate()
        {
            yield return new WaitForSeconds(0.2f);
            transform.DORotate(Vector3.zero, 0.4f, RotateMode.FastBeyond360);
        }
    }
}