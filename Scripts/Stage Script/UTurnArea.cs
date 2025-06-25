using UnityEngine;

public class UTurnArea : MonoBehaviour
{
    [TextArea(2, 3)]
    public string[] messages = {
        "Hey! There's been a crash ahead.",
        "The road is blocked. You'll have to turn.",
        "Make the turn without hitting anything!"
    };

    public float messageDelay = 2f;
    public int rewardPoints = 250;

    private bool triggered = false;
    private bool exited = false;

    private StageBaseManager stageManager;
    private int activeDialogId = -1;

    void Start()
    {
        stageManager = FindObjectOfType<StageBaseManager>();
        if (stageManager == null)
        {
            Debug.LogWarning("StageBaseManager not found in the scene.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(PlayWadeMessages());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!exited && other.CompareTag("Player"))
        {
            exited = true;

            if (stageManager != null)
                stageManager.HideWade(activeDialogId); // Clean up dialog box

            StageScoreManager.Instance.AddPoints(rewardPoints);
            Debug.Log($"Player rewarded {rewardPoints} points for completing the U-turn area.");
        }
    }

    private System.Collections.IEnumerator PlayWadeMessages()
    {
        foreach (string msg in messages)
        {
            if (stageManager != null)
                activeDialogId = stageManager.ShowWade(msg);

            yield return new WaitForSeconds(messageDelay);
        }
    }
}
