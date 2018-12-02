using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    enum GameState {
        InGame = 0,
        GameOver = 1,
        Won = 2
    }

    public Level[] levels;
    public static int loadedLevel = 0;

    GameState state;

    // waves
    int waveIndex = 0;
    float nextWaveTime;
    bool stillSpawning = true;
    int enemiesAlive = 0;

    void Awake() {
        instance = this;
    }

    void Start () {
        state = GameState.InGame;
        StartLevel();
	}

    void StartLevel () {
        print("Starting level " + loadedLevel);
        nextWaveTime = levels[loadedLevel].waves[0].triggerTime;
    }
	
	void Update () {
        if (stillSpawning) {
            if (Time.time > nextWaveTime) {
                StartWave(waveIndex);
                waveIndex++;
                // test to see if this wave is the last one
                if (waveIndex >= levels[loadedLevel].waves.Length) {
                    print("All waves spawned");
                    stillSpawning = false;
                } else {
                    nextWaveTime = levels[loadedLevel].waves[waveIndex].triggerTime;
                }
            }
        }
	}

    void StartWave (int waveIndex) {
        print("Spawning Wave " + waveIndex + " at " + Time.time + " seconds");
        EnemyManager.instance.SpawnWave(levels[loadedLevel].waves[waveIndex]);
    }

    public void EnemyAdded () {
        enemiesAlive++;
    }

    public void EnemyRemoved() {
        enemiesAlive--;
        if (!stillSpawning && enemiesAlive <= 0) {
            LevelComplete();
        }
    }

    public void GameOver() {
        if (state != GameState.InGame) {
            return;
        }

        state = GameState.GameOver;
        Player.instance.RegisterGameOver();
        UIManager.instance.ShowGameOver();
    }

    void LevelComplete () {
        if (state != GameState.InGame) {
            return;
        }

        print("Level Complete");
        state = GameState.Won;
        UIManager.instance.ShowWin();
    }

    public void RegisterUITap() {
        switch (state) {
            case GameState.GameOver:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case GameState.Won:
                loadedLevel++;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }

    [System.Serializable]
    public struct Level {
        public Wave[] waves;
    }

    [System.Serializable]
    public struct Wave {
        public float triggerTime;
        public int[] enemies;
        public int count;
        public float length;
    }
}
