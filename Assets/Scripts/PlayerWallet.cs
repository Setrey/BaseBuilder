using Unity.Netcode;
using UnityEngine;

public class PlayerWallet : NetworkBehaviour
{
    // Synchronizowana zmienna z³ota
    public NetworkVariable<int> gold = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        gold.OnValueChanged += OnGoldChanged;
        Debug.Log($"[Portfel] Do³¹czono do gry. Stan konta: {gold.Value}");
    }

    public override void OnNetworkDespawn()
    {
        gold.OnValueChanged -= OnGoldChanged;
    }

    private void OnGoldChanged(int oldVal, int newVal)
    {
        if (IsOwner)
        {
            Debug.Log($"[Portfel] Twoje z³oto zmieni³o siê na: {newVal}");
            // Tutaj w przysz³oœci podepniemy tekst w UI ekranu
        }
    }

    // Funkcja wywo³ywana przez serwer, gdy gracz podnosi surowiec
    public void AddGold(int amount)
    {
        if (!IsServer) return;
        gold.Value += amount;
    }

    // Funkcja pomocnicza do wydawania z³ota (np. przy budowaniu)
    public bool TrySpendGold(int amount)
    {
        if (!IsServer) return false;

        if (gold.Value >= amount)
        {
            gold.Value -= amount;
            return true;
        }
        return false;
    }
}