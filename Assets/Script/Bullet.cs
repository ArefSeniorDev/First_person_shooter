using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage = 10;
    public bool moveWithoutRigidbody = true;

    void Start()
    {
        // Remove old bullets so the scene does not fill with hidden projectiles.
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // If the bullet has no Rigidbody, move it manually along its forward direction.
        if (moveWithoutRigidbody && GetComponent<Rigidbody>() == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
