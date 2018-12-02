using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Indicator : MonoBehaviour {
    GameObject target;

    const float margin = 15f;   // margin from edge of screen
    const float renderMargin = -0.1f;
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

    Camera gameCam;
    RectTransform canvas;
    Image image;

    public void Init(Canvas parentCanvas) {
        gameCam = Camera.main;
        canvas = parentCanvas.GetComponent<RectTransform>();
        image = transform.GetComponentInChildren<Image>();
    }

    public void SetTarget(GameObject _newTarget) {
        target = _newTarget;

        enabled = true;
    }

    void Start() {
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

        image.enabled = false;
    }

    void Update() {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        SetPositionAndRotation();
        UpdateVisibility();
    }

    void SetPositionAndRotation() {
        image.enabled = true;
        float angle = (CalculateAngle() + 360) % 360;

        // position
        Vector2 position = Vector2.zero;

        // assign edgeView to 1 of 4 edges (Top, Right, Bottom, Left)
        if (angle >= TLAngle || angle <= TRAngle) {
            // top
            position.y = TEdge;
            position.x = TEdge * Mathf.Tan((angle) * Mathf.Deg2Rad);

        } else if (angle > TRAngle && angle < BRAngle) {
            // right
            position.x = REdge;
            position.y = REdge * Mathf.Tan((90 - angle) * Mathf.Deg2Rad);
        } else if (angle >= BRAngle && angle <= BLAngle) {
            // bottom
            position.y = BEdge;
            position.x = BEdge * Mathf.Tan((angle) * Mathf.Deg2Rad);
        } else if (angle > BLAngle && angle < TLAngle) {
            // left
            position.x = LEdge;
            position.y = LEdge * Mathf.Tan((270 - angle) * Mathf.Deg2Rad);
        }

        transform.localPosition = position;

        // rotate

        Quaternion targetRotation = Quaternion.Euler(0, 0, -angle);
        transform.rotation = targetRotation;

    }

    float CalculateAngle() { //uses law of cosines to calculate the angle
        float a = ((Vector2)gameCam.transform.position).magnitude;
        float b = Vector2.Distance(gameCam.transform.position, target.transform.position);
        float c = ((Vector2)target.transform.position).magnitude;

        float angle = Mathf.Acos((Mathf.Pow(c, 2) - (Mathf.Pow(a, 2) + Mathf.Pow(b, 2))) / (-2 * a * b)) * Mathf.Rad2Deg + 180;
        float angleDiff = (Mathf.Atan2(gameCam.transform.position.y, gameCam.transform.position.x) - Mathf.Atan2(target.transform.position.y, target.transform.position.x) + 2 * Mathf.PI) % (2 * Mathf.PI);
        if (angleDiff < Mathf.PI) {
            return -angle;
        } else {
            return angle;
        }
    }

    void UpdateVisibility() {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(target.transform.position);
        bool targetIsOnScreen = viewportPoint.x > renderMargin && viewportPoint.x < (1f - renderMargin) && viewportPoint.y > renderMargin && viewportPoint.y < (1f - renderMargin);
        image.enabled = (!targetIsOnScreen);
    }
}
