using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
	public float health = 100f;
	public string deathAnimationName = "Z_FallingBack"; // نام دقیق انیمیشن مردن

	private bool isDead = false;
	private Animation legacyAnim;

	void Start()
	{
		legacyAnim = GetComponentInChildren<Animation>();

		// قفل کردن انیمیشن مردن که فقط یک‌بار پخش شود
		if (legacyAnim != null && legacyAnim[deathAnimationName] != null)
		{
			legacyAnim[deathAnimationName].wrapMode = WrapMode.Once;
		}
	}

	public void TakeDamage(float amount)
	{
		if (isDead) return;

		health -= amount;

		if (health <= 0)
		{
			Die();
		}
	}


	private void Die()
	{
		isDead = true;

		KillCounter killCounter = FindObjectOfType<KillCounter>();
		if (killCounter != null)
		{
			killCounter.AddKill();
		}

		GameManager gameManager = FindObjectOfType<GameManager>();
		if (gameManager != null)
		{
			gameManager.EnemyKilled();
		}
		// -----------------------------------------------------

		if (legacyAnim != null && legacyAnim[deathAnimationName] != null)
		{
			legacyAnim.CrossFade(deathAnimationName, 0.2f);
		}

		if (GetComponent<EnemyAI>() != null) 
			GetComponent<EnemyAI>().enabled = false;

		if (GetComponent<UnityEngine.AI.NavMeshAgent>() != null) 
			GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

		if (GetComponent<EnemyAttack>() != null)
			GetComponent<EnemyAttack>().enabled = false;

		Collider col = GetComponent<Collider>();
		if (col != null) col.enabled = false;

		Destroy(gameObject, 10f);
	}
}