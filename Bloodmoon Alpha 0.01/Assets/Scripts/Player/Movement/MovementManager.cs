using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    public float moveSpeed = 2;
    public float speedMultiplier = 2.5f;
    public float sensitivity = 2;

    public float jumpPower = 250;

    private Walking move;
    private Running run;
    private Looking look;
    private Camera camera;
    private Jumping jump;

    void Start()
    {
        move = GetComponent<Walking>();
        jump = GetComponent<Jumping>();
        run = GetComponent<Running>();
        look = GetComponent<Looking>();
        camera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        jump.Jump(jumpPower);
        speedMultiplier = run.run();
        move.Move(moveSpeed * speedMultiplier);
        look.Look(sensitivity, camera.transform);
    }
}
