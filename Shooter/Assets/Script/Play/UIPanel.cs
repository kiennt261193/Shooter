﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{

    public void BtnReset()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
