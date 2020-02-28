﻿using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Controller : EnemyBase
{
    public List<GameObject> targets;
    public LineRenderer line1, line2;
    public GameObject eye1, eye2, endLine1, endLine2, allEndLine, effectattack3, effectattack4;
    public override void Start()
    {
        base.Start();
        Init();
    }
    public override void Init()
    {
        base.Init();
        if (!EnemyManager.instance.boss3s.Contains(this))
        {
            EnemyManager.instance.boss3s.Add(this);
        }
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if (EnemyManager.instance.boss3s.Contains(this))
        {
            EnemyManager.instance.boss3s.Remove(this);
        }
    }
    public override void Active()
    {
        base.Active();
        enemyState = EnemyState.idle;
    }
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (!isActive)
        {
            return;
        }
        if (enemyState == EnemyState.die)
            return;
        switch (enemyState)
        {
            case EnemyState.idle:
                if (Mathf.Abs(transform.position.x - Camera.main.transform.position.x) <= 0.5f)
                {
                    if (GameController.instance.uiPanel.CheckWarning())
                    {
                        GameController.instance.uiPanel.warning.SetActive(false);
                        GameController.instance.autoTarget.Add(this);
                        takeDamageBox.enabled = true;
                    }
                    PlayAnim(1, aec.idle, true);
                    enemyState = EnemyState.attack;
                    takeDamageBox.enabled = true;
                    randomCombo = 2;
                    timePreviousAttack = maxtimeDelayAttack1;
                    typeAttack = 0;
                }
                break;
            case EnemyState.attack:
                switch (typeAttack)
                {
                    case 0:
                        Attack1(deltaTime);
                        break;
                    case 1:
                        Attack2(deltaTime);
                        break;
                    case 2:
                        Attack3(deltaTime);
                        break;
                    case 3:
                        Attack4(deltaTime);
                        break;

                }
                break;
            case EnemyState.run:
                Def(deltaTime);
                break;
            case EnemyState.falldown:

                timeChangePos -= deltaTime;

                //if (timeChangePos > 0 && timeChangePos <= 0.5f)
                //{

                //}

                if (timeChangePos <= 0)
                {
                    enemyState = EnemyState.attack;
                    timeChangePos = 1;
                    switch (previousState)
                    {
                        case EnemyState.attack:
                            switch (previousTypeAttack)
                            {
                                case 0:
                                    timePreviousAttack = 2;
                                    typeAttack = 1;
                                    combo = 0;
                                    break;
                                case 1:
                                    typeAttack = 2;
                                    timePreviousAttack = maxtimeDelayAttack1;
                                    randomCombo = 2;
                                    combo = 0;
                                    break;
                                case 2:
                                    typeAttack = 3;
                                    timePreviousAttack = maxtimeDelayAttack3;
                                    randomCombo = Random.Range(5, 7);
                                    combo = 0;
                                    break;
                                case 3:
                                    enemyState = EnemyState.run;
                                    timePreviousAttack = 10;
                                    combo = 0;
                                    break;
                            }
                            break;
                        case EnemyState.run:
                            randomCombo = 2;
                            timePreviousAttack = maxtimeDelayAttack1;
                            typeAttack = 0;
                            combo = 0;
                            break;
                    }
                }
                break;
        }
    }
    public List<float> xVelo = new List<float>(), yVelo = new List<float>();
    void Throw(List<GameObject> target, int i, Vector2 myPos)
    {
        float xdistance;
        xdistance = target[i].transform.position.x - myPos.x;

        float ydistance;
        ydistance = target[i].transform.position.y - myPos.y;

        float throwAngle; // in radian

        throwAngle = Mathf.Atan((ydistance + 5f) / xdistance);

        float totalVelo = xdistance / Mathf.Cos(throwAngle);
        xVelo[i] = totalVelo * Mathf.Cos(throwAngle);
        yVelo[i] = totalVelo * Mathf.Sin(throwAngle);
    }
    void Attack1(float deltaTime)
    {
        timePreviousAttack -= deltaTime;
        if (timePreviousAttack > 0 && timePreviousAttack <= maxtimeDelayAttack1 / 2)
        {
            if (!targets[0].activeSelf)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    pos.y = targets[i].transform.position.y;
                    if (i == 0)
                    {
                        pos.x = PlayerController.instance.GetTranformXPlayer();
                    }
                    else
                    {
                        if (PlayerController.instance.GetTranformXPlayer() <= Camera.main.transform.position.x)
                            pos.x = targets[i - 1].transform.position.x + 2.5f;
                        else
                            pos.x = targets[i - 1].transform.position.x - 2.5f;
                    }
                    targets[i].transform.position = pos;
                    targets[i].SetActive(true);
                }
            }
        }
        else if (timePreviousAttack <= 0)
        {
            PlayAnim(0, aec.attack1, false);

            counttimeshot++;

            bulletEnemy = ObjectPoolManagerHaveScript.Instance.rocketBoss3Pooler.GetBulletEnemyPooledObject();
            bulletEnemy.AddProperties(damage1, bulletspeed1);

            listMyBullet.Add(bulletEnemy);
            if (counttimeshot - 1 < 2)
                bulletEnemy.transform.position = boneBarrelGun.GetWorldPosition(skeletonAnimation.transform);
            else
                bulletEnemy.transform.position = boneBarrelGun1.GetWorldPosition(skeletonAnimation.transform);


            Throw(targets, counttimeshot - 1, bulletEnemy.transform.position);
            bulletEnemy.BeginDisplay(new Vector2(xVelo[counttimeshot - 1], yVelo[counttimeshot - 1]), this);
            bulletEnemy.gameObject.SetActive(true);

            //  Debug.Log("combo:" + counttimeshot);

            if (counttimeshot == 4)
            {
                combo++;
                if (combo == randomCombo)
                {
                    previousState = enemyState;
                    enemyState = EnemyState.falldown;
                    previousTypeAttack = typeAttack;
                    timeChangePos = 1f;
                    for (int i = 0; i < targets.Count; i++)
                    {
                        targets[i].SetActive(false);
                    }
                }
                else
                {
                    timePreviousAttack = maxtimeDelayAttack1;
                    for (int i = 0; i < targets.Count; i++)
                    {
                        targets[i].SetActive(false);
                    }
                }
                counttimeshot = 0;
            }
            else
            {
                timePreviousAttack = maxtimeDelayAttack1 / 3;
            }
        }
    }
    int counttimeshot;
    Vector2 pos;
    float timeChangePos;
    int previousTypeAttack;
    void Attack2(float deltaTime)
    {
        timePreviousAttack -= deltaTime;
        if (timePreviousAttack > 0 && timePreviousAttack <= 1.5f)
        {
            PlayAnim(0, aec.attack2, true);
        }
        else if (timePreviousAttack <= 0 && timePreviousAttack > -5)
        {
            if (!allEndLine.activeSelf)
            {
                pos.x = PlayerController.instance.GetTranformXPlayer();
                pos.y = allEndLine.transform.position.y;
                allEndLine.transform.position = pos;
                allEndLine.SetActive(true);
                eye1.SetActive(true);
                eye2.SetActive(true);
                line1.SetPosition(0, eye1.transform.position);
                line2.SetPosition(0, eye2.transform.position);
                line1.SetPosition(1, endLine1.transform.position);
                line2.SetPosition(1, endLine2.transform.position);
                line1.gameObject.SetActive(true);
                line2.gameObject.SetActive(true);
                timeChangePos = 0;
            }

            line1.SetPosition(0, eye1.transform.position);
            line2.SetPosition(0, eye2.transform.position);
            line1.SetPosition(1, endLine1.transform.position);
            line2.SetPosition(1, endLine2.transform.position);

            if (Mathf.Abs(allEndLine.transform.position.x - PlayerController.instance.GetTranformXPlayer()) > 0.5f)
            {
                pos.x = PlayerController.instance.GetTranformXPlayer();
                pos.y = allEndLine.transform.position.y;
                allEndLine.transform.position = Vector2.MoveTowards(allEndLine.transform.position, pos, deltaTime * bulletspeed2 / 10);
            }
            timeChangePos -= deltaTime;
            if (timeChangePos <= 0)
            {
                boxAttack1.gameObject.SetActive(true);
                boxAttack2.gameObject.SetActive(true);
                timeChangePos = maxtimeDelayAttack2;
            }
        }
        else if (timePreviousAttack <= -5)
        {
            allEndLine.SetActive(false);

            previousState = enemyState;
            enemyState = EnemyState.falldown;
            previousTypeAttack = typeAttack;
            timeChangePos = 1f;

            PlayAnim(0, aec.idle, true);
            eye1.SetActive(false);
            eye2.SetActive(false);
            line1.gameObject.SetActive(false);
            line2.gameObject.SetActive(false);
        }
    }
    void Attack3(float deltaTime)
    {
        timePreviousAttack -= deltaTime;
        if (timePreviousAttack <= 0)
        {
            PlayAnim(0, aec.attack3, false);

            combo++;
            Debug.LogError("combo:" + combo);
            if (combo == randomCombo)
            {
                previousState = enemyState;
                enemyState = EnemyState.falldown;
                previousTypeAttack = typeAttack;
                timeChangePos = 1f;
                combo = 0;
            }
            else
            {
                timePreviousAttack = maxtimeDelayAttack1 * 3;
                Debug.LogError("reset:");
            }
        }
    }
    void Attack4(float deltaTime)
    {
        timePreviousAttack -= deltaTime;
        if (timePreviousAttack <= 0)
        {
            PlayAnim(0, aec.run, false);

            combo++;
            bulletEnemy = ObjectPoolManagerHaveScript.Instance.tialuaBoss3Pooler.GetBulletEnemyPooledObject();
            bulletEnemy.transform.position = rightFace.transform.position;
            bulletEnemy.AddProperties(damage3, bulletspeed3);
            bulletEnemy.BeginDisplay(Vector2.zero, this);
            listMyBullet.Add(bulletEnemy);
            rotation.x = 0;
            rotation.y = 0;
            rotation.z = Random.Range(-360, 360);
            bulletEnemy.transform.eulerAngles = rotation;
            bulletEnemy.gameObject.SetActive(true);

            if (combo == randomCombo)
            {
                previousState = enemyState;
                enemyState = EnemyState.falldown;
                previousTypeAttack = typeAttack;
                timeChangePos = 1f;
                effectattack4.SetActive(false);
                PlayAnim(0, aec.idle, true);
                combo = 0;
            }
            else
            {
                timePreviousAttack = maxtimeDelayAttack3 / 2;
                Debug.LogError("reset:");
            }
        }
    }
    void Def(float deltaTime)
    {
        if (!isShield)
        {
            PlayAnim(0, aec.run2, false);
            isShield = true;
        }
        timePreviousAttack -= deltaTime;
        if (timePreviousAttack <= 0)
        {
            previousState = enemyState;
            enemyState = EnemyState.falldown;
            previousTypeAttack = typeAttack;
            timeChangePos = 1f;
            PlayAnim(0, aec.idle, true);
            isShield = false;
        }

    }
    Vector3 rotation;
    protected override void OnEvent(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnEvent(trackEntry, e);
        // if (trackEntry.Animation.Name.Equals(aec.attack1.name))
        // {
        //     PlayAnim(0, aec.idle, true);
        // }
        //else if (trackEntry.Animation.Name.Equals(aec.attack2.name))
        // {
        //     PlayAnim(0, aec.idle, true);
        // }
        if (trackEntry.Animation.Name.Equals(aec.attack3.name))
        {
            effectattack3.SetActive(true);

            bulletEnemy = ObjectPoolManagerHaveScript.Instance.quacauluaBoss3Pooler.GetBulletEnemyPooledObject();
            bulletEnemy.transform.position = leftFace.transform.position;
            bulletEnemy.AddProperties(damage1, bulletspeed1);
            bulletEnemy.BeginDisplay(Vector2.zero, this);
            listMyBullet.Add(bulletEnemy);
            rotation.x = 0;
            rotation.y = 0;
            rotation.z = 0;
            bulletEnemy.transform.eulerAngles = rotation;
            bulletEnemy.gameObject.SetActive(true);

            bulletEnemy = ObjectPoolManagerHaveScript.Instance.quacauluaBoss3Pooler.GetBulletEnemyPooledObject();
            bulletEnemy.transform.position = leftFace.transform.position;
            bulletEnemy.AddProperties(damage1, bulletspeed1);
            bulletEnemy.BeginDisplay(Vector2.zero, this);
            listMyBullet.Add(bulletEnemy);
            rotation.x = 0;
            rotation.y = -180;
            rotation.z = 0;
            bulletEnemy.transform.eulerAngles = rotation;
            bulletEnemy.gameObject.SetActive(true);
        }
        else if (trackEntry.Animation.Name.Equals(aec.run.name))
        {
            effectattack4.SetActive(true);
        }
    }
    protected override void OnComplete(TrackEntry trackEntry)
    {
        base.OnComplete(trackEntry);
        if (trackEntry.Animation.Name.Equals(aec.attack1.name))
        {
            PlayAnim(0, aec.idle, true);
        }
        else if (trackEntry.Animation.Name.Equals(aec.attack2.name))
        {
            //  PlayAnim(0, aec.idle, true);
        }
        else if (trackEntry.Animation.Name.Equals(aec.attack3.name))
        {
            PlayAnim(0, aec.idle, true);
        }
    }
    public override void Dead()
    {
        base.Dead();
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].SetActive(false);
        }
        eye1.SetActive(false);
        eye2.SetActive(false);
        allEndLine.SetActive(false);
        line1.gameObject.SetActive(false);
        line2.gameObject.SetActive(false);
        effectattack3.SetActive(false);
        effectattack4.SetActive(false);
    }
}
