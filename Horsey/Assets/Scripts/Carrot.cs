using UnityEngine;

public class Carrot : MonoBehaviour
{
    [SerializeField] int staminaToGive = 20; // How much stamina the player will regain when picked up

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
