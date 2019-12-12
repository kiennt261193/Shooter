﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPanel : MonoBehaviour
{
    public GameObject finishPanel;
    public GameObject ResetBtn, NextBtn;
    public Image grenadeFillAmout;
    public Text levelText,gameoverText;
    public GameObject starbouder;
    public List<GameObject> starCount;

    public void BtnReset()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void DisplayStar(int total)
    {
        for(int i = 0; i < total; i ++)
        {
            starCount[i].SetActive(true);
        }

        starbouder.SetActive(true);
    }
    public void BtnBack()
    {
        Application.LoadLevel(0);
    }
    public void FillGrenade(float _current,float _max)
    {
        grenadeFillAmout.fillAmount = _current / _max;
    }
    public void BtnNext()
    {
        if (DataParam.indexMap < GameController.instance.listMap.Count - 1)
            DataParam.indexMap++;
        Application.LoadLevel(Application.loadedLevel);
    }
    public void DisplayFinish()
    {
        if (finishPanel.activeSelf)
            return;
        if (GameController.instance.win)
        {
            ResetBtn.SetActive(true);
            NextBtn.SetActive(true);
            gameoverText.text = "WIN";
            SoundController.instance.PlaySound(soundGame.soundwin);

        }
        else
        {
            ResetBtn.SetActive(true);
            NextBtn.SetActive(false);
            gameoverText.text = "DIE";
            SoundController.instance.PlaySound(soundGame.soundlose);
        }
        finishPanel.SetActive(true);
    }
}
