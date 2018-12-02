using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    public GameObject indicatorPrefab;
    Canvas canvas;

    public GameObject gameOverScreen;
    public GameObject winScreen;
    public Text tapToText;

    bool canTap;

    void Awake() {
        instance = this;
        canvas = FindObjectOfType<Canvas>();

        gameOverScreen.SetActive(false);
        winScreen.SetActive(false);
        tapToText.gameObject.SetActive(false);
    }

    public Indicator CreateIndicator(GameObject _newTarget) {
        GameObject edgeViewObj = Instantiate(indicatorPrefab, canvas.transform);
        Indicator newIndicator = edgeViewObj.GetComponent<Indicator>();
        newIndicator.Init(canvas);
        newIndicator.SetTarget(_newTarget);
        return newIndicator;
    }

    public void ShowGameOver() {
        gameOverScreen.SetActive(true);
        tapToText.text = "Tap to retry";
        Invoke("AllowTap", 1f);
    }

    public void ShowWin() {
        winScreen.SetActive(true);
        tapToText.text = "Tap to continue";
        Invoke("AllowTap", 1f);
    }

    void AllowTap() {
        canTap = true;
        tapToText.gameObject.SetActive(true);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && canTap) {
            GameManager.instance.RegisterUITap();
        }
    }
}
