using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 10f;
    public float attackRate = 1f; 
    private float nextAttackTime = 0f;

    public string attackAnimationName = "Z_Attack"; // نام دقیق انیمیشن حمله
    private Animation legacyAnim;

    void Start()
    {
        legacyAnim = GetComponentInChildren<Animation>();
    }

    void OnTriggerEnter(Collider other)
    {
        TryAttack(other);
    }
    
    void OnTriggerStay(Collider other)
    {
        TryAttack(other);
    }

    private void TryAttack(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                nextAttackTime = Time.time + attackRate;

                if (legacyAnim != null && legacyAnim[attackAnimationName] != null)
                {
                    legacyAnim.CrossFade(attackAnimationName, 0.1f);
                }
            }
        }
    }
}





// using UnityEngine;

// public class EnemyAttack : MonoBehaviour
// {
//     public float damage = 10f;
//     public float attackRate = 1f; 
//     private float nextAttackTime = 0f;

//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player") && Time.time >= nextAttackTime)
//         {
//             PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
//             if (playerHealth != null)
//             {
//                 playerHealth.TakeDamage(damage);
//                 nextAttackTime = Time.time + attackRate;
//             }
//         }
//     }
//     void OnTriggerStay(Collider other)
//     {
//         if (other.CompareTag("Player") && Time.time >= nextAttackTime)
//         {
//             PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
//             if (playerHealth != null)
//             {
//                 playerHealth.TakeDamage(damage);
//                 nextAttackTime = Time.time + attackRate;
//             }
//         }
//     }
// }
