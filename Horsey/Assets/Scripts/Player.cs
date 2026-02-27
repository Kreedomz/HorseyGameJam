using NUnit.Framework.Internal.Filters;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    const int MIN_STAMINA = 0;
    const int MAX_STAMINA = 100;

    [Header("Stamina Settings")]
    // Set the stamina of the player to the max on game start
    [SerializeField] int currentStamina = MAX_STAMINA;
    // How much player stamina is drained per second
    [SerializeField] int drainStaminaPerSecond = 1;

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

    IEnumerator DrainStamina()
    {
        while (currentStamina > MIN_STAMINA)
        {
            int staminaAfterDrain = currentStamina - drainStaminaPerSecond;

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

    public void GiveStamina(int staminaToGive)
    {
        int totalAmountAfterAddition = currentStamina + staminaToGive;

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

    private void OnDrawGizmos()
    {
        RenderGroundCheck();
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
}
