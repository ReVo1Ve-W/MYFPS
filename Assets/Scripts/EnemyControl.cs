using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyControl : MonoBehaviour
{
    public float HP = 100f;
    public float maxHP = 100f;
    public GameObject bombEffect;

    public Image hpFill;
    public TextMeshProUGUI hpText;

    void Start()
    {
        UpdateHPUI();
    }

    public void Gethit(float damage)
    {
        HP -= damage;
        UpdateHPUI();

        if (HP <= 0)
        {
            var effect = Instantiate(bombEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
            Destroy(gameObject);
        }
    }

    void UpdateHPUI()
    {
        if (hpFill != null)
            hpFill.fillAmount = HP / maxHP;
        if (hpText != null)
            hpText.text = $"HP {HP:F0} / {maxHP}";
    }
}
