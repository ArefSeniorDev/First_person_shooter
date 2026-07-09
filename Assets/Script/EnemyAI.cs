using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float stoppingDistance = 1.6f;
    public float detectionInterval = 0.25f;
    public float destinationRefreshInterval = 0.15f;
    public float navMeshSampleDistance = 4f;
    public string legacyWalkAnimation = "Zombie@Z_Walk_InPlace";
    public string legacyIdleAnimation = "Zombie@Z_Idle";

    private NavMeshAgent agent;
    private Animator animator;
    private Animation legacyAnimation;
    private float nextDetectionTime;
    private float nextDestinationRefreshTime;
    private Vector3 lastDestination;

    private const float DestinationMoveThreshold = 0.2f;

    void Start()
    {
        // Cache the movement and animation components once so Update stays light.
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        legacyAnimation = GetComponentInChildren<Animation>();

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = true;
        }

        FindTarget();
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            target = null;
            FindTarget();
        }

        if (target == null || agent == null || !agent.enabled)
        {
            PlayZombieAnimation(false);
            return;
        }

        if (!EnsureAgentIsOnNavMesh())
        {
            PlayZombieAnimation(false);
            return;
        }

        UpdateDestination();

        bool isMoving = agent.pathPending || agent.velocity.sqrMagnitude > 0.05f || agent.remainingDistance > agent.stoppingDistance + 0.05f;
        PlayZombieAnimation(isMoving);
    }

    private void FindTarget()
    {
        if (Time.time < nextDetectionTime)
        {
            return;
        }

        nextDetectionTime = Time.time + detectionInterval;

        // Prefer the Player tag, but fall back to PlayerHealth so a missing tag does not break zombie tracking.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            return;
        }

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            target = playerHealth.transform;
        }
    }

    private bool EnsureAgentIsOnNavMesh()
    {
        if (agent.isOnNavMesh)
        {
            return true;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            return agent.isOnNavMesh;
        }

        return false;
    }

    private void UpdateDestination()
    {
        if (Time.time < nextDestinationRefreshTime && (target.position - lastDestination).sqrMagnitude < DestinationMoveThreshold * DestinationMoveThreshold)
        {
            return;
        }

        nextDestinationRefreshTime = Time.time + destinationRefreshInterval;

        NavMeshHit hit;
        Vector3 destination = target.position;
        if (NavMesh.SamplePosition(target.position, out hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            destination = hit.position;
        }

        if (agent.SetDestination(destination))
        {
            lastDestination = target.position;
        }
    }

    private void PlayZombieAnimation(bool isMoving)
    {
        // Mecanim controllers can read a Speed float if the zombie prefab uses Animator.
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetFloat("Speed", isMoving ? 1f : 0f);
        }

        // Older Unity zombie imports often use the legacy Animation component.
        if (legacyAnimation != null)
        {
            string clipName = isMoving ? legacyWalkAnimation : legacyIdleAnimation;
            if (legacyAnimation[clipName] != null && !legacyAnimation.IsPlaying(clipName))
            {
                legacyAnimation.CrossFade(clipName, 0.2f);
            }
        }
    }
}
