using Unity.Netcode;
using UnityEngine;

public class ResourceSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject resourcePrefab;
    [SerializeField] private int initialResources = 10;
    [SerializeField] private float spawnRadius = 20f;

    public override void OnNetworkSpawn()
    {
        // Tylko serwer decyduje o rozmieszczeniu surowców w œwiecie gry
        if (IsServer)
        {
            SpawnInitialResources();
        }
    }

    private void SpawnInitialResources()
    {
        for (int i = 0; i < initialResources; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0.5f, // Lekko nad ziemi¹
                Random.Range(-spawnRadius, spawnRadius)
            );

            GameObject resource = Instantiate(resourcePrefab, randomPos, Quaternion.identity);
            resource.GetComponent<NetworkObject>().Spawn();
        }
    }
}