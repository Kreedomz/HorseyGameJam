using UnityEngine;

public class Carrot : MonoBehaviour
{
    enum BounceState
    {
        Up,
        Down
    }

    [Header("Carrot Settings")]
    [SerializeField] float staminaToGive = 20.0f; // How much stamina the player will regain when picked up

    [Header("Bounce Settings")]
    [SerializeField] BounceState bounceState = BounceState.Up;
    [SerializeField] float bounceAmount = 0.25f; // How hight/low from the starting position the carrot should bounce for
    [SerializeField] float bounceSpeed = 1.0f; // How fast the carrot should sway

    Vector3 startPosition = Vector3.zero; // The position we will use to start the bounce from
    
    Vector3 upPosition;
    Vector3 downPosition;

    void Start()
    {
        startPosition = transform.position;

        upPosition = new Vector3(startPosition.x, startPosition.y + bounceAmount, startPosition.z);
        downPosition = new Vector3(startPosition.x, startPosition.y - bounceAmount, startPosition.z);
    }

    void Update()
    {
        HandleBounce();
    }

    void HandleBounce()
    {
        // Go UP
        if (bounceState == BounceState.Up)
        {
            Vector3 movePosition = new Vector3(transform.position.x, transform.position.y + (bounceSpeed * Time.deltaTime), transform.position.z);
            transform.position = movePosition;

            if (transform.position.y >= upPosition.y)
            {
                bounceState = BounceState.Down;
            }

        }
        // Go DOWN
        else
        {
            Vector3 movePosition = new Vector3(transform.position.x, transform.position.y - (bounceSpeed * Time.deltaTime), transform.position.z);
            transform.position = movePosition;

            if (transform.position.y <= downPosition.y)
            {
                bounceState = BounceState.Up;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If player is detected in the carrot collision
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.GiveStamina(staminaToGive); // Give the player stamina on trigger
            Destroy(this.gameObject); // Destroy the carrot after
        }
    }
}
