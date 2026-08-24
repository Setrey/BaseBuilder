using UnityEngine;
using Unity.Netcode;
public class TurretBullet : NetworkBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float lifeTime = 5f;

    private Transform target;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke(nameof(DespawnBullet), lifeTime);
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        if (target == null)
        {
            Debug.Log("despawnuje");
            DespawnBullet();
            return;
        }

        // Kierunek do celu
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed* Time.deltaTime;

        // Sprawdzamy czy w tej klatce dolecieæ do wroga (trafienie)
        if (dir.magnitude <= distanceThisFrame)
        {
            Debug.Log("Cel Trafiony!");
            HitTarget();
            return;
        }

        // Ruch w stronê wroga i obrót w stronê lotu
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);

    }
    public void setTarget(Transform _target)
    {
        target = _target;

    }
    private void HitTarget()
    {
        // Próbujemy zadaæ obra¿enia
        EnemyAi enemy = target.GetComponent<EnemyAi>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        DespawnBullet();
    }
    
    private void DespawnBullet()
    {
        if (IsServer && NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
    }
}
