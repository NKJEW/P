﻿using UnityEngine;
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

    Camera gameCam;
    Vector3 offset; //offset from center
    RectTransform canvas;
    GameObject image;

    public void Init(Canvas parentCanvas) {
        gameCam = Camera.main;
        canvas = parentCanvas.GetComponent<RectTransform>();
        image = transform.GetComponentInChildren<Image>().gameObject;
    }

    public void SetTarget(GameObject _newTarget) {
        target = _newTarget;

        offset = gameCam.transform.forward * Mathf.Sqrt(2) * (gameCam.transform.position.y - target.transform.position.y); //gets point in the center of the camera's view

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
        Vector3 targetVector = (target.transform.position - (gameCam.transform.position + offset));
        float angle = CalculateAngle(transform.forward, targetVector) - gameCam.transform.rotation.eulerAngles.z; //rotate based on camera's rotation

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

    void UpdateVisibility() {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(target.transform.position);
        bool targetIsOnScreen = viewportPoint.x > margin && viewportPoint.x < (1f - margin) && viewportPoint.y > margin && viewportPoint.y < (1f - margin) && viewportPoint.z > 0;
        image.SetActive(!targetIsOnScreen);
    }

    //From https://gist.github.com/shiwano/0f236469cd2ce2f4f585
    public static float CalculateAngle(Vector3 from, Vector3 to) {
        return Quaternion.FromToRotation(from, to).eulerAngles.y;
    }
}
