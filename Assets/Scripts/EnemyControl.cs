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

    private EnemyAI ai;

    void Awake()
    {
        ai = GetComponent<EnemyAI>();
    }

    void OnEnable()
    {
        HP = maxHP;
        UpdateHPUI();
        if (ai != null) ai.enabled = true;
    }

    void OnDisable()
    {
        if (ai != null) ai.enabled = false;
    }

    public void Gethit(float damage)
    {
        HP -= damage;
        UpdateHPUI();

        if (HP <= 0)
        {
            if (bombEffect != null)
            {
                var effect = Instantiate(bombEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            if (WaveManager.Instance != null)
                WaveManager.Instance.OnEnemyKilled();
            var hud = FindObjectOfType<PlayerHUD>();
            if (hud != null) hud.AddKill();

            gameObject.SetActive(false);
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
