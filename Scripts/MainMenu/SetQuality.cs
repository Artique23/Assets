using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuality : MonoBehaviour
{
    void Start()
    {
        // Load saved quality setting if it exists
        if (PlayerPrefs.HasKey("QualityLevel"))
        {
            int savedQuality = PlayerPrefs.GetInt("QualityLevel");
            QualitySettings.SetQualityLevel(savedQuality, true);
        }
    }

    public void SetQ(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        SaveQualitySettings(qualityIndex);
    }

    public void LowQ()
    {
        QualitySettings.SetQualityLevel(0, true);
        SaveQualitySettings(0);
        Debug.Log("Quality Set to Low");
    }

    public void MediumQ()
    {
        QualitySettings.SetQualityLevel(2, true); // Changed from 3 to 2 for Android
        SaveQualitySettings(2);
        Debug.Log("Quality Set to Medium");
    }

    public void HighQ()
    {
        QualitySettings.SetQualityLevel(4, true); // Changed from 5 to 4 for Android
        SaveQualitySettings(4);
        Debug.Log("Quality Set to High");
    }

    private void SaveQualitySettings(int qualityLevel)
    {
        PlayerPrefs.SetInt("QualityLevel", qualityLevel);
        PlayerPrefs.Save();
    }
}
