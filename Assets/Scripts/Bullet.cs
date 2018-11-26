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

        Physics2D.IgnoreCollision(parentCol, GetComponent<Collider2D>()); //TODO: remove
    }

    void Update() {
        if (Time.time > nextParticleEmit) {
            nextParticleEmit = Time.time + emitRate;
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

    void OnTriggerEnter2D(Collider2D other) {
        if (isPlayer && other.gameObject.GetComponent<BasicEnemy>() != null) { //TODO: remove
            Destroy(other.gameObject);
        }

        if (isPlayer) {
            if (other.gameObject.CompareTag("Enemy")) {
                Destroy(other.gameObject); //TODO: replace with broadcast
                other.GetComponent<BasicEnemy>().Die();
            }

            if (!other.gameObject.CompareTag("Player")) {
                Destroy(gameObject);
            }
        } else {
            if (!other.gameObject.CompareTag("Enemy")) {
                other.gameObject.BroadcastMessage("RegisterHit");
                Destroy(gameObject);
            }
        }
    }
}
