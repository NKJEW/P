using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    public GameObject indicatorPrefab;
    public Canvas canvas;

    void Awake() {
        instance = this;
    }

    public Indicator CreateIndicator(GameObject _newTarget) {
        GameObject edgeViewObj = Instantiate(indicatorPrefab, canvas.transform);
        Indicator newIndicator = edgeViewObj.GetComponent<Indicator>();
        newIndicator.Init(canvas);
        newIndicator.SetTarget(_newTarget);
        return newIndicator;
    }
}
