using UnityEngine;
using System.Collections.Generic;

public class Snake : MonoBehaviour
{
    enum MovingDirection
    {
        Left,
        Right
    }

    [Header("Ground Check Settings")]
    [SerializeField] LayerMask[] layersToIgnore;
    [SerializeField] float groundCheckDetection = 0.15f;

    [Header("Movement Settings")]
    [SerializeField] MovingDirection movingDirection = MovingDirection.Left; // Start the snake moving left by default on creation
    [SerializeField] float moveSpeed = 5.0f;

    Rigidbody2D rb;

    Vector3 leftFootCollider;
    Vector3 rightFootCollider;

    Vector3 leftWallCollider;
    Vector3 rightWallCollider;

    CircleCollider2D circleCollider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    bool CheckGameObjectLayerIsAllowed(GameObject gameObjectCheck)
    {
        foreach (LayerMask layerMask in layersToIgnore)
        {
            if (gameObjectCheck.layer == layerMask.value)
            {
                return false; 
            }
        }

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleWallColliders();
        HandleGroundColliders();
    }

    void HandleMovement()
    {
        // Moving left
        if (movingDirection == MovingDirection.Left)
        {
            Vector2 moveLeftDirection = new Vector2(-transform.right.x * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = moveLeftDirection;

            // SNAKE SPRITE IMAGE IS INVERTED, BY DEFAULT THE SNAKE IS LOOKING LEFT
            RotateSnakeSprite(1.0f); // -1 = Left | +1 = Right
        }
        // Moving right
        else
        {
            Vector2 moveRightDirection = new Vector2(transform.right.x * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = moveRightDirection;

            // SNAKE SPRITE IS INVERTED, BY DEFAULT THE SNAKE IS LOOKING LEFT
            RotateSnakeSprite(-1.0f);
        }
    }


    // Used to detect if the snake is colliding with a wall and reverse its movement to the other direciton
    void HandleWallColliders()
    {
        if (circleCollider == null) return;

        // Update the colliders location
        leftWallCollider = new Vector3(transform.position.x - circleCollider.radius, transform.position.y, transform.position.z);
        rightWallCollider = new Vector3(transform.position.x + circleCollider.radius, transform.position.y, transform.position.z);

        // Check if wall colliders have hit a wall and if so, flip the snake's moving direction
        List<GameObject> collidingGameObjects = AreEitherCoordsTouchingColliders(leftWallCollider, rightWallCollider);
        if (collidingGameObjects != null && collidingGameObjects.Count > 0)
        {
            foreach (GameObject colliderGameObject in collidingGameObjects)
            {
                if (CheckGameObjectLayerIsAllowed(colliderGameObject))
                {
                    FlipMovementDirection();
                }
            }
        }
    }

    // Used to detect if the snake is going to fall of the map/platform and then reverse its movement
    void HandleGroundColliders()
    {
        if (circleCollider == null) return;

        // Update the colliders location
        leftFootCollider = new Vector3(transform.position.x - circleCollider.radius, transform.position.y - circleCollider.radius, transform.position.z);
        rightFootCollider = new Vector3(transform.position.x + circleCollider.radius, transform.position.y - circleCollider.radius, transform.position.z);
    }

    List<GameObject> AreEitherCoordsTouchingColliders(Vector3 vectorA, Vector3 vectorB)
    {
        // Container for all the objects the colliders are colliding with
        List<GameObject> collidingGameObjects = new List<GameObject>();

        Collider2D[] colliderA = Physics2D.OverlapCircleAll(vectorA, groundCheckDetection);
        Collider2D[] colliderB = Physics2D.OverlapCircleAll(vectorB, groundCheckDetection);

        // Check if the only thing the colliders are colliding with is the SNAKE
        if ((colliderA.Length == 1 && colliderA[0].gameObject == this.gameObject) && (colliderB.Length == 1 && colliderB[0].gameObject == this.gameObject))
        {
            return null;
        }

        // Loop through the first vector's colliders
        foreach(Collider2D collider in colliderA)
        {
            // If the current iteration has found the SNAKE, ignore and continue to the next iteration
            if (colliderA.Length == 1 && colliderA[0].gameObject == this.gameObject)
            {
                continue;
            }

            collidingGameObjects.Add(collider.gameObject);
        }

        // Loop through the second vector's colliders
        foreach (Collider2D collider in colliderB)
        {
            // If the current iteration has found the SNAKE, ignore and continue to the next iteration
            if (colliderB.Length == 1 && colliderB[0].gameObject == this.gameObject)
            {
                continue;
            }

            collidingGameObjects.Add(collider.gameObject);
        }

        return collidingGameObjects;
    }

    void FlipMovementDirection()
    {
        if (movingDirection == MovingDirection.Left)
        {
            movingDirection = MovingDirection.Right;
        }
        else
        {
            movingDirection = MovingDirection.Left;
        }
    }

    // Roate the snakes's sprite depending on its moving direction
    void RotateSnakeSprite(float movingDirection)
    {
        if (movingDirection == 0) return; // Ignore if no movement is passed to the func

        // If input is moving left
        if (movingDirection < 0.0f)
        {
            transform.localScale = new Vector3(-1.0f, transform.localScale.y, transform.localScale.z);
        }
        // Input is going right
        else
        {
            transform.localScale = new Vector3(1.0f, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (circleCollider == null) return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(leftFootCollider, groundCheckDetection);
        Gizmos.DrawWireSphere(rightFootCollider, groundCheckDetection);
        Gizmos.DrawWireSphere(leftWallCollider, groundCheckDetection);
        Gizmos.DrawWireSphere(rightWallCollider, groundCheckDetection);
    }
}
