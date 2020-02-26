﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public enum PACK_NAME { CHEAP_PACK, BEST_CHOICE, PROFESSIONAL_PACK }
    public PACK_NAME packName;
    public Text txtPrice;
    public DataUtils.ITEM_SHOP_TYPE shopType;
    public string packID;
    private Button btn;

    private void OnEnable()
    {
        btn = GetComponent<Button>();
        ProcessPackID();
    }

    private void ProcessPackID()
    {
        switch (packName)
        {
            case PACK_NAME.CHEAP_PACK:
                packID = DataUtils.P_CHEAP_PACK;
                break;
            case PACK_NAME.PROFESSIONAL_PACK:
                packID = DataUtils.P_PROFESSIONAL_PACK;
                break;
            case PACK_NAME.BEST_CHOICE:
                packID = DataUtils.P_BEST_CHOICE;
                break;
        }
        txtPrice.text = "BUY";//GameIAPManager.GetPriceByID(packID);
    }
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(() => {
            switch (shopType)
            {
                case DataUtils.ITEM_SHOP_TYPE.GEM:
                    ProcessBuyGem();
                    break;
                case DataUtils.ITEM_SHOP_TYPE.PACKAGE:
                    ProcessBuyPackage();
                    break;
                case DataUtils.ITEM_SHOP_TYPE.RESOURCES:
                    ProcessBuyResources();
                    break;
            }
        });
    }

    private void ProcessBuyGem()
    {

    }
    private void ProcessBuyPackage()
    {
        switch (packName)
        {
            case PACK_NAME.CHEAP_PACK:
                ShopManager.Instance.ShowBuyPanel("Beginner Pack", GameIAPManager.GetPriceByID(packID), packID, 0, 25, 7500);
                break;
            case PACK_NAME.PROFESSIONAL_PACK:
                ShopManager.Instance.ShowBuyPanel("Professional Pack", GameIAPManager.GetPriceByID(packID), packID, 50, 100, 85000);
                break;
            case PACK_NAME.BEST_CHOICE:
                ShopManager.Instance.ShowBuyPanel("Best Choice", GameIAPManager.GetPriceByID(packID), packID, 20, 50, 15000);
                break;
        }
    }
    public void ProcessBuyResources()
    {

    }
}