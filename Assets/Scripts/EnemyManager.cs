using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager instance;

    [Header("Spawning")]
    GameManager.Wave curWave;
    bool spawning = false;
    float spawnRate;
    public float spawnHeight;
    float spawnTimer;
    int spawnQue = 0;

    public GameObject[] enemies;
    List<BasicEnemy> activeEnemies = new List<BasicEnemy>();

    private void Awake()
    {
        instance = this;
    }

    public void SpawnWave (GameManager.Wave wave) {
        curWave = wave;
        spawnQue = curWave.count;
        spawnRate = curWave.length / curWave.count;
        spawnTimer = spawnRate;

        spawning = true;
    }

    void Update() {
        if (spawning) {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f) {
                int enemyIndex = curWave.enemies[Random.Range(0, curWave.enemies.Length)];
                GameObject enemy = enemies[enemyIndex];
                Spawn(enemy);

                spawnQue--;
                if (spawnQue > 0) {
                    spawnTimer = spawnRate;
                } else {
                    spawning = false;
                }
            }
        }
    }

    void Spawn(GameObject obj) {
        float angle = Random.Range(0f, 360f);
        float rad = Mathf.Deg2Rad * angle;
        Vector3 spawnPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * spawnHeight;

        GameObject newEnemy = Instantiate(obj, spawnPos, Quaternion.Euler(0, 0, angle + 90f));
        activeEnemies.Add(newEnemy.GetComponent<BasicEnemy>());
    }

    public void EnemyDied (BasicEnemy enemy) {
        activeEnemies.Remove(enemy);
    }

    public void PlanetRadiusUpdated (float newRadius) {
        foreach (var enemy in activeEnemies)
        {
            enemy.PlanetRadiusUpdated(newRadius);
        }
    }

    // ENEMY INDICATORS
    //[Header("Indicators")]
    //public GameObject indicatorPrefab;

}
