﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIPanel : MonoBehaviour
{
    public AllBossAndMiniBossInfo allbossandminibossInfo;
    public Sprite nvSprite;
    public GameObject[] bouders;
    public Image[] rewardImg, bouderLevel;
    public Text[] rewardText;
    public Sprite[] rewardSp, levelSp;

    public Button btnReviveByGem;
    public List<Text> missionTexts;
    public GameObject winPanel, defeatPanel, leftwarning, rightwarning, btnReviveByAds, lowHealth;
    public Image grenadeFillAmout, fillbouderGrenade;
    public Text levelText, bulletText, timeText,pricegemText;
    public TextMeshProUGUI myGemText;

    public Animator animGamOver;
    public HealthBarBoss healthBarBoss;
    public GameObject comboDisplay;
    public TextMeshProUGUI comboText, comboNumberText;

    public Slider slideMiniMap;
    public GameObject warning;
    public Image haveBossInMiniMap;

    public int pricesGemRevive;

    public bool CheckWarning()
    {
        return true ? warning.activeSelf : !warning.activeSelf;
    }

    public void CalculateMiniMap()
    {
        if (slideMiniMap.value == 1)
            return;
        slideMiniMap.value = (PlayerController.instance.transform.position.x - GameController.instance.currentMap.pointBeginPlayer.transform.position.x) / GameController.instance.currentMap.distanceMap;
    }
    public void Begin()
    {
        slideMiniMap.value = 0;
        if (GameController.instance.currentMap.haveBoss || GameController.instance.currentMap.haveMiniBoss)
        {
            if (GameController.instance.currentMap.haveMiniBoss)
                haveBossInMiniMap.sprite = allbossandminibossInfo.infos[DataParam.indexStage].icons[0];
            else if (GameController.instance.currentMap.haveBoss)
                haveBossInMiniMap.sprite = allbossandminibossInfo.infos[DataParam.indexStage].icons[1];
            haveBossInMiniMap.gameObject.SetActive(true);
        }
    }
    public void BtnBackToWorld()
    {
        SoundController.instance.PlaySound(soundGame.soundbtnclick);
        DataParam.nextSceneAfterLoad = 1;
        MyAnalytics.LogEventLoseLevel(DataParam.indexMap, CameraController.instance.currentCheckPoint, DataParam.indexStage);
        PlayerController.instance.EndEvent();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }
    int randomAds;
    public void BtnBack()
    {
        PopupSetting.Instance.ShowPanelSetting();
        SoundController.instance.PlaySound(soundGame.soundbtnclick);
    }
    public void FillGrenade(float _current, float _max)
    {
        fillbouderGrenade.fillAmount = grenadeFillAmout.fillAmount = _current / _max;
    }
    public void BtnNext()
    {
        GameController.instance.StopAll();
        if (DataParam.indexMap < GameController.instance.listMaps[DataParam.indexStage].listMap.Count - 1)
        {
            DataParam.indexMap++;
            MissionController.Instance.AddMission();
            DataParam.nextSceneAfterLoad = 2;
        }
        else
        {
            DataParam.nextSceneAfterLoad = 1;
        }
        SoundController.instance.PlaySound(soundGame.soundbtnclick);
        PlayerController.instance.EndEvent();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }
    public void DisplayDefeat()
    {
        if (defeatPanel.activeSelf)
            return;
        if (GameController.instance.reviveCount == 0)
        {
            myGemText.text = "Own: <color=green>" + DataUtils.playerInfo.gems + "</color>";
            pricegemText.text = "" + pricesGemRevive;
            btnReviveByAds.SetActive(true);

            if (DataUtils.playerInfo.gems >= pricesGemRevive)
            {
                btnReviveByGem.image.color = Color.white;
            }
            else
            {
                btnReviveByGem.image.color = Color.gray;
            }

            btnReviveByGem.gameObject.SetActive(true);
        }
        else
        {
            btnReviveByAds.SetActive(false);
            btnReviveByGem.gameObject.SetActive(false);
            DataUtils.AddCoinAndGame((int)DataParam.totalCoin, 0);
        }
        defeatPanel.SetActive(true);
        SoundController.instance.PlaySound(soundGame.soundlose);
    }
    public void DisplayFinish(int _countstar)
    {
        if (winPanel.activeSelf)
            return;
        missionTexts[0].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission1name;

        if (MissionController.Instance.listMissions[0].isDone && MissionController.Instance.listMissions[1].isDone)
        {
            missionTexts[1].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission2name;
            missionTexts[2].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission3name;
            //  Debug.LogError("TH1");
        }
        else if (MissionController.Instance.listMissions[0].isDone && !MissionController.Instance.listMissions[1].isDone)
        {
            missionTexts[1].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission2name;
            missionTexts[2].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission3name;
            //  Debug.LogError("TH2");
        }
        else if (!MissionController.Instance.listMissions[0].isDone && MissionController.Instance.listMissions[1].isDone)
        {
            missionTexts[1].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission3name;
            missionTexts[2].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission2name;
            //  Debug.LogError("TH3");
        }
        else if (!MissionController.Instance.listMissions[0].isDone && !MissionController.Instance.listMissions[1].isDone)
        {
            missionTexts[1].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission2name;
            missionTexts[2].text = DataController.instance.allMission[DataParam.indexStage].missionData[DataParam.indexMap].mission3name;
            //   Debug.LogError("TH4");
        }

        winPanel.SetActive(true);
        switch (_countstar)
        {
            case 1:
                animGamOver.Play("Win1Star");
                break;
            case 2:

                animGamOver.Play("Win2Star");
                break;
            case 3:

                animGamOver.Play("Win3Star");
                break;
        }
#if UNITY_EDITOR

#else
        randomAds = Random.Range(0, 100);
        if (randomAds < 40)
        {
            AdsManager.Instance.ShowInterstitial((b) => { });
        }
#endif
    }
    public void BtnReviveByGem()
    {
        SoundController.instance.PlaySound(soundGame.soundbtnclick);
        if (DataUtils.playerInfo.gems >= pricesGemRevive)
        {
            Reward(80);
            DataUtils.AddCoinAndGame(0, -pricesGemRevive);
        }
    }
    public void BtnRevive()
    {
        SoundController.instance.PlaySound(soundGame.soundbtnclick);
#if UNITY_EDITOR

        Reward(50);
#else
        AdsManager.Instance.ShowRewardedVideo((b) => {if(b) Reward(50);});
#endif
    }
    void Reward(int healthBonus)
    {
        PlayerController.instance.Revive(healthBonus);
        defeatPanel.SetActive(false);
        GameController.instance.gameState = GameController.GameState.play;
    }
}
