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
    Collider2D playerCollider;

    public GameObject line;
    [Header("Shooting")]
    public Transform shotSpawn;
    public float rechargeTime;
    float rechargeTimer = 0;
    public ShotData[] shotDatas;
    int[] shots; //-1 if empty, otherwise the number associated with the shot id
    int numShots = 0;
    List<SpriteRenderer> shotIndicators = new List<SpriteRenderer>();
    public Color shotEmptyColor;

    Camera gameCam;
    Planet planet;

    void Awake() {
        instance = this;

        playerRadius = transform.localScale.x / 2;
        playerCollider = GetComponent<Collider2D>();
    }

    void Start() {
        line.SetActive(false);
        Transform shotsContainer = transform.Find("Shots");
        for (int i = 0; i < shotsContainer.childCount; i++) {
            shotIndicators.Add(shotsContainer.GetChild(i).GetComponent<SpriteRenderer>());
        }
        shots = new int[shotIndicators.Count];
        for (int i = 0; i < shotIndicators.Count; i++) {
            shots[i] = -1; //initialize list
        }
        UpdateShotIndicators();
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
        if (numShots < shots.Length) {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= rechargeTime) {
                AddShot(0);
                rechargeTimer = 0f;
            }
        }

        if (numShots > 0) {
            if (shootDown) {
                line.SetActive(true);
            }
            if (shootUp) {
                line.SetActive(false);
                Shoot();
            }
        }
    }

    void AddShot(int shotType) {
        int idToReplaceAt = -1;
        for (int i = 0; i < shots.Length; i++) { //iterate through the shots we have
            if (shots[i] == -1) { //replace if the slot is empty
                shots[i] = shotType;
                numShots++;
                UpdateShotIndicators();
                return;
            }
            if (GetDataForSlotIndex(i).priority < shotDatas[shotType].priority) {
                idToReplaceAt = i;
            }
        }

        //if we made it through the list and we found a possible slot to replace
        if (idToReplaceAt != -1) {
            shots[idToReplaceAt] = shotType;
            UpdateShotIndicators();
        }
    }

    void UpdateShotIndicators () {
        for (int i = 0; i < shotIndicators.Count; i++) {
            bool hasShot = shots[i] != -1;
            Color color = (hasShot) ? GetDataForSlotIndex(i).ammoColor : shotEmptyColor;
            shotIndicators[i].color = color;
        }
    }

    ShotData GetDataForSlotIndex(int id) {
        return shotDatas[shots[id]];
    }

    void Shoot () {
        numShots--;
        int curHighestPriority = -1;
        int slotIdToRemove = -1;
        for (int i = 0; i < shots.Length; i++) {
            if (shots[i] != -1 && GetDataForSlotIndex(i).priority > curHighestPriority) {
                slotIdToRemove = i;
                curHighestPriority = GetDataForSlotIndex(i).priority;
            }
        }

        GameObject newBullet = Instantiate(GetDataForSlotIndex(slotIdToRemove).bulletPrefab, shotSpawn.position, shotSpawn.rotation);
        newBullet.GetComponent<Bullet>().Setup(10, playerCollider);
        Destroy(newBullet, 2f);

        shots[slotIdToRemove] = -1;

        UpdateShotIndicators();
    }

    public void RegisterHit() {
        AddShot(1);
    }

    public void RegisterGameOver() {
        line.SetActive(false);
        enabled = false;
    }

    float movementInput { get { return (!isMobile) ? Input.GetAxis("Horizontal") : Input.acceleration.x; } }
    bool shootDown { get { return (!isMobile) ? Input.GetKeyDown(KeyCode.Space) : Input.GetMouseButtonDown(0); } }
    bool shootUp { get { return (!isMobile) ? Input.GetKeyUp(KeyCode.Space) : Input.GetMouseButtonUp(0); } }

    [System.Serializable]
    public struct ShotData {
        public Material ammoMat;
        public GameObject bulletPrefab;
        public int priority;

        public Color ammoColor { get { return ammoMat.color; } }
    }
}
