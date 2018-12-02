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
    [Header("Shooting")]
    public Transform shotSpawn;
    public GameObject bullet;
    public float rechargeTime;
    float rechargeTimer = 0;
    int shots = 0;
    List<SpriteRenderer> shotIndicators = new List<SpriteRenderer>();
    public Color shotEmptyColor;

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
        Transform shotsContainer = transform.Find("Shots");
        for (int i = 0; i < shotsContainer.childCount; i++)
        {
            shotIndicators.Add(shotsContainer.GetChild(i).GetComponent<SpriteRenderer>());
        }
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
        if (shots < shotIndicators.Count) {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= rechargeTime) {
                shots++;
                rechargeTimer = 0f;
                UpdateShotIndicators();
            }
        }

        if (shots > 0) {
            if (shootDown) {
                line.SetActive(true);
            }
            if (shootUp) {
                line.SetActive(false);
                Shoot();
            }
        }
    }

    void UpdateShotIndicators () {
        for (int i = 0; i < shotIndicators.Count; i++) {
            bool hasShot = (i < shots);
            Color color = (hasShot) ? Color.white : shotEmptyColor;
            shotIndicators[i].color = color;
        }
    }

    void Shoot () {
        GameObject newBullet = Instantiate(bullet, shotSpawn.position, shotSpawn.rotation);
        newBullet.GetComponent<Bullet>().Setup(10, playerColor, playerCollider);
        Destroy(newBullet, 2f);

        shots--;
        UpdateShotIndicators();
    }

    public void RegisterHit() {
        
    }

    float movementInput { get { return (!isMobile) ? Input.GetAxis("Horizontal") : Input.acceleration.x; } }
    bool shootDown { get { return (!isMobile) ? Input.GetKeyDown(KeyCode.Space) : Input.GetMouseButtonDown(0); } }
    bool shootUp { get { return (!isMobile) ? Input.GetKeyUp(KeyCode.Space) : Input.GetMouseButtonUp(0); } }
}
