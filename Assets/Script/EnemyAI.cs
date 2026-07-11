

using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float stoppingDistance = 1.6f;
    public float detectionInterval = 0.25f;
    public float navMeshSampleRadius = 8f;

    [Header("Animation Names")]
    public string legacyWalkAnimation = "Z_Walk_InPlace";

    public string legacyIdleAnimation = "Z_Walk_InPlace";

    public string legacyAttackAnimation = "Z_Attack";

    public string legacyStumbleAnimation = "Z_Walk_InPlace";

    private NavMeshAgent agent;
    private Animation legacyAnimation;
    private float nextDetectionTime;

    private float changeStateTimer = 0f;
    private string currentMoveAnimation;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        legacyAnimation = GetComponentInChildren<Animation>();

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
            EnsureAgentOnNavMesh();
        }

        if (legacyAnimation != null)
        {
            if (legacyAnimation[legacyIdleAnimation] != null)
                legacyAnimation[legacyIdleAnimation].wrapMode = WrapMode.Loop;

            if (legacyAnimation[legacyWalkAnimation] != null)
                legacyAnimation[legacyWalkAnimation].wrapMode = WrapMode.Loop;

            if (legacyAnimation[legacyStumbleAnimation] != null)
                legacyAnimation[legacyStumbleAnimation].wrapMode = WrapMode.Loop;

            if (legacyAnimation[legacyAttackAnimation] != null)
                legacyAnimation[legacyAttackAnimation].wrapMode = WrapMode.Once;
        }

        currentMoveAnimation = legacyWalkAnimation;
        FindTarget();
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            target = null;
            FindTarget();
        }

        if (target == null || agent == null || !agent.enabled) return;

        if (!EnsureAgentOnNavMesh())
        {
            PlayAnimation(legacyIdleAnimation);
            return;
        }

        if (legacyAnimation != null && legacyAnimation[legacyAttackAnimation] != null && legacyAnimation.IsPlaying(legacyAttackAnimation))
        {
            agent.isStopped = true;
            return;
        }

        Vector3 destination;
        if (!TryGetDestinationOnNavMesh(target.position, out destination))
        {
            agent.isStopped = true;
            PlayAnimation(legacyIdleAnimation);
            return;
        }

        agent.isStopped = false;
        if (!agent.SetDestination(destination))
        {
            PlayAnimation(legacyIdleAnimation);
            return;
        }

        bool hasPathDistance = !agent.pathPending && agent.hasPath;
        bool isMoving = agent.velocity.sqrMagnitude > 0.05f || (hasPathDistance && agent.remainingDistance > agent.stoppingDistance + 0.05f);

        if (isMoving)
        {
            changeStateTimer -= Time.deltaTime;
            if (changeStateTimer <= 0f)
            {
                changeStateTimer = Random.Range(3f, 6f);
                currentMoveAnimation = (Random.value > 0.5f) ? legacyWalkAnimation : legacyStumbleAnimation;
            }

            PlayAnimation(currentMoveAnimation);
        }
        else
        {
            PlayAnimation(legacyIdleAnimation);
        }
    }

    private void FindTarget()
    {
        if (Time.time < nextDetectionTime) return;
        nextDetectionTime = Time.time + detectionInterval;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    private void PlayAnimation(string animName)
    {
        if (legacyAnimation != null && legacyAnimation[animName] != null)
        {
            if (!legacyAnimation.IsPlaying(animName))
            {
                legacyAnimation.CrossFade(animName, 0.3f);
            }
        }
    }

    private bool EnsureAgentOnNavMesh()
    {
        if (agent == null || !agent.enabled)
        {
            return false;
        }

        if (agent.isOnNavMesh)
        {
            return true;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, navMeshSampleRadius, agent.areaMask))
        {
            transform.position = hit.position;
            return agent.Warp(hit.position) || agent.isOnNavMesh;
        }

        return false;
    }

    private bool TryGetDestinationOnNavMesh(Vector3 requestedPosition, out Vector3 destination)
    {
        destination = requestedPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(requestedPosition, out hit, navMeshSampleRadius, agent.areaMask))
        {
            destination = hit.position;
            return true;
        }

        return false;
    }
}
