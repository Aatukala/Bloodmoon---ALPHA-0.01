using UnityEngine;
using System.Collections;

public class Walking : MonoBehaviour
{
    public void Move(float speed)
    {
        Debug.Log("Moving at speed: " + speed);
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("Before Moving Forward: " + transform.position);
            transform.position += Vector3.forward * speed * Time.deltaTime;
            Debug.Log("Walking Forward");
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -Vector3.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -Vector3.right * speed * Time.deltaTime;
        }
    }
}
