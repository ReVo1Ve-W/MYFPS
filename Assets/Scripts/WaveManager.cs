using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("刷怪池")]
    public ObjectPool enemyPool;

    [Header("随机刷怪 — 围绕防御塔")]
    public DefenseTarget defenseTarget;
    public float spawnMinRadius = 20f;
    public float spawnMaxRadius = 40f;

    [Header("波次配置")]
    public float timeBetweenWaves = 15f;
    public float spawnInterval = 1.5f;

    [Header("难度递增")]
    public int baseEnemyCount = 3;
    public int enemiesPerWave = 2;
    public float hpScalePerWave = 0.15f;
    public float speedScalePerWave = 0.05f;

    [Header("UI")]
    public GameObject waveText;

    private int currentWave;
    private int enemiesAlive;
    private int enemiesToSpawn;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (defenseTarget == null)
            defenseTarget = FindObjectOfType<DefenseTarget>();
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            currentWave++;
            enemiesToSpawn = baseEnemyCount + (currentWave - 1) * enemiesPerWave;
            enemiesAlive = enemiesToSpawn;

            if (waveText != null)
            {
                waveText.SetActive(true);
                var txt = waveText.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = $"Wave {currentWave}";
                StartCoroutine(HideWaveText(3f));
            }

            Debug.Log($"Wave {currentWave} started! Enemies: {enemiesToSpawn}");

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            while (enemiesAlive > 0)
                yield return new WaitForSeconds(1f);

            Debug.Log($"Wave {currentWave} cleared! Next wave in {timeBetweenWaves}s...");

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        if (defenseTarget == null) return;

        if (!FindSpawnPoint(out Vector3 point))
        {
            Debug.LogWarning("WaveManager: 找不到有效刷怪点！");
            return;
        }

        var enemy = enemyPool.Spawn("Enemy", point, Quaternion.identity);
        if (enemy == null) return;

        var ec = enemy.GetComponent<EnemyControl>();
        if (ec != null)
        {
            ec.HP = ec.maxHP * (1 + (currentWave - 1) * hpScalePerWave);
            ec.maxHP = ec.HP;
            ec.UpdateHPUI();
        }

        var ai = enemy.GetComponent<NavMeshAgent>();
        if (ai != null)
        {
            ai.speed *= (1 + (currentWave - 1) * speedScalePerWave);
        }
    }

    bool FindSpawnPoint(out Vector3 point)
    {
        Vector3 center = defenseTarget.transform.position;

        for (int i = 0; i < 20; i++)
        {
            Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(spawnMinRadius, spawnMaxRadius);
            Vector3 candidate = center + new Vector3(circle.x, 0, circle.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                point = hit.position;
                return true;
            }
        }

        point = Vector3.zero;
        return false;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }

    IEnumerator HideWaveText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (waveText != null) waveText.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (defenseTarget == null) return;
        Vector3 center = defenseTarget.transform.position;
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(center, spawnMinRadius);
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(center, spawnMaxRadius);
    }
}
