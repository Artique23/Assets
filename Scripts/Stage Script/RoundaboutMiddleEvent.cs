using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundaboutMiddleEvent : MonoBehaviour
{
     public StageBaseManager stageBaseManager; // Assign in Inspector
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            stageBaseManager.ShowWade("Try to go around the roundabout while avoiding cars and use proper signals when switching lanes!");

            // Hide Wade after 4 seconds
            StartCoroutine(HideWadeAfterDelay());
        }
    }

    IEnumerator HideWadeAfterDelay()
    {
        yield return new WaitForSeconds(4f);
        stageBaseManager.HideWade();
    }
}
