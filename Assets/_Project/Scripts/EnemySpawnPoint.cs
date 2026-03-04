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

    private Transform[] path;

    void Awake()
    {
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
        while (true)
        {
            Debug.Log("Wave " + wave);

            if (waveText != null)
                waveText.text = "Wave: " + wave;

            int countThisWave = enemiesToSpawn + (wave - 1) * extraEnemiesPerWave;

            for (int i = 0; i < countThisWave; i++)
            {
                GameObject e = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

                var mover = e.GetComponent<EnemyPath>();
                mover.waypoints = path;

                var end = e.GetComponent<ReachEndDamage>();
                if (end != null) end.core = core;

                yield return new WaitForSeconds(spawnInterval);
            }

            wave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}