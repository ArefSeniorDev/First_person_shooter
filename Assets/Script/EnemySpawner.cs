using UnityEngine;

using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemyPrefab;
	public Transform spawnPoint;
	public float spawnInterval = 20f;

	public float spawnCheckRadius = 1.5f; 
	public float navMeshSampleRadius = 8f;

	private float timer;

	public int maxEnemies = 100;
	private int enemiesSpawned = 0;

	void Start()
	{
		timer = spawnInterval;
	}

	void Update()
	{
		if (enemiesSpawned >= maxEnemies) return;

		timer -= Time.deltaTime;

		if (timer <= 0f)
		{
			TrySpawnEnemy();
			timer = spawnInterval;
		}
	}

	void TrySpawnEnemy()
	{
		if (enemyPrefab == null)
		{
			return;
		}

		Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
		if (!TryGetSpawnPositionOnNavMesh(spawnPosition, out spawnPosition))
		{
			timer = 1f;
			return;
		}

		Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, spawnCheckRadius);
		bool isSpaceClear = true;

		foreach (var col in hitColliders)
		{
			if (col.CompareTag("Enemy") || col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				isSpaceClear = false;
				break;
			}
		}

		if (isSpaceClear)
		{
			GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity) as GameObject;
			NavMeshAgent spawnedAgent = enemy != null ? enemy.GetComponent<NavMeshAgent>() : null;
			if (spawnedAgent != null && spawnedAgent.enabled && !spawnedAgent.isOnNavMesh)
			{
				spawnedAgent.Warp(spawnPosition);
			}
			enemiesSpawned++;
		}
		else
		{
			timer = 1f; 
		}
	}

	private bool TryGetSpawnPositionOnNavMesh(Vector3 requestedPosition, out Vector3 spawnPosition)
	{
		spawnPosition = requestedPosition;

		NavMeshHit hit;
		if (NavMesh.SamplePosition(requestedPosition, out hit, navMeshSampleRadius, NavMesh.AllAreas))
		{
			spawnPosition = hit.position;
			return true;
		}

		return false;
	}
}
