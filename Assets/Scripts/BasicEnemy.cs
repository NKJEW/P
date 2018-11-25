using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
    protected enum State
    {
        idle,
        moving,
        shooting,
        dead
    };
    protected State state = State.idle;

    // moving
    public float stoppingDistance;
    float targetHeight;
    public float speed;

    // shooting
    public GameObject bullet;
    List<Transform> bulletSpawns = new List<Transform>();
    public float minFireDelay;
    public float maxFireDelay;
    float fireDelay;

    // vitals

    // visuals
    Color enemyColor;

    // references
    Collider2D col;

	void Start() {
        // setup references
        enemyColor = GetComponent<SpriteRenderer>().color;
        col = GetComponent<Collider2D>();

        // setup bullet spawns
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "BulletSpawn")
            {
                bulletSpawns.Add(child);
            }
        }

        MoveInward(stoppingDistance); // need to factor in planet radius
	}

    void MoveInward (float _targetHeight) {
        targetHeight = _targetHeight;
        state = State.moving;
    }

    void StartShooting () {
        fireDelay = maxFireDelay;
        state = State.shooting;
    }

    void Shoot () {
        foreach (var spawn in bulletSpawns)
        {
            GameObject newBullet = Instantiate(bullet, spawn.position, spawn.rotation); //bullets point in
            newBullet.GetComponent<Bullet>().Setup(3f, enemyColor, col);
            Destroy(newBullet, 2f);
        }
    }

    void Update() {
        if (state == State.moving) {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
            if (transform.position.magnitude <= targetHeight) {
                // end movement
                StartShooting();
            }
        } else if (state == State.shooting) {
            fireDelay -= Time.deltaTime;
            if (fireDelay <= 0f) {
                Shoot();
                fireDelay = Random.Range(minFireDelay, maxFireDelay);
            }
        }
    }

    public void Die () {
        Shatter();
        Destroy(gameObject);
    }

    [Header("Shattering")]
    public GameObject fragmentPrefab;
    public float shatterForce;
    public float shatterSpin;

    public void Shatter()
    {
        //ShatterQuad(transform, GetComponent<PolygonCollider2D>().GetPath(0));
        Vector2[] edgePath = new Vector2[4] { new Vector2(0.5f, 0.5f), new Vector2(0.5f, -0.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f) };
        ShatterQuad(transform, edgePath);
    }

    public void ShatterQuad(Transform transform, Vector2[] edgePath)
    {
        Vector3[] meshVerts = new Vector3[edgePath.Length];
        Vector3 middlePoint = new Vector3(0f, 0f, meshVerts[0].z);

        // adapt 2d edge path to 3d verticies
        for (int i = 0; i < edgePath.Length; i++)
        {
            meshVerts[i] = new Vector3(edgePath[i].x, edgePath[i].y, transform.position.z);
        }

        for (int i = 0; i < meshVerts.Length; i++)
        {
            // define new verticies of triangular mesh
            List<Vector3> shatteredMeshVerts = new List<Vector3>();
            shatteredMeshVerts.Add(middlePoint);
            shatteredMeshVerts.Add(meshVerts[i]);
            shatteredMeshVerts.Add(meshVerts[(i + 1) % meshVerts.Length]);

            // apply verticies to mesh
            Mesh newMesh = new Mesh();
            newMesh.SetVertices(shatteredMeshVerts);

            // set basic mesh triangles
            int[] triangles = new int[3] { 0, 1, 2 };
            newMesh.SetTriangles(triangles, 0);

            // create fragment gameObject to which the mesh is applied
            GameObject fragment = Instantiate(fragmentPrefab, transform.position, transform.rotation);

            // scale and rotate the object to make it properly alligned with the old object quad
            fragment.transform.localScale = transform.localScale;
            fragment.GetComponent<MeshFilter>().mesh = newMesh;

            // apply force and spin to fragment
            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            rb.AddForce((Vector2)Random.insideUnitCircle.normalized * shatterForce);
            rb.AddTorque(Random.Range(-shatterSpin, shatterSpin));
        }
    }
}
