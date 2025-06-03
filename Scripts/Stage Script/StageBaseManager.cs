using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBaseManager : MonoBehaviour
{
 public GameObject wadePopupPanel;
    public TMPro.TMP_Text wadeText;

    public virtual void ShowWade(string text)
    {
        wadePopupPanel.SetActive(true);
        wadeText.text = text;
    }
    public virtual void HideWade()
    {
        wadePopupPanel.SetActive(false);
    }
}
