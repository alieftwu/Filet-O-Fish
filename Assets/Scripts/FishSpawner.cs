using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab; // Fish prefab to spawn
    public int numberOfFish = 10; // Total fish to spawn
    public Vector3 spawnAreaSize = new Vector3(10f, 2f, 10f); // Spawn area dimensions

    void Start()
    {
        SpawnFish();
    }

    void SpawnFish()
    {
        for (int i = 0; i < numberOfFish; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            ) + transform.position;

            Instantiate(fishPrefab, randomPosition, Quaternion.identity);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
