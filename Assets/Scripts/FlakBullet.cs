using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlakBullet : Bullet {
    public GameObject miniBullet;
    public int numBullets;
    public float rotateSpeed;

    Transform child;

	public override void Setup(float speed, Collider2D parentCol) {
        base.Setup(speed, parentCol);
        child = transform.GetChild(0);
	}

	void LateUpdate() {
        child.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (isPlayer && other.gameObject.CompareTag("Enemy")) {
            Explode();
        }
    }

    void Explode() {
        float angle = transform.rotation.eulerAngles.z;
        for (int i = 0; i < numBullets; i++) {
            GameObject newBullet = Instantiate(miniBullet, transform.position, Quaternion.Euler(0, 0, angle));
            newBullet.GetComponent<Bullet>().Setup(5f, null);
            Destroy(newBullet, 0.75f);
            angle += 360f / numBullets;
        }
        Destroy(gameObject);
    }
}
