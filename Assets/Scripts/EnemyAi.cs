using UnityEngine;
using Unity.Netcode;

public class EnemyAi : NetworkBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private int damagePerSecond = 5;

    private Transform baseCoreTransform;
    private float waitTillHit = 0f;

    public override void OnNetworkSpawn()
    {
        // Tylko serwer przetwarza logikê poruszania AI
        if (!IsServer)
        {
            enabled = false; // Wy³¹czamy skrypt u klientów, ¿eby nie marnowaæ procesora
            return;
        }

        // Serwer szuka bazy na scenie
        BaseCore core = FindObjectOfType<BaseCore>();

        if (core != null)
        {
            baseCoreTransform = core.transform;
        }
    }

    void Update()
    {
        // Ta logika wykonuje siê TYLKO na serwerze (Hosta)
        if (baseCoreTransform == null) return;

        Movement();
    }

    void Movement ()
    {
        // Prosty ruch w stronê bazy
        Vector3 direction = (baseCoreTransform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        // Jeœli wróg dotknie bazy, zadaje jej obra¿enia
        BaseCore core = other.GetComponent<BaseCore>();

        if (core != null)
        {
            core.TakeDamage(damagePerSecond);

            // Po uderzeniu w bazê wróg znika w sieci
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
