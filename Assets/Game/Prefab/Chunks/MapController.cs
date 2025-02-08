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

            Vector3 spawnPos = GetSpawnPositionOutsideView();
            if (spawnPos != Vector3.zero)
            {
                SpawnEnemy(spawnPos);
            }
        }
    }

    Vector3 GetSpawnPositionOutsideView()
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector3.zero;

        // Get camera bounds in world space
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // Define spawn margins (off-screen distance)
        float spawnMargin = 2f; // Adjust as needed

        // Randomize spawn position outside the view
        int side = Random.Range(0, 4); // 0 = Left, 1 = Right, 2 = Top, 3 = Bottom
        Vector3 spawnPos = Vector3.zero;

        switch (side)
        {
            case 0: // Left
                spawnPos = new Vector3(player.transform.position.x - (camWidth / 2) - spawnMargin,
                                       player.transform.position.y + Random.Range(-camHeight / 2, camHeight / 2),
                                       0);
                break;
            case 1: // Right
                spawnPos = new Vector3(player.transform.position.x + (camWidth / 2) + spawnMargin,
                                       player.transform.position.y + Random.Range(-camHeight / 2, camHeight / 2),
                                       0);
                break;
            case 2: // Top
                spawnPos = new Vector3(player.transform.position.x + Random.Range(-camWidth / 2, camWidth / 2),
                                       player.transform.position.y + (camHeight / 2) + spawnMargin,
                                       0);
                break;
            case 3: // Bottom
                spawnPos = new Vector3(player.transform.position.x + Random.Range(-camWidth / 2, camWidth / 2),
                                       player.transform.position.y - (camHeight / 2) - spawnMargin,
                                       0);
                break;
        }

        return spawnPos;
    }

    void SpawnEnemy(Vector3 spawnPos)
    {
        int rand = Random.Range(0, enemyPrefabs.Count);
        Instantiate(enemyPrefabs[rand], spawnPos, Quaternion.identity);
    }

}
