using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image staminaForegroundImage;
    [SerializeField] TextMeshProUGUI staminaText;
    Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerStaminaBar();
        UpdatePlayerStaminaText();
    }

    void UpdatePlayerStaminaBar()
    {
        float staminaPercentage = player.GetStamina() / Player.MAX_STAMINA;
        staminaForegroundImage.fillAmount = staminaPercentage;
    }

    void UpdatePlayerStaminaText()
    {
        staminaText.text = player.GetStamina().ToString() + "/" + Player.MAX_STAMINA.ToString();
    }
}
