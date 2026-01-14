using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Jumping : MonoBehaviour
{
    Rigidbody rb;
    public bool isOnGround = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnTriggerStay(Collider other)
    {
        isOnGround = true;
    }

    public void OnTriggerExit(Collider other)
    {
        isOnGround = false;
    }

    public void Jump (float jumpPower)
    {
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            rb.AddForce(Vector3.up * jumpPower);
        }
    }
}
