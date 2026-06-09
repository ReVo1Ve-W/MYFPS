using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("刷怪池")]
    public ObjectPool enemyPool;

    [Header("波次配置")]
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 15f;
    public float spawnInterval = 1.5f;      // 同一波内每个怪间隔

    [Header("难度递增")]
    public int baseEnemyCount = 3;
    public int enemiesPerWave = 2;           // 每波多 2 个
    public float hpScalePerWave = 0.15f;     // 每波 +15% HP
    public float speedScalePerWave = 0.05f;  // 每波 +5% 速度

    [Header("UI")]
    public GameObject waveText;              // 波次提示文字（可选）

    private int currentWave = 0;
    private int enemiesAlive;
    private int enemiesToSpawn;
    private float waveTimer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        // 等几秒再开始第一波
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

            // 分批生成这一波的所有敌人
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            // 等所有敌人死完
            while (enemiesAlive > 0)
                yield return new WaitForSeconds(1f);

            Debug.Log($"Wave {currentWave} cleared! Next wave in {timeBetweenWaves}s...");

            // 下一波前等待
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("WaveManager: 没有设置刷怪点！");
            return;
        }

        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        var enemy = enemyPool.Spawn("Enemy", point.position, point.rotation);
        if (enemy == null) return;

        // 难度递增：提升 HP 和速度
        var ec = enemy.GetComponent<EnemyControl>();
        if (ec != null)
        {
            ec.HP = ec.maxHP * (1 + (currentWave - 1) * hpScalePerWave);
            ec.maxHP = ec.HP;
        }

        var ai = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (ai != null)
        {
            ai.speed *= (1 + (currentWave - 1) * speedScalePerWave);
        }
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
}
