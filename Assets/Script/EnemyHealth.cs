using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject healthBarPrefab;
    private Image fillImage;
    private Transform cameraTransform;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            hb.transform.SetParent(transform); // attach to enemy
            fillImage = hb.transform.Find("Enemies/EnemyHealthBar/Background/Fill").GetComponent<Image>();
            if (fillImage == null)
            {
                Debug.LogError ("Fill Image not assigned!");
            }
        }

        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Rotate health bar to always face camera
        if (fillImage != null)
        {
            fillImage.transform.parent.parent.LookAt(cameraTransform);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (fillImage != null)
            fillImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
