using UnityEngine;

public class SimpleAnimalWander : MonoBehaviour
{
    public float moveSpeed = 0.8f;
    public float turnSpeed = 70f;
    public float directionChangeInterval = 3f;

    private float nextDirectionChange;
    private Vector3 moveDirection;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        PickNewDirection();
    }

    void Update()
    {
        // Ambient animals keep the city alive without needing NavMesh setup.
        if (Time.time >= nextDirectionChange)
        {
            PickNewDirection();
        }

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", moveSpeed);
        }
    }

    private void PickNewDirection()
    {
        nextDirectionChange = Time.time + directionChangeInterval + Random.Range(0f, 2f);
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }
}
