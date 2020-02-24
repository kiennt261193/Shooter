﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelHeroes : MonoBehaviour
{
    public Sprite sprStar, sprStarUnlock;
    public Image[] imgAllStars;
    public Image imgArrow;
    public TextMeshProUGUI txtLevel;
    public Text txtCurPice;
    public Text txtCurHealth, txtCurDamage;
    public Text txtPriceUpdate;
    public Text txtPiceSelected;
    public TextMeshProUGUI txtHealth, txtDamage, txtAttSpeed, txtCritDamage, txtCritRate, txtDefRate;
    public TextMeshProUGUI txtHealthUP, txtDamageUP, txtAttSpeedUP, txtCritDamageUP, txtCritRateUP;
    public ParticleSystem pEvolve;
    public Text txtPice;
    public Image imgProgress;
    public TextMeshProUGUI txtTotalPower, txtHealthDis, txtDamageDis;
    public Button[] btnTabs;
    public Sprite sprSelect, sprDeSelect;

    private PlayerData pData;
    private double priceUpdate;

    private string keyEquipped;
    ItemData itemEquipped = null;
    private int curWeponStar = 0, nextWeaponStar = 0;

    private void Awake()
    {
        pEvolve.Stop();
    }

    private void OnEnable()
    {
        FillHeroData();
    }
    private void FillHeroData()
    {
        for (int i = 0; i < DataUtils.playerInfo.level; i++)
        {
            imgAllStars[i].sprite = sprStar;
        }
        FillDataPlayer();
    }
    private void FillDataPlayer()
    {
        foreach (ItemData _id in DataUtils.dicEquippedItem.Values)
        {
            if (_id.type.Equals("WEAPON"))
            {
                itemEquipped = _id;
                break;
            }
        }
        keyEquipped = itemEquipped.id + "_" + itemEquipped.level;
        curWeponStar = itemEquipped.curStar;
        nextWeaponStar = itemEquipped.curStar + 1 > 5 ? itemEquipped.curStar : itemEquipped.curStar + 1;

        Debug.LogError(DataController.instance.playerData[0].playerData[DataUtils.lstAllPlayerHeroes[DataUtils.HeroIndex()].level-1].hp);

        for (int i = 0; i < DataController.instance.playerData.Count; i++)
        {
            Debug.LogError("------> " + i);
            pData = DataController.instance.playerData[0].playerData[i];
            if (pData.ID == DataUtils.playerInfo.id)
            {
                //Debug.LogError(pData.ID + " vs " + pData.level);
                int outLv = 1;
                if (int.TryParse(pData.level, out outLv))
                {
                    if (outLv == DataUtils.playerInfo.level)
                    {
                        txtCurHealth.text = pData.hp.ToString();
                        if (i + 1 < DataController.instance.playerData.Count)
                        {
                            txtHealth.text = pData.hp.ToString();
                            txtHealthUP.text = DataUtils.DisplayRichText(pData.hp, DataController.instance.playerData[0].playerData[(i + 1 < DataUtils.MAX_LEVEL_HERO ? i + 1 : i)].hp);
                        }
                    }
                }
            }
        }

        txtDamage.text = GetDoublevalue(DataUtils.dicWeapon[keyEquipped].DmgValue[curWeponStar]).ToString();
        txtDamageUP.text = DataUtils.DisplayRichText(GetDoublevalue(DataUtils.dicWeapon[keyEquipped].DmgValue[curWeponStar]),GetDoublevalue(DataUtils.dicWeapon[keyEquipped].DmgValue[nextWeaponStar]));

        txtAttSpeed.text = GetDoublevalue(DataUtils.dicWeapon[keyEquipped].BulletSpeedValue[curWeponStar]).ToString();
        txtAttSpeedUP.text = DataUtils.DisplayRichText(GetDoublevalue(DataUtils.dicWeapon[keyEquipped].BulletSpeedValue[curWeponStar]), GetDoublevalue(DataUtils.dicWeapon[keyEquipped].BulletSpeedValue[nextWeaponStar]));

        txtCritDamage.text = GetDoublevalue(DataUtils.dicWeapon[keyEquipped].CritDmgValue[curWeponStar]).ToString();
        txtCritDamageUP.text = DataUtils.DisplayRichText(GetDoublevalue(DataUtils.dicWeapon[keyEquipped].CritDmgValue[curWeponStar]), GetDoublevalue(DataUtils.dicWeapon[keyEquipped].CritDmgValue[nextWeaponStar]));

        txtCritRate.text = GetDoublevalue(DataUtils.dicWeapon[keyEquipped].CritRateValue[curWeponStar]).ToString();
        txtCritRateUP.text = DataUtils.DisplayRichText(GetDoublevalue(DataUtils.dicWeapon[keyEquipped].CritRateValue[curWeponStar]), GetDoublevalue(DataUtils.dicWeapon[keyEquipped].CritRateValue[nextWeaponStar]));

        txtCurDamage.text = GetDoublevalue(DataUtils.dicWeapon[keyEquipped].DmgValue[curWeponStar]).ToString();

        priceUpdate = 165 * pData.SoManhYeuCau * pData.Giamua1manh;
        Debug.LogError("priceUpdate: " + priceUpdate);
        txtPriceUpdate.text = priceUpdate.ToString();


        txtDamageDis.text = GetDoublevalue(DataUtils.dicWeapon[keyEquipped].DmgValue[curWeponStar]).ToString();
        txtHealthDis.text = pData.hp.ToString();
        txtTotalPower.text = pData.MoveSpeed.ToString();
        txtPice.text = DataUtils.lstAllPlayerHeroes[DataUtils.HeroIndex()].pices + "/" + pData.SoManhYeuCau;
        imgProgress.fillAmount = DataUtils.lstAllPlayerHeroes[DataUtils.HeroIndex()].pices*1.0f / (float)pData.SoManhYeuCau;
    }

    private double GetDoublevalue(double db)
    {
        return System.Math.Floor(db * 100) / 100;
    }
    public void EvolveHero()
    {
        if(DataUtils.playerInfo.coins >= priceUpdate)
        {
            if (DataUtils.playerInfo.level < DataUtils.MAX_LEVEL_HERO)
            {
                DataUtils.playerInfo.level += 1;
                DataUtils.AddCoinAndGame((int)-priceUpdate, 0);
                pEvolve.Play();
                FillHeroData();
                DataUtils.SavePlayerData();
            }
            else {
                MainMenuController.Instance.ShowMapNotify("Hero has reached the maximum level");
            }
        }
        else
        {
            MainMenuController.Instance.ShowMapNotify("Not enough coins to level up.");
        }
    }

    public void ChooseTab(int index_)
    {
        for (int i = 0; i < btnTabs.Length; i++)
        {
            if (i == index_)
            {
                btnTabs[i].image.sprite = sprSelect;
            }
            else btnTabs[i].image.sprite = sprDeSelect;
        }
    }
}