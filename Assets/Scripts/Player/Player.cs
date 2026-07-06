using UnityEngine;
using Unity.Netcode;
using System;

public class Player : NetworkBehaviour
{

    public CharacterController controller;
    public Statistics stats;
    public PlayerWallet playerWallet;

    public GameObject cameraGameObject;
    public GameObject cameraPivot;

    [SerializeField] private GameObject WallPrefab;
    [SerializeField] private GameObject WallBluePrintPrefab;
    [SerializeField] private GameObject BulletPrefab;


    private GameObject currentBlueprint;

    private Boolean doRotateOnEdges;

    [Header("General Settings")]
    public float maxBuildingDistance = 10f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float wallCost = 25.0f;
    public LayerMask groundLayer;
    [SerializeField] public float baseFireRate = 1f;
    private float fireTime = 0f;


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
        playerWallet = GetComponent<PlayerWallet>();
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentBlueprint == null)
            {
                currentBlueprint = Instantiate(WallBluePrintPrefab);
            }
        }

        if (Input.GetKey(KeyCode.E) && currentBlueprint != null)
        {
            Vector3 targetPosition = GetGridPositionFromMouse();
            currentBlueprint.transform.position = targetPosition;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (currentBlueprint != null)
            {
                Vector3 finalPosition = currentBlueprint.transform.position;

                Destroy(currentBlueprint);
                currentBlueprint = null;

                if (playerWallet.gold.Value >= wallCost)
                {
                    RequestSpawnWallServerRpc(finalPosition);
                }
                else
                {
                    Debug.Log("Za ma³o z³ota na budowê œciany!");
                }
            }
        }
    }

    // Funkcja rzucaj¹ca promieñ z myszki i wyrównuj¹ca pozycjê do siatki 1x1
    private Vector3 GetGridPositionFromMouse()
    {
        Ray ray = cameraGameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            float snappedX = Mathf.Round(hit.point.x / gridSize) * gridSize;
            float snappedZ = Mathf.Round(hit.point.z / gridSize) * gridSize;
            return new Vector3(snappedX, 0.5f, snappedZ);

        }

        // Skyhit
        Vector3 defaultPos = transform.position + transform.forward * 2f;
        return new Vector3(Mathf.Round(defaultPos.x), 0.5f, Mathf.Round(defaultPos.z));
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
        PlayerWallet serverWallet = GetComponent<PlayerWallet>();

        float actualWallCost = wallCost;

        //Discount for Builder
        if (GetComponent<PlayerClass>().currentProfession.Value == CharacterProfession.Builder)
            actualWallCost = Mathf.Round(wallCost * 0.6f);

        if (serverWallet != null && serverWallet.TrySpendGold(actualWallCost))
        {
            // Jeœli serwer pomyœlnie pobra³ z³oto, stawia sieæ œcianê
            GameObject wall = Instantiate(WallPrefab, position, Quaternion.identity);
            wall.GetComponent<NetworkObject>().Spawn();
        }
    }
    void Action()
    {

        if (Input.GetKey(KeyCode.R))
        {
            DamageBaseServerRpc(10.0f);
        }

        if (Input.GetButton("Fire1") && fireTime <=0f)
        {
            fireTime = Time.deltaTime + baseFireRate;
            
            ShootServerRpc(firePoint.position, firePoint.rotation);
        }
        if (fireTime > 0f)
            fireTime -= Time.deltaTime;
        
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