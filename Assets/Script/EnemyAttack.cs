// EnemyAttack.cs
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 10f;
    public float attackRate = 1f; // seconds between attacks
    private float nextAttackTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter ");

        if (other.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            Debug.Log("OnTriggerEnter 1");

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                nextAttackTime = Time.time + attackRate;
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                nextAttackTime = Time.time + attackRate;
            }
        }
    }
}
