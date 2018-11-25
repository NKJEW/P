using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : MonoBehaviour {
    public float gravity;
    public float lifeTime;
    float timer;
    Vector3 startScale;

    Rigidbody2D rb;
	
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        startScale = transform.localScale;
        timer = lifeTime;
	}
	
	void Update () {
        timer -= Time.deltaTime;
        if (lifeTime <= 0f) {
            Destroy(gameObject);
        } else {
            float timeRatio = timer / lifeTime;
            transform.localScale = Vector3.Lerp(new Vector3(0f, 0f, 1f), startScale, timeRatio);
        }

        rb.AddForce(-transform.position.normalized * gravity * Time.deltaTime);
	}
}
