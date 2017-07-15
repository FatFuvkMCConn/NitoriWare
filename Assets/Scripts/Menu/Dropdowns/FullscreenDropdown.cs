﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FullscreenDropdown : MonoBehaviour
{
    private const char ResolutionDelimiter = 'x';

#pragma warning disable 0649   //Serialized Fields
    [SerializeField]
    private Dropdown dropdown;
    [SerializeField]
    private string yesKey, noKey;
#pragma warning restore 0649

    void Start()
    {
        dropdown.options[0].text = TextHelper.getLocalizedText(yesKey, "Yes");
        dropdown.options[1].text = TextHelper.getLocalizedText(noKey, "No");
        dropdown.value = Screen.fullScreen ? 0 : 1;
    }

    void Update()
    {
        dropdown.value = Screen.fullScreen ? 0 : 1;
    }

    public void select(int item)
    {
        bool selectedFullscreen = item == 0;
        if (Screen.fullScreen != selectedFullscreen)
            Screen.fullScreen = selectedFullscreen;
    }
}