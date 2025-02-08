using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChunk : MonoBehaviour
{
    public LayerMask terrainMask;
    public GameObject[] chunkCheckers;
    public MapController mc;

    void FixedUpdate()
    {
        CheckChunks();
    }

    void CheckChunks()
    {
        foreach (GameObject checker in chunkCheckers)
        {
            if (!Physics2D.OverlapCircle(checker.transform.position, 0.1f, terrainMask))
            {
                SpawnChunk(checker.name);
            }
        }
    }

    void SpawnChunk(string direction)
    {
        if (mc != null)
        {
            Debug.Log($"Spawning chunk at: {direction}");
            mc.CheckAndSpawn(direction);
        }
        else
        {
            Debug.LogError("MapController is not assigned!");
        }
    }
}
