using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public Sprite trailSprite;
    public float trailStartSize;
    public bool isPlayer;

    Color color;
    float emitRate;
    float nextParticleEmit;

    const float DIST = 0.3f;

    public void Setup(float speed, Color newColor, Collider2D parentCol) {
        color = newColor;
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        GetComponent<SpriteRenderer>().color = color;

        emitRate = DIST / speed;

        Physics2D.IgnoreCollision(parentCol, GetComponent<Collider2D>());
    }

    void Update() {
        if (Time.time > nextParticleEmit) {
            nextParticleEmit = Time.time + emitRate;
            CreateParticle();
        }
    }

    void CreateParticle() {
        GameObject particle = new GameObject("Particle");
        particle.transform.position = transform.position - transform.up * DIST;
        particle.transform.rotation = transform.rotation;
        particle.transform.localScale = new Vector3(trailStartSize, trailStartSize, 1f);
        particle.AddComponent<Particle>().Setup(0.2f, trailSprite, color);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (isPlayer) {
            if (other.collider.gameObject.CompareTag("Enemy")) {
                other.collider.GetComponent<BasicEnemy>().Die();
            }

            if (!other.collider.gameObject.CompareTag("Player")) {
                Destroy(gameObject);
            }
        } else {
            if (!other.collider.gameObject.CompareTag("Enemy")) {
                other.collider.gameObject.BroadcastMessage("RegisterHit");
                Destroy(gameObject);
            }
        }
    }
}
