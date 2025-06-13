using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ModularUIButtonClickSFX : MonoBehaviour
{

    private Button button;
    private ModularClick musicManager;

    void Awake()
    {
        button = GetComponent<Button>();
        musicManager = FindObjectOfType<ModularClick>();

        if (musicManager != null)
        {
            button.onClick.AddListener(() => musicManager.PlayButtonClick());
        }
        else
        {
            Debug.LogWarning("MainMenuMusic not found in scene. Button SFX won't play.");
        }
    }

}
