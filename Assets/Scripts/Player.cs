using UnityEngine;
using Unity.Netcode;
using System;

public class Player : NetworkBehaviour
{

    public CharacterController controller;
    public Statistics stats;

    public GameObject cameraGameObject;
    public GameObject cameraPivot;

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
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Movement()
    {
        //if (!IsOwner) return;

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


        // Rotation using scroll
        if (Input.GetMouseButton(2))
        {
            currentX += -Input.GetAxis("Mouse Y") * sensitivity;
            currentY += Input.GetAxis("Mouse X") * sensitivity;
        }

        currentX = Mathf.Clamp(currentX, 0f, 85f);
        cameraPivot.transform.localRotation = Quaternion.Euler(currentX, currentY, 0f);
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

    // Update is called once per frame
    void Update()
    {
        Movement();
        CameraMovement();
    }

}