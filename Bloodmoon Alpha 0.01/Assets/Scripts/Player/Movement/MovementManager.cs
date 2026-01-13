using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public float moveSpeed = 2;
    public float speedMultiplier = 2.5f;

    private Walking move;
    private Running run;

    void Start()
    {
        move = GetComponent<Walking>();
        run = GetComponent<Running>();
    }

    void Update()
    {
        speedMultiplier = run.run();
        move.Move(moveSpeed * speedMultiplier);
    }
}
