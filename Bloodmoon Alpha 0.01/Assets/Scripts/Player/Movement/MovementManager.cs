using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public float moveSpeed = 2;

    private Walking move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = GetComponent<Walking>();
    }

    // Update is called once per frame
    void Update()
    {
        move.Move(moveSpeed);
    }
}
