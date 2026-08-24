using UnityEngine;
using Unity.Netcode;
using System;

public class Turret : NetworkBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] Transform cannonPivot;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float range = 8f;
    [SerializeField] private float fireRate = 1f; // Strza³y na sekundê
    [SerializeField] private LayerMask enemyLayer;

    private Transform targetEnemy;
    private float fireCooldown = 0f;


    
    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        checkEnemiesAround();

        // 2. Jeœli mamy cel – obracamy lufê i strzelamy
        if (targetEnemy != null)
        {
            RotateTowardsTarget();

            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Spawnujemy pocisk na serwerze
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Pobieramy NetworkObject i zsynchronizujemy go w sieci
        NetworkObject netObj = bulletObj.GetComponent<NetworkObject>();
        if (netObj != null)
            netObj.Spawn();

        TurretBullet turretBullet = bulletObj.GetComponent<TurretBullet>();

        if (turretBullet !=null)
            turretBullet.setTarget(targetEnemy);
    }

    private void RotateTowardsTarget()
    {
        if (cannonPivot == null) return;

        Vector3 direction = targetEnemy.position - cannonPivot.position;

        //TODO: 
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // P³ynne obracanie wie¿yczki
            cannonPivot.rotation = Quaternion.Slerp(cannonPivot.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void checkEnemiesAround()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider hit in hits)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = hit.transform;
            }
        }

        targetEnemy = nearestEnemy;
    }
}
