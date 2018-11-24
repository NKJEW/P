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

    Collider2D col;
    float curHealth;

    void Start() {
        curHealth = maxHealth;

        mainPlanet.transform.localScale = new Vector3(initialRadius * 2, initialRadius * 2, 1);
        core.transform.localScale = new Vector3(coreRadius * 2, coreRadius * 2, 1);

        UpdateRadius();
    }

    void UpdateRadius() {
        float healthPercent = curHealth / maxHealth;
        radius = coreRadius + healthPercent * (initialRadius - coreRadius);

        Player.instance.UpdateRadius();
    }
}
