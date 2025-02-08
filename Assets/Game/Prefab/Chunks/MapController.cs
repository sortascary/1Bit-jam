using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    private Vector2 moveDir;

    [Header("Optimization")]
    public HashSet<GameObject> spawnedChunks = new();
    private Queue<GameObject> chunkPool = new();
    public float maxOpDist;
    private float optimizerCooldown;
    public float optimizerCooldownDur;

    [Header("Enemy Spawning")]
    public List<GameObject> enemyPrefabs; // List of enemy types
    public float enemySpawnInterval = 60f; // Time in seconds
    private float enemySpawnTimer;

    void Start()
    {
        if (player.TryGetComponent(out PlayerBehavior pb))
        {
            moveDir = pb.MoveDirection;
        }
        else
        {
            Debug.LogError("PlayerBehavior component is missing on the player!");
        }

        enemySpawnTimer = enemySpawnInterval; // Initialize the timer
    }

    void FixedUpdate()
    {
        if (player.TryGetComponent(out PlayerBehavior pb))
        {
            moveDir = pb.MoveDirection;
        }
        ChunkChecker();
        ChunkOptimizer();
        EnemySpawner(); // Call enemy spawner every frame
    }

    void ChunkChecker()
    {
        if (!currentChunk) return;

        string direction = GetMovementDirection();
        if (!string.IsNullOrEmpty(direction))
        {
            CheckAndSpawn(direction);
        }
    }

    string GetMovementDirection()
    {
        if (moveDir.x > 0) return moveDir.y > 0 ? "Right Up" : (moveDir.y < 0 ? "Right Down" : "Right");
        if (moveDir.x < 0) return moveDir.y > 0 ? "Left Up" : (moveDir.y < 0 ? "Left Down" : "Left");
        if (moveDir.y > 0) return "Up";
        if (moveDir.y < 0) return "Down";
        return "";
    }

    public void CheckAndSpawn(string direction)
    {
        Transform checkTransform = currentChunk?.transform.Find(direction);
        if (checkTransform != null && !Physics2D.OverlapCircle(checkTransform.position, checkerRadius, terrainMask))
        {
            SpawnChunk(checkTransform.position);
        }
    }

    void SpawnChunk(Vector3 position)
    {
        GameObject chunk;
        if (chunkPool.Count > 0)
        {
            chunk = chunkPool.Dequeue();
            chunk.transform.position = position;
            chunk.SetActive(true);
        }
        else
        {
            int rand = Random.Range(0, terrainChunks.Count);
            chunk = Instantiate(terrainChunks[rand], position, Quaternion.identity);
        }
        spawnedChunks.Add(chunk);
    }

    void ChunkOptimizer()
    {
        if ((optimizerCooldown -= Time.deltaTime) > 0) return;
        optimizerCooldown = optimizerCooldownDur;

        List<GameObject> toDisable = new();
        foreach (GameObject chunk in spawnedChunks)
        {
            if (Vector3.Distance(player.transform.position, chunk.transform.position) > maxOpDist)
            {
                chunk.SetActive(false);
                chunkPool.Enqueue(chunk);
                toDisable.Add(chunk);
            }
        }
        foreach (var chunk in toDisable)
        {
            spawnedChunks.Remove(chunk);
        }
    }

    void EnemySpawner()
    {
        enemySpawnTimer -= Time.deltaTime;

        if (enemySpawnTimer <= 0)
        {
            enemySpawnTimer = enemySpawnInterval; // Reset timer

            if (spawnedChunks.Count > 0)
            {
                GameObject randomChunk = GetRandomChunk();
                if (randomChunk != null)
                {
                    SpawnEnemy(randomChunk);
                }
            }
        }
    }

    GameObject GetRandomChunk()
    {
        if (spawnedChunks.Count == 0) return null;

        int randIndex = Random.Range(0, spawnedChunks.Count);
        int i = 0;
        foreach (GameObject chunk in spawnedChunks)
        {
            if (i == randIndex) return chunk;
            i++;
        }
        return null;
    }

    void SpawnEnemy(GameObject chunk)
    {
        int rand = Random.Range(0, enemyPrefabs.Count);
        Vector3 spawnPos = chunk.transform.position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0); // Random offset within chunk
        Instantiate(enemyPrefabs[rand], spawnPos, Quaternion.identity);
    }
}
