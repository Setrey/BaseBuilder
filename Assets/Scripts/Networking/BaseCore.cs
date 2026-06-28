using UnityEngine;
using Unity.Netcode;

public class BaseCore : NetworkBehaviour
{
    public NetworkVariable<float> currentHealthPoints = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public override void OnNetworkSpawn()
    {
        currentHealthPoints.OnValueChanged += OnHpChanged;

        Debug.Log($"[Sieæ] Serce bazy zainicjalizowane. HP: {currentHealthPoints.Value}");
    }
    private void OnHpChanged(float oldVal, float newVal)
    {
        Debug.Log($"[Sieæ] HP Bazy zmieni³o siê z {oldVal} na: {newVal}");

        // ToDo Jakiœ kolor na czerwono screena mo¿na tu upchn¹æ

        if (newVal <= 0)
        {
            Debug.LogError("GAME OVER! Baza zosta³a zniszczona!");
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
