using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
    public float endHeight;
    public GameObject bullet;
    public float fallTime;

    Color enemyColor;
    Collider2D col;

	void Start() {
        enemyColor = GetComponent<SpriteRenderer>().color;
        col = GetComponent<Collider2D>();
        StartCoroutine(Go());
	}

	IEnumerator Go() {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos.normalized * endHeight;
        float p = 0;
        while (p < 1f) {
            transform.position = Vector3.Lerp(startPos, endPos, p);
            yield return new WaitForEndOfFrame();
            p += (Time.deltaTime / fallTime);
        }

        while (true) {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            Shoot();
        }
    }

    void Shoot() {
        GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation * Quaternion.Euler(0, 0, 90)); //bullets point in
        newBullet.GetComponent<Bullet>().Setup(3f, enemyColor, col);
        Destroy(newBullet, 2f);
    }
}
