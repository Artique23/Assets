using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenusUIScript : MonoBehaviour
{
    [SerializeField] GameObject GeneralUI, MainMenuUI, StartMenuUI, StoryModeUI,SettingsUI;
    // Start is called before the first frame update
    void Start()
    {
        GeneralUI.SetActive(true);
        MainMenuUI.SetActive(true);
        StartMenuUI.SetActive(false);
        StoryModeUI.SetActive(false);
        SettingsUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
