using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public PlayerControl player;
    public TextMeshProUGUI hpText;
    public Image hpFill;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerControl>();
    }

    void Update()
    {
        if (player == null) return;

        float hp = Mathf.Max(0, player.playerHP);
        float maxHP = player.maxHP;

        hpFill.fillAmount = hp / maxHP;
        hpText.text = $"HP {hp:F0} / {maxHP}";
    }
}
