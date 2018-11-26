using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Indicator : MonoBehaviour {
    GameObject target;

    const float margin = 15f;   // margin from edge of screen
    float edgeOffset;
    float BEdge;
    float TEdge;
    float LEdge;
    float REdge;

    // constants storing screen corner angles (T = top, R = right, B = bottom, L = left)
    float TRAngle;
    float BRAngle;
    float BLAngle;
    float TLAngle;

    float canvasWidth;
    float canvasHeight;

    bool hasWorldIndicator;

    Camera gameCam;
    Vector3 offset; //offset from center
    RectTransform canvas;
    GameObject image;
    GameObject worldIndicator;

    static GameObject screenIndicatorPrefab;
    static bool hasSetPrefabs = false;

    public static Indicator Create(GameObject _newTarget)
    {
        Indicator edgeView = Create();
        edgeView.SetTarget(_newTarget);
        return edgeView;
    }

    public static Indicator Create()
    {
        if (!hasSetPrefabs)
        {
            LoadPrefab();
        }

        GameObject edgeViewObj = Instantiate(screenIndicatorPrefab, GameManager.instance.transform.parent.GetComponentInChildren<Canvas>().transform); //always use the canvas in LevelAssets
        Indicator edgeView = edgeViewObj.GetComponent<Indicator>();
        edgeView.Init();

        return edgeView;
    }

    static void LoadPrefab()
    {
        screenIndicatorPrefab = Resources.Load("Indicator") as GameObject;
    }

    public void Init()
    {
        gameCam = Camera.main;
        canvas = GetComponent<RectTransform>();
        image = transform.GetComponentInChildren<Image>().gameObject;
    }

    public void SetTarget(GameObject _newTarget)
    {
        target = _newTarget;

        offset = gameCam.transform.forward * Mathf.Sqrt(2) * (gameCam.transform.position.y - target.transform.position.y); //gets point in the center of the camera's view

        //try to get the health component
        // DESTROY ON DEATH STUFF

        enabled = true;
    }

    public void Hide()
    {
        image.SetActive(false);
        enabled = false;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        edgeOffset = margin + (this.GetComponent<RectTransform>().rect.width / 2);

        canvasWidth = canvas.rect.width;
        canvasHeight = canvas.rect.height;

        BEdge = -canvasHeight / 2 + edgeOffset;
        TEdge = canvasHeight / 2 - edgeOffset;
        LEdge = -canvasWidth / 2 + edgeOffset;
        REdge = canvasWidth / 2 - edgeOffset;

        // calculate angles of the 4 corners of the screen
        TRAngle = Mathf.Atan(REdge / TEdge) * Mathf.Rad2Deg;
        BRAngle = (Mathf.PI + Mathf.Atan(REdge / BEdge)) * Mathf.Rad2Deg;
        BLAngle = (Mathf.PI + Mathf.Atan(LEdge / BEdge)) * Mathf.Rad2Deg;
        TLAngle = ((2 * Mathf.PI) + Mathf.Atan(LEdge / TEdge)) * Mathf.Rad2Deg;
    }

    void Update()
    {
        if (target == null)
        {
            this.Destroy();
            return;
        }

        SetPositionAndRotation();
        UpdateVisibility();
    }

    void SetPositionAndRotation()
    {
        Vector3 targetVector = (target.transform.position - (gameCam.transform.position + offset));
        float angle = CalculateAngle(transform.forward, targetVector) - gameCam.transform.rotation.eulerAngles.y; //rotate based on camera's rotation

        // position
        Vector2 position = Vector2.zero;

        // assign edgeView to 1 of 4 edges (Top, Right, Bottom, Left)
        if (angle >= TLAngle || angle <= TRAngle)
        {
            // top
            position.y = TEdge;
            position.x = TEdge * Mathf.Tan((angle) * Mathf.Deg2Rad);

        }
        else if (angle > TRAngle && angle < BRAngle)
        {
            // right
            position.x = REdge;
            position.y = REdge * Mathf.Tan((90 - angle) * Mathf.Deg2Rad);
        }
        else if (angle >= BRAngle && angle <= BLAngle)
        {
            // bottom
            position.y = BEdge;
            position.x = BEdge * Mathf.Tan((angle) * Mathf.Deg2Rad);
        }
        else if (angle > BLAngle && angle < TLAngle)
        {
            // left
            position.x = LEdge;
            position.y = LEdge * Mathf.Tan((270 - angle) * Mathf.Deg2Rad);
        }

        transform.localPosition = position;

        // rotate

        Quaternion targetRotation = Quaternion.Euler(0, 0, -angle);
        transform.rotation = targetRotation;

    }

    void UpdateVisibility()
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(target.transform.position);
        bool targetIsOnScreen = viewportPoint.x > margin && viewportPoint.x < (1f - margin) && viewportPoint.y > margin && viewportPoint.y < (1f - margin) && viewportPoint.z > 0;
        image.SetActive(!targetIsOnScreen);
    }

    //From https://gist.github.com/shiwano/0f236469cd2ce2f4f585
    public static float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(from, to).eulerAngles.y;
    }
}
