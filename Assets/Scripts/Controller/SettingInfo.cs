using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingInfo
{
    public int[] screenResolution;
    public bool fullScreen;
    public float volumeBgm;
    public float volumeSfx;
    public bool showDamage;

    public SettingInfo(int[] screenResolution, bool fullScreen, float volumeBgm, float volumeSfx, bool showDamage)
    {
        if (screenResolution.Length != 2)
        {
            Debug.Log($"Out of form: Screen Resolution(size: {screenResolution.Length})");
            return;
        }
        this.screenResolution = screenResolution;
        this.fullScreen = fullScreen;
        this.volumeBgm = volumeBgm;
        this.volumeSfx = volumeSfx;
        this.showDamage = showDamage;
    }
}
