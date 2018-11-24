using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public float spawnRate;
    public float spawnHeight;
    public GameObject enemy;

    float nextSpawn;

    void Update() {
        if (Time.time > nextSpawn) {
            nextSpawn = Time.time + spawnRate;
            Spawn(enemy);
        }
    }

    void Spawn(GameObject obj) {
        float angle = Random.Range(0f, 360f);
        float rad = Mathf.Deg2Rad * angle;
        Vector3 spawnPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * spawnHeight;

        Instantiate(obj, spawnPos, Quaternion.Euler(0, 0, angle));
    }
}
