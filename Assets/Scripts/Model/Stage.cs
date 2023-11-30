using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage : MonoBehaviour
{
    public Image stageImage;
    public TMP_Text name;

    public void Init(string code)
    {
        this.stageImage.sprite = Resources.Load<Sprite>("Sprites/Stages/" + code);
        this.name.text = "Stage" + code;
    }
}
