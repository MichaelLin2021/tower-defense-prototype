using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public Transform waypointsRoot;
    public TextMeshProUGUI waveText;

    public CoreHealth core;

    public int enemiesToSpawn = 10;
    public float spawnInterval = 0.8f;

    public int wave = 1;
    public float timeBetweenWaves = 5f;
    public int extraEnemiesPerWave = 2;
    public int extraHealthPerWave = 8;
    public float extraSpeedPerWave = 0.35f;
    public int extraGoldRewardPerWave = 2;
    public int bonusEnemyEveryWaves = 2;
    public float spawnIntervalReductionPerWave = 0.05f;
    public float minSpawnInterval = 0.3f;
    public float maxSecondsWaitingForWaveClear = 12f;

    private Transform[] path;
    private bool finishedSpawning;

    void Awake()
    {
        if (waypointsRoot == null) return;

        int count = waypointsRoot.childCount;
        path = new Transform[count];
        for (int i = 0; i < count; i++)
            path[i] = waypointsRoot.GetChild(i);
    }

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        while (!finishedSpawning)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                yield break;

            if (waveText != null)
                waveText.text = "Wave: " + wave;

            int countThisWave = enemiesToSpawn + (wave - 1) * extraEnemiesPerWave + (wave - 1) / bonusEnemyEveryWaves;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetWave(wave, countThisWave);
                GameManager.Instance.ShowWaveBreak(GetWaveSummary(countThisWave));
            }

            for (int i = 0; i < countThisWave; i++)
            {
                if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                    yield break;

                GameObject e = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                e.transform.localScale *= 1f + (wave - 1) * 0.08f;

                if (GameManager.Instance != null)
                    GameManager.Instance.RegisterEnemySpawned();

                var health = e.GetComponent<Health>();
                if (health != null)
                {
                    health.AddMaxHp((wave - 1) * extraHealthPerWave);
                    health.goldReward += (wave - 1) * extraGoldRewardPerWave;
                    health.SetWaveVisuals(wave);
                }

                var mover = e.GetComponent<EnemyPath>();
                if (mover != null)
                {
                    mover.waypoints = path;
                    mover.speed += (wave - 1) * extraSpeedPerWave;
                }

                var end = e.GetComponent<ReachEndDamage>();
                if (end != null) end.core = core;

                float currentSpawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - (wave - 1) * spawnIntervalReductionPerWave);
                yield return new WaitForSeconds(currentSpawnInterval);
            }

            float clearTimer = maxSecondsWaitingForWaveClear;
            while (GameManager.Instance != null && GameManager.Instance.EnemiesAlive > 0 && !GameManager.Instance.IsGameOver && clearTimer > 0f)
            {
                clearTimer -= Time.deltaTime;
                yield return null;
            }

            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                yield break;

            if (GameManager.Instance != null && wave >= GameManager.Instance.wavesToWin)
            {
                finishedSpawning = true;
                GameManager.Instance.WinGame();
                yield break;
            }

            wave++;
            float breakTimer = timeBetweenWaves;
            while (breakTimer > 0f)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.ShowWaveBreak(breakTimer);

                breakTimer -= Time.deltaTime;
                yield return null;
            }

            if (GameManager.Instance != null)
                GameManager.Instance.ClearStatus();
        }
    }

    private string GetWaveSummary(int enemyCount)
    {
        int bonusHp = (wave - 1) * extraHealthPerWave;
        float bonusSpeed = (wave - 1) * extraSpeedPerWave;
        return $"Wave {wave}: {enemyCount} enemies | +{bonusHp} HP | +{bonusSpeed:0.0} speed";
    }
}
