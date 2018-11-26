using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    public Transform mainPlanet;
    public Transform core;

    [Space(15)]
    public float initialRadius;
    public float coreRadius;
    public float maxHealth;

    public static float radius;

    CircleCollider2D col;
    float curHealth;

    void Start() {
        col = GetComponent<CircleCollider2D>();

        curHealth = maxHealth;

        core.transform.localScale = new Vector3(coreRadius * 2, coreRadius * 2, 1);

        UpdateRadius();

        SpawnTerrain();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            curHealth--;
            UpdateRadius();
        }
    }

    public void RegisterHit() {
        if (curHealth <= 0) {
            return;
        }

        curHealth--; //temporary behavior
        UpdateRadius();
    }

    void UpdateRadius() {
        float healthPercent = curHealth / maxHealth;
        radius = coreRadius + healthPercent * (initialRadius - coreRadius);

        mainPlanet.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        col.radius = radius;

        UpdateTerrain();
        Player.instance.UpdateRadius();
    }

    [Header("Terrain")]
    public GameObject[] terrainPrefabs;
    List<GameObject> terrainObjects = new List<GameObject>();
    public float minSpawnStep;
    public float maxSpawnStep;

    void SpawnTerrain () {
        float curAngle = 0f;
        while (curAngle < 360f - minSpawnStep) {
            SpawnTerrainObject(curAngle);
            curAngle += Random.Range(minSpawnStep, maxSpawnStep);
        }
    }

    void SpawnTerrainObject (float angle) {
        Vector3 spawnPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 3f);
        Quaternion spawnRot = Quaternion.Euler(0f, 0f, angle - 90f);
        GameObject newObject = (GameObject)Instantiate(terrainPrefabs[Random.Range(0, terrainPrefabs.Length)], spawnPos, spawnRot, transform);
        terrainObjects.Add(newObject);
    }

    void UpdateTerrain () {
        foreach (var terrainObj in terrainObjects) {
            Vector2 pos2D = new Vector2(terrainObj.transform.position.x, terrainObj.transform.position.y);
            Vector2 newPos2D = pos2D.normalized * radius;
            terrainObj.transform.position = new Vector3(newPos2D.x, newPos2D.y, transform.position.z);
        }
    }
}
