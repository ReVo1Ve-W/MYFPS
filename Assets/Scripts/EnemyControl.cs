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
    private ObjectPool pool;
    private static PlayerHUD cachedHUD;

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

            WaveManager.Instance?.OnEnemyKilled();

            if (cachedHUD == null)
                cachedHUD = FindObjectOfType<PlayerHUD>();
            cachedHUD?.AddKill();

            if (pool == null)
                pool = FindObjectOfType<ObjectPool>();
            if (pool != null)
                pool.Despawn(gameObject);
            else
                gameObject.SetActive(false);
        }
    }

    public void UpdateHPUI()
    {
        if (hpFill != null)
            hpFill.fillAmount = HP / maxHP;
        if (hpText != null)
            hpText.text = $"HP {HP:F0} / {maxHP}";
    }
}
