using DG.Tweening;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] GameObject Gun;

    [SerializeField] float velocity;

    [SerializeField] float GunRotationSpeed;

    
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(velocity * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= new Vector3(velocity * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.E))
        {
            float rotationAngle = GunRotationSpeed * Time.deltaTime;


        }

    }
}
