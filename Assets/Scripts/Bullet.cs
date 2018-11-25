using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public Sprite trailSprite;
    public float trailStartSize;
    public bool isPlayer;

    Color color;
    float fireRate;
    float nextFire;

    const float DIST = 0.3f;

    public void Setup(float speed, Color newColor, Collider2D parentCol) {
        color = newColor;
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        GetComponent<SpriteRenderer>().color = color;

        fireRate = DIST / speed;

        Physics2D.IgnoreCollision(parentCol, GetComponent<Collider2D>());
    }

    void Update() {
        if (Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            CreateParticle();
        }
    }

    void CreateParticle() {
        GameObject particle = new GameObject();
        particle.transform.position = transform.position - transform.up * DIST;
        particle.transform.rotation = transform.rotation;
        particle.transform.localScale = new Vector3(trailStartSize, trailStartSize, 1f);
        particle.AddComponent<Particle>().Setup(0.2f, trailSprite, color);
    }

    void OnCollisionEnter2D(Collision2D other) {
        BasicEnemy possibleEnemy = other.gameObject.GetComponent<BasicEnemy>();
        if (isPlayer && possibleEnemy != null) {
            possibleEnemy.Die();
        }

       
        Destroy(gameObject);
    }
}
