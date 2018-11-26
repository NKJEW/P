using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player instance;

    public bool isMobile;

    public float movementSpeed; //how fast the player goes around the planet
    float rotationSpeed; //how fast the player goes around in degrees per second
    float rollSpeed; //how fast the player is rolling

    float playerRadius;
    Color playerColor;
    Collider2D playerCollider;

    public GameObject line;
    public Transform shotSpawn;

    public GameObject bullet;

    Camera gameCam;
    Planet planet;

    void Awake() {
        instance = this;

        playerRadius = transform.localScale.x / 2;
        playerColor = GetComponent<SpriteRenderer>().color;
        playerCollider = GetComponent<Collider2D>();
    }

    void Start() {
        line.SetActive(false);
    }

    public void UpdateRadius() {
        transform.localPosition = Vector3.up * (Planet.radius + playerRadius);
        rotationSpeed = (movementSpeed / Planet.radius) * Mathf.Rad2Deg;
        rollSpeed = rotationSpeed * Planet.radius / playerRadius;
    }

    void FixedUpdate() {
        float input = -movementInput;
        transform.parent.Rotate(new Vector3(0, 0, input * rotationSpeed * Time.deltaTime));

        transform.Rotate(new Vector3(0, 0, input * rollSpeed * Time.deltaTime));
    }

    void Update() {
        if (shootDown) {
            line.SetActive(true);
        }

        if (shootUp) {
            line.SetActive(false);
            GameObject newBullet = Instantiate(bullet, shotSpawn.position, shotSpawn.rotation);
            newBullet.GetComponent<Bullet>().Setup(10, playerColor, playerCollider);
            Destroy(newBullet, 2f);
        }
    }

    public void RegisterHit() {
        
    }

    float movementInput { get { return (!isMobile) ? Input.GetAxis("Horizontal") : Input.acceleration.x; } }
    bool shootDown { get { return (!isMobile) ? Input.GetKeyDown(KeyCode.Space) : Input.GetMouseButtonDown(0); } }
    bool shootUp { get { return (!isMobile) ? Input.GetKeyUp(KeyCode.Space) : Input.GetMouseButtonUp(0); } }
}
