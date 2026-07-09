using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float speed = 20f;
	public float lifeTime = 2f;
	public float damage = 25f;
	public bool moveWithoutRigidbody = true;

	void Start()
	{
		Destroy(gameObject, lifeTime);
	}

	void Update()
	{
		if (moveWithoutRigidbody && GetComponent<Rigidbody>() == null)
		{
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		ApplyDamage(collision.collider);
	}

	void OnTriggerEnter(Collider other)
	{
		ApplyDamage(other);
	}

	private void ApplyDamage(Collider hitCollider)
	{
		EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();
		if (enemyHealth != null)
		{
			enemyHealth.TakeDamage(damage);
		}

		Destroy(gameObject);
	}
}