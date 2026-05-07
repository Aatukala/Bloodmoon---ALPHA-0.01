using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    [SerializeField] private Item weaponItem;

    private Camera cam;
    private float defaultFOV;
    private bool isAiming;

    private void Start()
    {
        cam = Camera.main;
        defaultFOV = cam.fieldOfView;
    }

    private void Update()
    {
        HandleAim();
    }

    void HandleAim()
    {
        if (weaponItem == null) return;

        // Right mouse = aim
        isAiming = Input.GetMouseButton(1);

        float targetFOV = isAiming ? weaponItem.aimFOV : defaultFOV;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            Time.deltaTime * weaponItem.aimSpeed
        );
    }
}