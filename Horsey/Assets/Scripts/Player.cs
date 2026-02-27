using Mono.Cecil.Cil;
using UnityEngine;

public class Player : MonoBehaviour
{
    const int MAX_STAMINA = 100;

    // Set the stamina of the player to the max on game start
    [SerializeField] int currentStamina = MAX_STAMINA;

    [SerializeField] float moveSpeed = 5.0f;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");

        // If the player is pressing any of the moving keys
        if (horizontalAxis != 0.0f)
        {
            Vector2 moveDirection = (transform.right * horizontalAxis).normalized;
            moveDirection.y = 0.0f;

            rb.linearVelocity = moveDirection * moveSpeed;

            RotatePlayerSprite(horizontalAxis);
        }
    }


    // Roate the player's sprite depending on the moving input
    void RotatePlayerSprite(float movingInput)
    {
        if (movingInput == 0) return; // Ignore if no movement is passed to the func

        // If input is moving left
        if (movingInput < 0.0f)
        {
            transform.localScale = new Vector3(-1.0f, transform.localScale.y, transform.localScale.z);
        }
        // Input is going right
        else
        {
            transform.localScale = new Vector3(1.0f, transform.localScale.y, transform.localScale.z);
        }
    }
}
