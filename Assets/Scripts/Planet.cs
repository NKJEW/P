using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    [Header("Planet")]
    public Transform mainPlanet;
    public Transform core;

    [Header("Shrinking")]
    public AnimationCurve shrinkCurve;
    public float shrinkTime;
    IEnumerator curShrinkSequence;

    [Header("Stats")]
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
        mainPlanet.transform.localScale = new Vector3(initialRadius * 2, initialRadius * 2, 1);

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
        if (curShrinkSequence != null) {
            StopCoroutine(curShrinkSequence);
        }
        curShrinkSequence = ShrinkSequence();
        StartCoroutine(curShrinkSequence);
    }

    IEnumerator ShrinkSequence() {
        float startRadius = mainPlanet.localScale.x / 2;

        float healthPercent = curHealth / maxHealth;
        float endRadius = coreRadius + healthPercent * (initialRadius - coreRadius);
        float p = 0f;

        col.radius = endRadius;

        while (p < 1f) {
            radius = Mathf.LerpUnclamped(startRadius, endRadius, shrinkCurve.Evaluate(p));
            UpdatePlanet();
            yield return new WaitForEndOfFrame();
            p += Time.deltaTime / shrinkTime;
        }

        radius = endRadius;
        UpdatePlanet();
        curShrinkSequence = null;
    }

    void UpdatePlanet() {
        mainPlanet.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
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
