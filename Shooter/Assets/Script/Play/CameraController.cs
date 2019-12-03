﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class CameraController : MonoBehaviour
{
    public GameObject nextPointCheck;
    public static CameraController instance;
    public List<Transform> posEnemyV2, posMiniBoss1;
    public List<GameObject> bouders;
    public float speed;

    public ProCamera2DNumericBoundaries NumericBoundaries;

    public int currentCamBoidaries;

    private void OnValidate()
    {
        NumericBoundaries = GetComponent<ProCamera2DNumericBoundaries>();
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;

        bouders[0].transform.localPosition = new Vector2(bouders[0].transform.localPosition.x, Camera.main.orthographicSize + 0.5f);
        bouders[1].transform.localPosition = new Vector2(bouders[1].transform.localPosition.x, -Camera.main.orthographicSize - 0.5f);
        bouders[2].transform.localPosition = new Vector2(Camera.main.orthographicSize + 3.61f, bouders[2].transform.localPosition.y);
        bouders[3].transform.localPosition = new Vector2(-Camera.main.orthographicSize - 3.61f, bouders[3].transform.localPosition.y);
        currentCamBoidaries = 0;
    }
    public void Init()
    {
        NumericBoundaries.RightBoundary = GameController.instance.currentMap.procam2DTriggerBoudaries[currentCamBoidaries].RightBoundary + GameController.instance.currentMap.procam2DTriggerBoudaries[currentCamBoidaries].transform.position.x;
    }
    Vector2 _cameraSize;
    float velocity;
    public void Start()
    {
        //ProCamera2D.Instance.OffsetX = 0.3f;
        //ProCamera2D.Instance.OffsetY = 0f;
        _cameraSize.y = Camera.main.orthographicSize;
        _cameraSize.x = Mathf.Max(1, ((float)Screen.width / (float)Screen.height)) * _cameraSize.y;

    }
    public bool setBoudariesLeft = true;
    public void NextPoint()
    {

        if (currentCamBoidaries < GameController.instance.currentMap.procam2DTriggerBoudaries.Length - 1)
        {
            currentCamBoidaries++;
            nextPointCheck.gameObject.SetActive(true);
            currentRightBoudary = Camera.main.transform.position.x;
        }
        else
        {
         //   PlayerController.instance.DoneMission(true);

        }
        // nextPointCheck.gameObject.SetActive(false);
    }
    float currentRightBoudary;
    public void OnUpdate(float deltaTime)
    {
        CacheSizeAndViewPos();
        NumericBoundaries.RightBoundary = Mathf.SmoothStep(NumericBoundaries.RightBoundary, GameController.instance.currentMap.procam2DTriggerBoudaries[currentCamBoidaries].RightBoundary + GameController.instance.currentMap.procam2DTriggerBoudaries[currentCamBoidaries].transform.position.x, speed);
        if (!setBoudariesLeft)
        {
            if (nextPointCheck.activeSelf)
            {
                if (Camera.main.transform.position.x - currentRightBoudary >= 1f)
                {
                    nextPointCheck.SetActive(false);
                    setBoudariesLeft = true;
                 //   Debug.LogError("--------------active again");
                }
            }
            else
            {
                if (GameController.instance.autoTarget.Count == 0)
                {
                    NextPoint();
                }
            }
            return;
        }
        var leftBoundary = transform.position.x - Size().x;
        NumericBoundaries.LeftBoundary = leftBoundary;

    }

    public Vector2 Size()
    {
        return _cameraSize;
    }
    private void CacheSizeAndViewPos()
    {
        _cameraSize.y = Camera.main.orthographicSize;
        _cameraSize.x = Mathf.Max(1, ((float)Screen.width / (float)Screen.height)) * _cameraSize.y;
        viewPos.minX = transform.position.x - _cameraSize.x;
        viewPos.minY = transform.position.y - _cameraSize.y;
        viewPos.maxX = transform.position.x + _cameraSize.x;
        viewPos.maxY = transform.position.y + _cameraSize.y;
    }
    public ViewPos viewPos;
}
[System.Serializable]
public struct ViewPos
{
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
}
