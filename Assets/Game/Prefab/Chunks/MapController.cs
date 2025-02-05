using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public Vector3 noTerrainPosition;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    private Vector2 moveDir;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist; //Must be greater than the length and width of the tilemap
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDur;

    void Start()
    {
        moveDir = player.GetComponent<PlayerBehavior>().MoveDirection;
    }

    void Update()
    {
        moveDir = player.GetComponent<PlayerBehavior>().MoveDirection;
        ChunkChecker();
        ChunkOptimzer();
    }

    void ChunkChecker()
    {
        if (!currentChunk) return;

        if (moveDir.x > 0 && moveDir.y == 0)
        {
            CheckAndSpawn("Right");
        }
        else if (moveDir.x < 0 && moveDir.y == 0)
        {
            CheckAndSpawn("Left");
        }
        else if (moveDir.y > 0 && moveDir.x == 0)
        {
            CheckAndSpawn("Up");
        }
        else if (moveDir.y < 0 && moveDir.x == 0)
        {
            CheckAndSpawn("Down");
        }
        else if (moveDir.x > 0 && moveDir.y > 0)
        {
            CheckAndSpawn("Right Up");
        }
        else if (moveDir.x > 0 && moveDir.y < 0)
        {
            CheckAndSpawn("Right Down");
        }
        else if (moveDir.x < 0 && moveDir.y > 0)
        {
            CheckAndSpawn("Left Up");
        }
        else if (moveDir.x < 0 && moveDir.y < 0)
        {
            CheckAndSpawn("Left Down");
        }
    }

    public void CheckAndSpawn(string direction)
    {
        Transform checkTransform = currentChunk.transform.Find(direction);
        if (checkTransform != null && !Physics2D.OverlapCircle(checkTransform.position, checkerRadius, terrainMask))
        {
            noTerrainPosition = checkTransform.position;
            SpawnChunk();
        }
    }

    void SpawnChunk()
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], noTerrainPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimzer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDur;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            chunk.SetActive(opDist <= maxOpDist);
        }
    }
}