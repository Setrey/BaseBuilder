using UnityEngine;
using Unity.Netcode;
public class Bullet : NetworkBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Enemy"))
        {
            dealDamage(other);

            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }

    void dealDamage(Collider other)
    {
        other.GetComponent<NetworkObject>().Despawn();
        Destroy(other.gameObject);
    }
}
