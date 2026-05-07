using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform headTarget;  // Head bone or CameraTarget
    public Transform playerBody;  // The player object to rotate horizontally

    [Header("Settings")]
    public float mouseSensitivity = 100f;
    public Vector3 offset = new Vector3(0f, 0.2f, -0.1f);
    public float followSmoothSpeed = 10f;
    public float aimSensitivityMultiplier = 0.5f;

    private float xRotation = 0f; // vertical rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleMouseLook();
        FollowHead();
    }

    void HandleMouseLook()
    {
        float currentSensitivity = mouseSensitivity;

        if (IsAiming())
            currentSensitivity *= aimSensitivityMultiplier;

        float mouseX = Input.GetAxis("Mouse X") * currentSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * currentSensitivity * Time.deltaTime;

        // Rotate player horizontally
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void FollowHead()
    {
        if (headTarget == null) return;

        // Position the camera relative to the head AND the player rotation
        Vector3 desiredPosition = headTarget.position + playerBody.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothSpeed * Time.deltaTime);
    }
    bool IsAiming()
    {
        if (!Input.GetMouseButton(1)) return false;

        Item equipped = PlayerHotbarController.Instance?.GetEquippedItem();
        if (equipped == null) return false;

        // Only apply to ranged weapons
        return equipped.weaponType == WeaponType.Gun || equipped.weaponType == WeaponType.Bow;
    }
}
