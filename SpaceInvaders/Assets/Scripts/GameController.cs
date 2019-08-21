﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static string NickName;
    public static int GameLevel;
    public static int EnemyKills;

    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private GameObject boss;
    [HideInInspector]
    public List<GameObject> listOfEnemy;
    [SerializeField]
    private WaveController waveController;

    //GL^2 * 50

    void Awake() {
        GameLevel = 1;
        EnemyKills = 0;
    }

    private void Start() {
        StartCoroutine(ActivateNewWave(waveController.Wave+1)); //Wave+1
        InvokeRepeating("CheckEnemies", 5.0f, 5.0f);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    public static void AddEnemyKills() {
        EnemyKills++;

        if (GameLevel * GameLevel * 5 <= EnemyKills)
            GameLevel++;
    }

    private void CheckEnemies() {

        bool time4NewWave = true;

        foreach (var item in listOfEnemy) {
            if (item != null) {
                time4NewWave = false;
                break;
            }
        }

        if(time4NewWave)
            SetNewWave();
    }
    private void SetNewWave()
    {
        StartCoroutine(ActivateNewWave(waveController.Wave+1));
    }

    private void InitEnemiesWave() {

        float x = -7;
        float y = 4;
        listOfEnemy.Clear();
        for (int j = 0; j < 4; j++) {

            for (int i = 0; i < 10; i++) {
                var enemyCopy = Instantiate(enemy, enemy.transform.parent);
                enemyCopy.transform.position = new Vector3(x, y, 0);

                x += 1.5f;
                listOfEnemy.Add(enemyCopy);
            }
            x = -7;
            y -= 1f;
        }
    }

    private void InitBoss() {

        float x = -7;
        float y = 3;
        listOfEnemy.Clear();
        var bossCopy = Instantiate(boss, boss.transform.parent);
        bossCopy.transform.position = new Vector3(x, y, 0);
        listOfEnemy.Add(bossCopy);
    }

    private void ActivateEnemies(bool isBoss)
    {
        foreach (var item in listOfEnemy)
        {
            if (item != null)
            {
                if (!isBoss)
                    item.GetComponent<EnemyBehaviour>().ActivateEnemy(true, false);
                else
                    item.GetComponent<BossBehavior>().ActivateEnemy(true, true);
                item.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator ActivateNewWave(int waveNumber)
    {
        playerController.IsShooting = false;
        waveController.ShowWaveText();
        if (waveNumber % 3 != 0) InitEnemiesWave();
        else InitBoss();
        yield return new WaitForSeconds(3f);
        waveController.HideWaveText();
        if (waveNumber % 3 != 0) ActivateEnemies(false);
        else ActivateEnemies(true);
        playerController.IsShooting = true;

    }
}
