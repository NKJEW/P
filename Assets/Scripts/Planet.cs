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

        Player.instance.UpdateRadius();
    }
}
