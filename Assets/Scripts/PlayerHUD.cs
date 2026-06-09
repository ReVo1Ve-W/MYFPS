using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public PlayerControl player;
    public TextMeshProUGUI hpText;
    public Image hpFill;
    public TextMeshProUGUI killCountText;

    private int killCount;

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
        killCountText.text = $"Kill Count: {killCount}";
    }

    public void AddKill()
    {
        killCount++;
    }
}
