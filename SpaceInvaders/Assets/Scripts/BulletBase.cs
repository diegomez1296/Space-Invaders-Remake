﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour {

    [SerializeField]
    private PlayerBehaviour player;
    [SerializeField]
    private BonusController bonusController;

    public float bulletSpeed;
    public bool isPlayerBullet;
    [SerializeField]
    private int bulletDamage;
    
    //AIM
    public bool isAimBullet;
    public bool isAimPlayerBullet;

    private Vector2 aimDirection;
    private Vector2 aimDirectionPlayer;

    //Rocket Effect
    [SerializeField]
    private GameObject GameBox;

    private void FixedUpdate() {
        if(this.gameObject.activeSelf) 
        {
            Moving();
            if(isAimBullet && GameController.GameIsRunning)
                Moving2();
            if (isAimPlayerBullet && GameController.GameIsRunning)
                MovingForAimPlayerBullet();
            DestroyMoment();
        }
    }

    private void Moving() 
    {
        this.gameObject.transform.Translate(new Vector3(0, 1, 0) * bulletSpeed );
    }
    private void Moving2() {
            aimDirection = (player.transform.position - this.transform.position).normalized * -bulletSpeed * 0.5f;
            this.gameObject.transform.Translate(aimDirection);
    }

    private void MovingForAimPlayerBullet() {
        aimDirectionPlayer = (Camera.main.transform.position - this.transform.position).normalized * bulletSpeed;
        this.gameObject.transform.Translate(aimDirectionPlayer);
        StartCoroutine(PlayerRocketExposion(50));
    }

    private void DestroyMoment() 
    {
        if (this.gameObject.transform.position.y > 10 || this.gameObject.transform.position.y < -10)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (this.gameObject.activeSelf) {
            if (collision.GetComponent<PlayerController>() && !isPlayerBullet) {
                player.GetDamage(bulletDamage, Vector2.zero, 100);
                Destroy(this.gameObject);
            }

            if (collision.GetComponent<EnemyBehaviour>() && isPlayerBullet && !isAimPlayerBullet) {
                var enemy = collision.GetComponent<EnemyBehaviour>();
                enemy.GetDamage(bulletDamage, enemy.transform.position, 100);
                Destroy(this.gameObject);
                if(enemy.HP <= 0)
                player.AddScore(enemy.IsBoss);
                player.CheckLevelUI();
            }
        }
    }

    private IEnumerator PlayerRocketExposion(int percentToExplosion) {
        yield return new WaitForSeconds(1);

        var enemies = GameBox.GetComponentsInChildren<EnemyBehaviour>();
        foreach (var item in enemies) {
            item.GetDamage(5, item.transform.position, percentToExplosion);
        }
        player.CheckLevelUI();
        Destroy(this.gameObject);
    }
}