using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public const float MIN_STAMINA = 0.0f;
    public const float MAX_STAMINA = 100.0f;

    [Header("Camera Settings")]
    [SerializeField] Transform cameraTransform; // The camera which will follow the player
    [SerializeField] bool followCameraY = true; // If the camera should follow the vertical position of the player

    [Header("Stamina Settings")]
    // Set the stamina of the player to the max on game start
    [SerializeField] float currentStamina = MAX_STAMINA;
    // How much player stamina is drained per second
    [SerializeField] float drainStaminaPerSecond = 1.0f;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float jumpSpeed = 7.5f;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    float groundCheckRadius = 0.25f;

    CircleCollider2D circleCollider;
    Rigidbody2D rb;

    IEnumerator drainStaminaCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStamina = MAX_STAMINA; // Override the stamina again in case a value in the inspector exceeds the MAX_STAMINA
        
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        // Start draining the player's stamina on game start-up
        drainStaminaCoroutine = DrainStamina();
        StartCoroutine(drainStaminaCoroutine);
    }


    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJumping();

        CameraFollow();
    }

    void HandleMovement()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        // If the player is pressing any of the moving keys
        if (horizontalAxis != 0.0f)
        {
            Vector2 moveDirection = new Vector2((transform.right * horizontalAxis).normalized.x * moveSpeed, rb.linearVelocity.y);
            //moveDirection.y = rb.linearVelocity.y;

            rb.linearVelocity = moveDirection;

            RotatePlayerSprite(horizontalAxis);
        }
        // They aren't pressing any movement keys
        else
        {
            // Stop the horizontal force/velocity
            rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
        }
    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            if (IsPlayerOnGround())
            {
                Vector2 jumpingVector = new Vector2(rb.linearVelocity.x, jumpSpeed);

                rb.linearVelocity = jumpingVector;
            }
        }
    }

    void CameraFollow()
    {
        if (cameraTransform == null) return;

        if (followCameraY)
        {
            Vector3 followVector = new Vector3(transform.position.x, transform.position.y, cameraTransform.position.z);
            cameraTransform.position = followVector;

        }
        else
        {
            Vector3 followVector = new Vector3(transform.position.x, cameraTransform.position.y, cameraTransform.position.z);
            cameraTransform.position = followVector;
        }
    }

    IEnumerator DrainStamina()
    {
        while (currentStamina > MIN_STAMINA)
        {
            float staminaAfterDrain = currentStamina - drainStaminaPerSecond;

            if (staminaAfterDrain > MIN_STAMINA)
            {
                currentStamina -= drainStaminaPerSecond;
            }
            else
            {
                currentStamina = 0;
            }


            // Wait for a couple seconds
            yield return new WaitForSeconds(drainStaminaPerSecond);
        }

        yield return null;
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

    public void GiveStamina(float staminaToGive)
    {
        float totalAmountAfterAddition = currentStamina + staminaToGive;

        // Check if we exceed the player's max limit of stamina allowed
        if (totalAmountAfterAddition > MAX_STAMINA)
        {
            currentStamina = MAX_STAMINA;
        }
        else
        {
            currentStamina += staminaToGive;
        }
    }

    bool IsPlayerOnGround()
    {
        Vector3 leftFoot = new Vector3(transform.position.x - circleCollider.radius / 2, transform.position.y - circleCollider.radius, 0.0f);
        Vector3 rightFoot = new Vector3(transform.position.x + circleCollider.radius / 2, transform.position.y - circleCollider.radius, 0.0f);
        Vector3 underPlayer = new Vector3(transform.position.x, transform.position.y - circleCollider.radius, 0.0f);

        Collider2D[] leftFootColliders = Physics2D.OverlapCircleAll(leftFoot, groundCheckRadius);
        Collider2D[] rightFootColliders = Physics2D.OverlapCircleAll(leftFoot, groundCheckRadius);
        Collider2D[] underPlayerColliders = Physics2D.OverlapCircleAll(underPlayer, groundCheckRadius);

        // Check if something is detected in the colliders
        if (leftFootColliders.Length > 0 || rightFootColliders.Length > 0)
        {
            // Check if the only thing detected in the colliders was the player himself and ignore him
            if (leftFootColliders.Length == 1 && leftFootColliders[0].gameObject == this.gameObject &&
                rightFootColliders.Length == 1 && leftFootColliders[0].gameObject == this.gameObject &&
                underPlayerColliders.Length == 1 && leftFootColliders[0].gameObject == this.gameObject)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player/horse collides with another object, in this case (an enemy = snake)
        if (collision.gameObject.GetComponent<Snake>() != null)
        {
            Snake snake = collision.gameObject.GetComponent<Snake>();

            // Check if the horse/player's legs is on top of the snake when the collision has happened
            Vector3 underPlayer = new Vector3(transform.position.x, transform.position.y - circleCollider.radius, 0.0f);
            if (underPlayer.y > snake.transform.position.y)
            {
                Destroy(snake.gameObject);
            }
        }
    }

    void RenderGroundCheck()
    {
        if (circleCollider == null) return;
        Gizmos.color = Color.red;

        Vector3 leftFoot = new Vector3(transform.position.x - circleCollider.radius / 2, transform.position.y - circleCollider.radius, 0.0f);
        Vector3 rightFoot = new Vector3(transform.position.x + circleCollider.radius / 2, transform.position.y - circleCollider.radius, 0.0f);
        Vector3 underPlayer = new Vector3(transform.position.x, transform.position.y - circleCollider.radius, 0.0f);

        Gizmos.DrawWireSphere(leftFoot, groundCheckRadius);
        Gizmos.DrawWireSphere(rightFoot, groundCheckRadius);
        Gizmos.DrawWireSphere(underPlayer, groundCheckRadius);
    }

    private void OnDrawGizmos()
    {
        RenderGroundCheck();
    }

    public float GetStamina()
    {
        return currentStamina;
    }

}
