using UnityEngine;
using Unity.Netcode;
using System;

public class Player : NetworkBehaviour
{

    public CharacterController controller;
    public Statistics stats;

    public GameObject cameraGameObject;
    public GameObject cameraPivot;

    [SerializeField] private GameObject WallPrefab;
    [SerializeField] private GameObject WallBluePrintPrefab;
    [SerializeField] private GameObject BulletPrefab;


    private GameObject currentBluePrint;

    private Boolean doRotateOnEdges;

    [Header("General Settings")]
    public float maxBuildingDistance = 10f;
    [SerializeField] private Transform firePoint;
    public LayerMask groundLayer;

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public float edgeThreshold = 20f;
    private float currentX = 0f;
    private float currentY = 0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    public float sensitivity = 5.0f;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        stats = new Statistics();
        doRotateOnEdges = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Right Camera and AudioListener Atachment
    public override void OnNetworkSpawn()
    {
        //base.OnNetworkSpawn();
        if (IsOwner)
        {
            cameraGameObject.SetActive(true);

            if(Camera.main !=null && Camera.main!=cameraGameObject.GetComponent<Camera>())
            {
                Camera.main.gameObject.SetActive(false);
            }
        }
        else
        {
            cameraGameObject.SetActive(false);

            AudioListener otherListener = cameraGameObject.GetComponent<Camera>().GetComponent<AudioListener>();
            if (otherListener != null) otherListener.enabled = false;
        }
    }
    void Movement()
    {

        // Pobieranie inputu (WASD / Strzalki)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward  = cameraPivot.transform.forward;
        Vector3 camRight = cameraPivot.transform.right;
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        //Debug.Log("x: " + x + "z: " + z);
        // Obliczanie kierunku wzgledem rotacji gracza
        Vector3 move = camRight * x + camForward * z;

        controller.Move(move * Time.deltaTime * stats.speed);
    }

    void Rotating()
    {

        Vector3 mousePos = Input.mousePosition;
        float rotationAmount = rotationSpeed * Time.deltaTime;

        if (doRotateOnEdges)
        { 
            // Check Horizontal Edges (Rotate around Y axis)
            if (mousePos.x <= edgeThreshold)
                currentY += rotationAmount;
            else if (mousePos.x >= Screen.width - edgeThreshold)
                currentY -= rotationAmount;

            // Check Vertical Edges (Rotate around X axis)
            if (mousePos.y <= edgeThreshold)
                currentX -= rotationAmount;
            else if (mousePos.y >= Screen.height - edgeThreshold)
                currentX += rotationAmount;
        }

        // Rotation using scroll
        if (Input.GetMouseButton(2))
        {
            currentX += -Input.GetAxis("Mouse Y") * sensitivity;
            currentY += Input.GetAxis("Mouse X") * sensitivity;
        }

        currentX = Mathf.Clamp(currentX, 0f, 85f);
        cameraPivot.transform.localRotation = Quaternion.Euler(currentX, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, currentY, 0f);
    }


    void Scrolling()
    {

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float v = cameraGameObject.transform.localPosition.z + Input.GetAxis("Mouse ScrollWheel") * 2f;
            cameraGameObject.transform.localPosition = new Vector3 (0f, 0f, v);
        }
    }
    void CameraMovement()
    {
        Rotating();
        Scrolling();
    }

    void Building()
    {
        /*
         * 
         * // ToDo Blueprint
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("wchodze tu Building E ");
            SpawnBluePrint();
            Debug.Log("wychodze stad Building E Down");
            
        }*/

        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("wchodze tu Building E UP");
            Vector3 spawnPosition = transform.position + transform.forward * 2f;

            // ToDo Blueprint
            //if (currentBluePrint != null) Destroy(currentBluePrint);

            RequestSpawnWallServerRpc(spawnPosition);

        }
    }

    private void SpawnBluePrint()
    {
        // Rzucamy promieñ ze œrodka ekranu (tam gdzie patrzy kamera)
        Ray ray = cameraGameObject.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Debug.Log("wchodze tu");

        if (Physics.Raycast(ray, out hit, maxBuildingDistance, groundLayer))
        {
            Debug.Log("wchodze tu 2");

            WallBluePrintPrefab.transform.position = hit.point;

            WallBluePrintPrefab.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Debug.Log("wchodze tu 3");
            currentBluePrint = Instantiate(WallBluePrintPrefab);
        }
        else
        {
            // Jeœli patrzymy w niebo, ukrywamy obiekt daleko lub wy³¹czamy widocznoœæ
            WallBluePrintPrefab.transform.position = ray.GetPoint(maxBuildingDistance);

            if (currentBluePrint != null) Destroy(currentBluePrint);
        }
    }

    [ServerRpc]
    private void DamageBaseServerRpc(float damageAmount)
    {
        BaseCore core = FindObjectOfType<BaseCore>();
        if (core != null)
        {
            core.TakeDamage(damageAmount);
        }
    }
    [ServerRpc]
    private void RequestSpawnWallServerRpc(Vector3 position)
    {
        
        GameObject go = Instantiate(WallPrefab, position, Quaternion.identity);

        go.GetComponent<NetworkObject>().Spawn();
    }
    void Action()
    {

        if (Input.GetKey(KeyCode.R))
        {
            DamageBaseServerRpc(10.0f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            ShootServerRpc(firePoint.position, firePoint.rotation);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(BulletPrefab, position, rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        Movement();
        CameraMovement();
        Building();
        Action();
    }

}