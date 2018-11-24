using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Level[] levels;
    public int loadedLevel = 0;

    // waves
    int waveIndex = 0;
    float nextWaveTime;
    bool stillSpawning = true;

    void Awake() {
        instance = this;
    }

    void Start () {
        StartLevel();
	}

    void StartLevel () {
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
        EnemySpawner.instance.SpawnWave(levels[loadedLevel].waves[waveIndex]);
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
