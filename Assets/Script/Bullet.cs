using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Hit object: " + collision.gameObject.name);
        EnemyHealth health = collision.collider.GetComponent<EnemyHealth>();
        if (health != null)
        {
            Debug.LogError("Shoted");

            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
