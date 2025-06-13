using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuizModeOnbuttonclick : MonoBehaviour
{
    private Button button;
    private QuizModeSFX sfxManager;
    private AnswerScript answerScript;

    void Awake()
    {
        button = GetComponent<Button>();
        sfxManager = FindObjectOfType<QuizModeSFX>();
        answerScript = GetComponent<AnswerScript>();

        if (sfxManager != null)
        {
            button.onClick.AddListener(PlayAnswerSFX);
        }
        else
        {
            Debug.LogWarning("QuizModeSFX not found in scene. Button SFX won't play.");
        }
    }

    private void PlayAnswerSFX()
    {
        if (answerScript != null)
        {
            if (answerScript.isCorrect)
                sfxManager.PlayCorrect();
            else
                sfxManager.PlayWrong();
        }
        else
        {
            // fallback: just play button click
            sfxManager.PlayButtonClick();
        }
    }
}