using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBaseManager : MonoBehaviour
{
 public GameObject wadePopupPanel;
    public TMPro.TMP_Text wadeText;

    private int wadeDialogCounter = 0;

    public virtual int ShowWade(string text)
    {
        wadePopupPanel.SetActive(true);
        wadeText.text = text;
        wadeDialogCounter++;
        return wadeDialogCounter; // return current id
    }
    public virtual void HideWade(int dialogId)
    {
        if (dialogId == wadeDialogCounter)
            wadePopupPanel.SetActive(false);
    }
    public virtual void HideWade() // fallback
    {
        wadePopupPanel.SetActive(false);
    }
}
