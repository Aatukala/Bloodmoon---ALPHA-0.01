using UnityEngine;

public class DeathUIManager : MonoBehaviour
{
    public GameObject deathScreen;

    void OnEnable()
    {
        IDamageable.OnPlayerDeath += ShowDeathScreen;
    }

    void OnDisable()
    {
        IDamageable.OnPlayerDeath -= ShowDeathScreen;
    }

    void ShowDeathScreen()
    {
        deathScreen.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadGame()
    {
        deathScreen.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SaveSystem.Load();
    }
}