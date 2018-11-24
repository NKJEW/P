using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float degreesPerSecond;
    public Transform player;
    public Transform planet;
    public GameObject line;

    public GameObject bullet;

    float playerRotateRate;
    Color playerColor;
    Collider2D playerCol;

    void Start() {
        playerRotateRate = degreesPerSecond * planet.transform.localScale.x / player.transform.localScale.x;
        player.transform.position = Vector3.up * ((planet.transform.localScale.x / 2) + (player.transform.localScale.y / 2));
        playerColor = player.GetComponent<SpriteRenderer>().color;
        playerCol = player.GetComponent<Collider2D>();
        line.SetActive(false);
    }

    void FixedUpdate() {
        float input = -Input.acceleration.x;
        transform.Rotate(new Vector3(0, 0, input * degreesPerSecond * Time.deltaTime));

        player.Rotate(new Vector3(0, 0, input * playerRotateRate * Time.deltaTime));
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            line.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0)) {
            line.SetActive(false);
            GameObject newBullet = Instantiate(bullet, player.transform.position + (player.up), player.rotation);
            newBullet.GetComponent<Bullet>().Setup(10, playerColor, playerCol);
            Destroy(newBullet, 2f);
        }
    }
}
