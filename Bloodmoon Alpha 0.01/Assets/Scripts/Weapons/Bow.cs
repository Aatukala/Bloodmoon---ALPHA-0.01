using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private Item bowItemData;

    [Header("References")]
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float shootForce = 20f;
    [SerializeField] private float shootCooldown = 0.8f;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;


    private float lastShotTime;


    private PlayerInput input;

    private bool readytoshoot = true;

    private void Start()
    {
        input = GameObject.Find("Character").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (input.actions.FindAction("Attack").IsPressed() && readytoshoot)
        {
            TryShoot();
        }
        else if (!input.actions.FindAction("Attack").IsPressed())
        {
            readytoshoot = true;
        }
    }

    void TryShoot()
    {
        if (PauseMenu.IsPaused) return;
        if (Time.time < lastShotTime + shootCooldown)
            return;

        if (bowItemData == null || bowItemData.ammoType == null)
        {
            Debug.LogWarning("Bow or ammo missing!");
            return;
        }

        if (!ConsumeAmmo())
        {
            Debug.Log("No ammo!");
            return;
        }

        lastShotTime = Time.time;
        Shoot();
        readytoshoot = false;
    }

    void Shoot()
    {
        // Play bow sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // slight variation
            audioSource.PlayOneShot(shootSound);
        }

        GameObject arrow = Instantiate(
            bowItemData.ammoType.projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();

        if (projectile != null)
        {
            projectile.Launch(
                Camera.main.transform.forward,
                bowItemData.damage,
                bowItemData.knockbackForce
            );
        }
    }

    bool ConsumeAmmo()
    {
        return Inventory.Singleton.ConsumeItem(bowItemData.ammoType, 1);
    }
}