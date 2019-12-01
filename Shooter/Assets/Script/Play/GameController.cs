﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

[System.Serializable]
public class AssetSpinePlayerController
{
    public AnimationReferenceAsset waitstandAnim, falldownAnim, jumpAnim, sitAnim, idleAnim, runForwardAnim, runBackAnim, aimTargetAnim, fireAnim, grenadeAnim, dieAnim;
}

[System.Serializable]
public class AssetSpineEnemyController
{
    public AnimationReferenceAsset attack1, attack2, attack3, idle, run, aimTargetAnim, run2, die, jumpOut;
}

public class GameController : MonoBehaviour
{
    public UIPanel uiPanel;
    public List<MapController> listMap;

    public MapController currentMap;

    public static int indexMap = 0;

    public GameObject targetDetectSprite;
    public List<EnemyBase> autoTarget;
    public GameObject UIControll;
    public enum GameState
    {
        begin,
        play,
        gameover
    }
    public GameState gameState = GameState.begin;
    public UltimateJoystick joystickMove, joystickShot;
    public static GameController instance;
    [HideInInspector]
    public Vector2 movePosition, shootPosition;


    private void Awake()
    {

        instance = this;

#if UNITY_EDITOR
        Application.targetFrameRate = 300;
#else
        Application.targetFrameRate = 60;
#endif
        gameState = GameState.play;
    }
    private void Start()
    {
        currentMap = Instantiate(listMap[indexMap]);
        currentMap.transform.position = Vector2.zero;

        CameraController.instance.Init();

        PlayerController.instance.transform.position = currentMap.pointBeginPlayer.transform.position;
    }
    //   public EnemyBase currentEnemyTarget;
    public void RemoveTarget(EnemyBase enemy)
    {
        if (autoTarget.Contains(enemy))
            autoTarget.Remove(enemy);
    }
    private void JoystickMovement(UltimateJoystick joystick)
    {
        movePosition = new Vector3(joystick.GetHorizontalAxis(), joystick.GetVerticalAxis(), 0);
        if (joystick.GetJoystickState())
        {
            OnMove(movePosition);
        }
        else
        {
            if (PlayerController.instance == null)
                return;
            StopMove();
        }
    }
    public void StopMove()
    {
        if (PlayerController.instance.playerState == PlayerController.PlayerState.Jump)
            return;
        if (PlayerController.instance.playerState != PlayerController.PlayerState.Idle)
        {
            PlayerController.instance.playerState = PlayerController.PlayerState.Idle;
            if (PlayerController.instance.isfalldow)
            {
                PlayerController.instance.isWaitStand = true;
            }
        }
    }

    public void CheckAfterJump(UltimateJoystick joystick)
    {

        movePosition = new Vector3(joystick.GetHorizontalAxis(), joystick.GetVerticalAxis(), 0);

        var angle = Mathf.Atan2(movePosition.x, movePosition.y) * Mathf.Rad2Deg;
        var h = movePosition.x;
        if (angle <= 135f && angle >= -135.5f)
        {
            PlayerController.instance.speedmove = h > 0 ? 1.5f : -1.5f;
            PlayerController.instance.dirMove = h > 0 ? true : false;
            PlayerController.instance.playerState = PlayerController.PlayerState.Run;
        }
        else if ((angle > -180f && angle < -135f) || (angle > 135f && angle < 180f))
        {
            PlayerController.instance.playerState = PlayerController.PlayerState.Sit;
        }
    }

    float getSpeed()
    {
        float speedMovetemp = !PlayerController.instance.isSlow ? PlayerController.instance.speedMoveMax : (PlayerController.instance.speedMoveMax - (PlayerController.instance.speedMoveMax / 100 * 50));
        return speedMovetemp;
    }
    public void OnMove(Vector2 axis)
    {

        var angle = Mathf.Atan2(axis.x, axis.y) * Mathf.Rad2Deg;
        var h = axis.x;
        //   PlayerController.instance.FlipX = h > 0 ? false : true;
        if (angle <= 135f && angle >= -135.5f)
        {
            PlayerController.instance.speedmove = h > 0 ? 1 * getSpeed() : -1 * getSpeed();
            PlayerController.instance.dirMove = h > 0 ? false : true;
            if (PlayerController.instance.playerState == PlayerController.PlayerState.Jump)
                return;
            PlayerController.instance.playerState = PlayerController.PlayerState.Run;

        }
        else if ((angle > -180f && angle < -135f) || (angle > 135f && angle < 180f))
        {
            if (PlayerController.instance.playerState == PlayerController.PlayerState.Jump)
                return;
            PlayerController.instance.playerState = PlayerController.PlayerState.Sit;
        }

        if (joystickShot.GetJoystickState())
            return;
        if (!PlayerController.instance.haveTarget)
            PlayerController.instance.FlipX = h < 0;
    }
    private void JoystickShooting(UltimateJoystick joystick)
    {
        if (PlayerController.instance == null)
            return;

        shootPosition = new Vector3(joystick.GetHorizontalAxis(), joystick.GetVerticalAxis(), 0);

        if (joystick.GetJoystickState())
        {
            TryShot();
            PlayerController.instance.isBouderJoystick = joystick.GetDistance() >= 0.9f;


            if (autoTarget.Count == 0)
            {
                if (PlayerController.instance.isBouderJoystick)
                {
                    PlayerController.instance.FlipX = shootPosition.x < 0;
                    PlayerController.instance.SelectNonTarget(shootPosition);
                }
            }
            else
            {
                PlayerController.instance.SelectTarget();
            }
        }
        else
        {
            if (autoTarget.Count == 0)
            {
                PlayerController.instance.SelectNonTarget(!PlayerController.instance.FlipX ? Vector2.right : Vector2.left);
            }
            else
            {
                PlayerController.instance.SelectTarget();
            }
        }
    }
    void OnUpdatePlayer(float deltaTime)
    {
        if (PlayerController.instance == null)
            return;
        PlayerController.instance.OnUpdate(deltaTime);
    }
    void OnUpdateEnemyManager(float deltaTime)
    {
        if (EnemyManager.instance == null)
            return;
        EnemyManager.instance.OnUpdate(deltaTime);
    }
    void OnUpdateCamera(float deltaTime)
    {
        if (CameraController.instance == null)
            return;
        CameraController.instance.OnUpdate(deltaTime);
    }
    //void OnUpdateCam(float deltaTime)
    //{
    //    if (CameraController.instance == null)
    //        return;
    //    CameraController.instance.OnUpdate(deltaTime);
    //}
    IEnumerator delayDisplayFinish()
    {
        yield return new WaitForSeconds(1.5f);
        uiPanel.DisplayFinish();
    }
    private void Update()
    {
        if (gameState == GameState.begin || gameState == GameState.gameover)
        {
            if (gameState == GameState.gameover)
            {
                StartCoroutine(delayDisplayFinish());
            }
            return;
        }


        var deltaTime = Time.deltaTime;
        JoystickMovement(joystickMove);
        JoystickShooting(joystickShot);
        OnUpdatePlayer(deltaTime);
        OnUpdateEnemyManager(deltaTime);
        OnUpdateCamera(deltaTime);



    }
    public void TryShot()
    {
        PlayerController.instance.ShootDown();
    }
    public void TryJump()
    {
        PlayerController.instance.TryJump();
    }
    public void BtnGrenade()
    {
        PlayerController.instance.TryGrenade();
    }
}
