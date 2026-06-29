using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class WaveSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private int enemiesPerWave = 3;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(WaveRoutine());
        }
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        Debug.Log("[Serwer] Fala nadchodzi!");
        for (int i = 0; i < enemiesPerWave; i++)
        {
            // Losujemy pozycjê spawnu wokó³ bazy (np. w promieniu 15 jednostek)
            Vector3 randomOffset = new Vector3(Random.Range(-15f, 15f), 0.5f, Random.Range(-15f, 15f));
            Vector3 spawnPos = Vector3.zero + randomOffset; // Zak³adamy, ¿e baza jest blisko œrodka (0,0,0)

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // Kluczowe: Spawnujemy obiekt w sieci!
            enemy.GetComponent<NetworkObject>().Spawn();
        }
    }
}
