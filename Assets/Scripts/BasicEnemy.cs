using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
    protected enum State
    {
        idle,
        moving,
        shooting,
        dead
    };
    protected State state = State.idle;

    // moving
    public float stoppingDistance;
    float targetHeight;
    public float speed;

    // shooting
    public GameObject bullet;
    List<Transform> bulletSpawns = new List<Transform>();
    public float minFireDelay;
    public float maxFireDelay;
    float fireDelay;

    // vitals

    // visuals
    Color enemyColor;

    // references
    Collider2D col;

	void Start() {
        // setup references
        enemyColor = GetComponent<SpriteRenderer>().color;
        col = GetComponent<Collider2D>();

        // setup bullet spawns
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "BulletSpawn")
            {
                bulletSpawns.Add(child);
            }
        }

        MoveInward(stoppingDistance); // need to factor in planet radius
	}

    void MoveInward (float _targetHeight) {
        targetHeight = _targetHeight;
        state = State.moving;
    }

    void StartShooting () {
        fireDelay = maxFireDelay;
        state = State.shooting;
    }

    void Shoot () {
        foreach (var spawn in bulletSpawns)
        {
            GameObject newBullet = Instantiate(bullet, spawn.position, spawn.rotation); //bullets point in
            newBullet.GetComponent<Bullet>().Setup(3f, enemyColor, col);
            Destroy(newBullet, 2f);
        }
    }

    void Update() {
        if (state == State.moving) {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
            if (transform.position.magnitude <= targetHeight) {
                // end movement
                StartShooting();
            }
        } else if (state == State.shooting) {
            fireDelay -= Time.deltaTime;
            if (fireDelay <= 0f) {
                Shoot();
                fireDelay = Random.Range(minFireDelay, maxFireDelay);
            }
        }
    }
}
