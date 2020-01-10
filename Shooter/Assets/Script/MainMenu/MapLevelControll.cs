﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelControll : MonoBehaviour
{
    public MapLevelControll mapLevelControll;
    public int stageIndex;
    public int mapIndex;
    public Image[] imgStars;
    private Button btn;
    //[HideInInspector]
    public bool canPlay;

    public Image imgMap;
    private void Awake()
    {
        btn = GetComponent<Button>();
        imgMap = GetComponent<Image>();
        if (!DataUtils.StageHasInit() && mapIndex == 0)
        {
            imgMap.color = StageManager.Instance.clUnlock;
            canPlay = true;
        }
    }
    private void OnEnable()
    {
        SwitchColor();
        btn.onClick.AddListener(() => {
            StageManager.Instance.levelControll = mapLevelControll;

            MainMenuController.Instance.SoundClickButton();
            if (MapHasUnlock() || imgMap.color == StageManager.Instance.clUnlock || imgMap.color == StageManager.Instance.clSelected)
            {
                StageManager.Instance.SwitchColor();
                imgMap.color = StageManager.Instance.clSelected;
            }
            //else
            //{
            //    MainMenuController.Instance.ShowMapNotify("Map " + (mapIndex + 1) + " not yet unlock.");
            //}
            var miss_ = DataController.instance.allMission[stageIndex].missionData[mapIndex];
            GetMapInfo(miss_, stageIndex, mapIndex);
        });
        CheckMapStars();
    }
    public void SwitchColor()
    {
        if (!DataUtils.StageHasInit() && mapIndex == 0)
        {
            imgMap.color = StageManager.Instance.clUnlock;
        }
        else if (MapHasUnlock())
        {
            imgMap.color = StageManager.Instance.clUnlock;
        }
        else
        {
            imgMap.color = StageManager.Instance.clNotYetUnlock;
        }
    }
    public void CheckMapStars()
    {
        if (DataUtils.StageHasInit())
        {
            for (int i = 0; i < DataUtils.GetMapByIndex(stageIndex, mapIndex).mission.Count; i++)
            {
                LVMission vMission = DataUtils.GetMapByIndex(stageIndex, mapIndex).mission[i];
                if (vMission.isPass)
                {
                    imgStars[i].sprite = StageManager.Instance.imgStar;
                }
                else
                {
                    imgStars[i].sprite = StageManager.Instance.imgStarNotYetUnlock;
                    imgStars[i].gameObject.transform.SetAsLastSibling();
                }
            }
        }
    }
    private void GetMapInfo(Mission miss_, int stageSelect, int mapSelect)
    {
        for (int i = 0; i < StageManager.Instance.imgItemReward.Length; i++) {
            StageManager.Instance.imgItemReward[i].transform.parent.gameObject.SetActive(false);
        }
        StageManager.Instance.FillMapInfo(miss_, stageSelect, mapSelect);
        if (DataUtils.StageHasInit())
        {
            for (int i = 0; i < DataUtils.GetMapByIndex(stageIndex, mapIndex).rewards.Count; i++)
            {
                //Debug.LogError(DataUtils.GetMapByIndex(stageIndex, mapIndex).rewards[i].rType);
                if(MainMenuController.Instance.GetSpriteByName(DataUtils.GetMapByIndex(stageIndex, mapIndex).rewards[i].rType) != null)
                {
                    StageManager.Instance.imgItemReward[i].sprite = MainMenuController.Instance.GetSpriteByName(DataUtils.GetMapByIndex(stageIndex, mapIndex).rewards[i].rType);
                    StageManager.Instance.imgItemReward[i].transform.parent.gameObject.SetActive(true);
                }
            }
        }
    }

    void Start()
    {

    }
    private bool MapHasUnlock()
    {
        return canPlay;
    }
}
