using UnityEngine;
using Unity.Netcode;

public class BaseCore : NetworkBehaviour
{
    public NetworkVariable<float> currentHealthPoints = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    //public Transform playerSpawnPoint;

    void Awake()
    {
        //playerSpawnPoint.position = transform.position + Vector3.forward;
    }
    public override void OnNetworkSpawn()
    {
        // Rejestrujemy funkcjê, która wywo³a siê ZAWSZE, gdy wartoœæ HP siê zmieni (u Hosta i Klienta)
        currentHealthPoints.OnValueChanged += OnHpChanged;

        // Wyœwietlamy pocz¹tkowe HP na starcie
        Debug.Log($"[Sieæ] Serce bazy zainicjalizowane. HP: {currentHealthPoints.Value}");
    }
    private void OnHpChanged(float oldVal, float newVal)
    {
        // Ten kod odpali siê na KA¯DYM komputerze, gdy serwer zmieni wartoœæ baseHp
        Debug.Log($"[Sieæ] HP Bazy zmieni³o siê z {oldVal} na: {newVal}");

        if (newVal <= 0)
        {
            Debug.LogError("GAME OVER! Baza zosta³a zniszczona!");
            // Tutaj w przysz³oœci dodamy ekran koñca gry
        }
    }
    public void TakeDamage(float damage)
    {
        if (!IsServer) return;

        currentHealthPoints.Value -= damage;

        if (currentHealthPoints.Value <= 0f)
            Debug.Log("Baza Zniszczona!");

    }
}
