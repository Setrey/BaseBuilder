using UnityEngine;

public class Player : Unit
{
    public CharacterController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Movement()
    {
        // Pobieranie inputu (WASD / Strza�ki)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Obliczanie kierunku wzgl�dem rotacji gracza
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * Time.deltaTime * stats.speed);
    }
    // Update is called once per frame
    void Update()
    {
        Movement();
    }
}
