 using System.Collections.Generic;
  using UnityEngine;

  public class ChunkSpawner : MonoBehaviour
  {
      public Transform player;
      public GameObject chunkPrefab;

      public int chunkSize = 28;
      public int viewDistance = 1;

      private HashSet<Vector2Int> spawnedChunks = new HashSet<Vector2Int>();

      void Update()
      {
          Vector2Int playerChunk = GetPlayerChunk();

          for (int y = -viewDistance; y <= viewDistance; y++)
          {
              for (int x = -viewDistance; x <= viewDistance; x++)
              {
                  Vector2Int chunkCoord = playerChunk + new Vector2Int(x, y);

                  if (!spawnedChunks.Contains(chunkCoord))
                  {
                      SpawnChunk(chunkCoord);
                  }
              }
          }
      }

      Vector2Int GetPlayerChunk()
      {
          int chunkX = Mathf.FloorToInt(player.position.x / chunkSize);
          int chunkY = Mathf.FloorToInt(player.position.y / chunkSize);

          return new Vector2Int(chunkX, chunkY);
      }

      void SpawnChunk(Vector2Int chunkCoord)
      {
          Vector3 spawnPosition = new Vector3(
              chunkCoord.x * chunkSize,
              chunkCoord.y * chunkSize,
              0
          );

          Instantiate(chunkPrefab, spawnPosition, Quaternion.identity);

          spawnedChunks.Add(chunkCoord);
      }
  }
