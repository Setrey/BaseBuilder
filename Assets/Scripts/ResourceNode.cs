using UnityEngine;
using Unity.Netcode;

public class ResourceNode : NetworkBehaviour
{
    [SerializeField] private int resourceAmount = 25;
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        // Sprawdzamy, czy to gracz na niego wszed³
        PlayerWallet wallet = other.GetComponent<PlayerWallet>();
        if (wallet != null)
        {
            // Dodajemy surowce do portfela tego konkretnego gracza
            wallet.AddGold(resourceAmount);

            // Surowiec znika z sieci
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
