using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Image healthBarFill;
    public GameObject gameOverPanel; 

    void Start()
    {
        Time.timeScale = 1f;
        currentHealth = maxHealth;
        UpdateHealthBar();

        // The Game Over panel must start hidden; otherwise it can appear as a thin UI line in Game view.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    void Die()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        KillCounter killCounter = FindObjectOfType<KillCounter>();
        if (killCounter != null)
        {
            killCounter.CheckHighScore();
        }

    }
}
