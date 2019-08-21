﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : CharacterBase {

    protected enum RandOption { SHOT_TIME, BULLET_SPEED}

    public bool IsBoss { get; set; }

    [SerializeField]
    protected bool isMoving;
    [SerializeField]
    protected bool isMortal;
    [SerializeField]
    protected bool isShooting;
    [SerializeField]
    protected float enemySpeed;
    [SerializeField]
    protected GameObject enemyBullet;
    [SerializeField]
    protected Slider slider;

    protected Vector3 enemyPosition;
    protected float actualTime;

    // Start is called before the first frame update
    protected virtual void Start() {
        actualTime = ShootTime();
        IsBoss = false;
    }

    protected void FixedUpdate() {
        if (isMoving) Moving();
        if (isShooting) Shooting();
    }

    public override void GetDamage(int damage) {
        if (isMortal) {
            base.GetDamage(damage);
            if (HP <= 0) {
                GameController.AddEnemyKills();
                slider.value++;
            }
        }
    }

    protected virtual void Shooting() {

        if (actualTime <= 0) {
            var copyEnemyBullet = Instantiate(enemyBullet, enemyPosition, new Quaternion(0, 0, 0, 1));
            copyEnemyBullet.SetActive(true);
            copyEnemyBullet.GetComponent<BulletBase>().BulletSpeed -= GetRandomValue(RandOption.BULLET_SPEED);
            copyEnemyBullet.GetComponent<BulletBase>().BulletSpeed -= (GameController.GameLevel * 0.01f);
            ShootTime();
        }
        else
            actualTime -= Time.deltaTime;
    }

    protected virtual void Moving() {
        this.gameObject.transform.Translate(new Vector3(3.0f, 0, 0) * enemySpeed);
        enemyPosition = this.gameObject.transform.position;
        if (enemyPosition.x > 10 || enemyPosition.x < -10)
            enemySpeed *= -1;
    }

    protected float ShootTime() {
        float shootTime = GetRandomValue(RandOption.SHOT_TIME);
        actualTime = shootTime;
        return shootTime;
    }

    protected virtual float GetRandomValue(RandOption option)
    {
        switch (option)
        {
            case RandOption.SHOT_TIME:
                if (GameController.GameLevel >= 10)
                    return Random.Range(1.0f, 3.0f);
                else if (GameController.GameLevel >= 5)
                    return Random.Range(2.0f, 6.0f);
                else
                    return Random.Range(3.0f, 9.0f);

            case RandOption.BULLET_SPEED:
                if (GameController.GameLevel >= 10)
                    return Random.Range(0.05f, 0.1f);
                else if (GameController.GameLevel >= 5)
                    return Random.Range(0, 0.05f);
                else
                    return Random.Range(-0.025f, 0.025f);
            default:
                return 0.0f;
        }     
    }

    public virtual void ActivateEnemy(bool isActive, bool isBoss)
    {
        isMoving = isActive;
        isMortal = isActive;
        isShooting = isActive;

        if (isBoss)
            HP = GameController.GameLevel;
        else
            HP = (GameController.GameLevel / 5) + 1;
        }
    

}
