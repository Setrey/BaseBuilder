using UnityEngine;
using Unity.Netcode;
using System;

public class EnemyAi : NetworkBehaviour
{
    [SerializeField] private float healthPoints = 1f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int damagePerSecond = 5;

    private Transform baseCoreTransform;
    private float waitTillHit = 0f;
    private Rigidbody rb;
    public override void OnNetworkSpawn()
    {
        // Tylko serwer przetwarza logikê poruszania AI
        if (!IsServer)
        {
            enabled = false; // Wy³¹czamy skrypt u klientów, ¿eby nie marnowaæ procesora
            return;
        }

        rb = GetComponent<Rigidbody>();
        BaseCore core = FindObjectOfType<BaseCore>();

        if (core != null)  baseCoreTransform = core.transform;

    }

    void FixedUpdate()
    {
        // Ta logika wykonuje siê TYLKO na serwerze (Hosta)
        if (baseCoreTransform == null || rb==null) return;

        Movement();
    }

    void Movement ()
    {
        // Prosty ruch w stronê bazy
        Vector3 direction = (baseCoreTransform.position - transform.position).normalized;
        //transform.position += direction * speed * Time.deltaTime;

        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);

        if (direction != Vector3.zero)
            rb.MoveRotation(Quaternion.LookRotation(direction));
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

    public void TakeDamage(float damageToTake)
    {
        Debug.Log("Odejmujemy hp");
        if (healthPoints-damageToTake<0f)
        {
            Debug.Log("Enemy Dead");
            healthPoints = 0f;
            
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
        else
        {
            healthPoints -= damageToTake;
            UpdateHUD();

        }
    }

    private void UpdateHUD()
    {
        Debug.Log("health Points: " + healthPoints);

    }
}
