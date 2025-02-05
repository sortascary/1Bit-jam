using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChunk : MonoBehaviour
{
    public LayerMask terrainMask;
    private Dictionary<string, bool> chunkDetected;
    public GameObject[] chunkCheckers; // Assign the checker GameObjects (Up, Down, Left, Right, etc.)
    public MapController mc;

    void Start()
    {
        chunkDetected = new Dictionary<string, bool>();
        foreach (GameObject checker in chunkCheckers)
        {
            chunkDetected[checker.name] = false;
        }
    }

    void Update()
    {
        CheckChunks();
    }

    void CheckChunks()
    {
        foreach (GameObject checker in chunkCheckers)
        {
            Collider2D hit = Physics2D.OverlapCircle(checker.transform.position, 0.1f, terrainMask);
            chunkDetected[checker.name] = hit != null;
        }

        foreach (var chunk in chunkDetected)
        {
            if (!chunk.Value)
            {
                SpawnChunk(chunk.Key);
            }
        }
    }

    void SpawnChunk(string direction)
    {
        // Tambahkan logic spawn chunk sesuai dengan arah yang diperlukan
        if (mc is not null)
        {
            Debug.Log("Spawning chunk at: " + direction);
            mc.CheckAndSpawn(direction);
        }
        else
        {
            Debug.LogError("MapController is not assigned!");
        }
    }
}
