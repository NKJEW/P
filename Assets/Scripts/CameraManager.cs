using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager instance;

    public float maxRadius;
    public float minRadius;
    IEnumerator curTransitionSequence;
    public float moveTime;

    Vector3 zOffset;

    void Awake() {
        instance = this;
        zOffset = new Vector3(0, 0, transform.position.z);
    }

    public void Move(float newPercent) {
        if (curTransitionSequence != null) {
            StopCoroutine(curTransitionSequence);
        }
        curTransitionSequence = TransitionSequence(Mathf.Lerp(maxRadius, minRadius, newPercent));
        StartCoroutine(curTransitionSequence);
    }

    IEnumerator TransitionSequence(float goalRadius) {
        float curRadius = transform.localPosition.y;
        float p = 0;

        while (p < 1) {
            transform.localPosition = Vector3.up * Mathf.Lerp(curRadius, goalRadius, p) + zOffset;
            yield return new WaitForEndOfFrame();
            p += Time.deltaTime / moveTime;
        }

        transform.localPosition = Vector3.up * goalRadius + zOffset;
        curTransitionSequence = null;
    }
}
