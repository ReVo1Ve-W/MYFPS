using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public PlayerControl player;
    public Text hpText;
    public Slider hpSlider;

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

        hpSlider.value = hp / maxHP;
        hpText.text = $"HP {hp:F0} / {maxHP}";
    }
}
