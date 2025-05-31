using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianMover : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 target;
    private bool moving = false;
    private Stage1TutorialManager tutorialManager;
    private bool isDone = false;

    public void Init(Vector3 targetPos, Stage1TutorialManager manager)
    {
        target = targetPos;
        moving = true;
        tutorialManager = manager;
    }

    void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target) < 0.1f && !isDone)
            {
                isDone = true;
                moving = false;
                // Safe crossing, praise player
                if (tutorialManager != null)
                {
                    // Show dialog until despawn
                    StartCoroutine(HideWadeAndDespawn(1f));
                }
                else
                {
                    Destroy(gameObject, 1f);
                }
            }
        }
    }

    // Detect player collision
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !isDone)
        {
            isDone = true;
            moving = false;
            if (tutorialManager != null)
            {
                // Show dialog until despawn
                tutorialManager.ShowWade("You hit a pedestrian! Slow down and always yield to people crossing. -100 points");
                StartCoroutine(HideWadeAndDespawn(1f));
            }
            else
            {
                Destroy(gameObject, 1f);
            }
        }
    }

    // NEW: Hide Wade and then despawn NPC
    private IEnumerator HideWadeAndDespawn(float waitTime)
    {
        if (isDone && tutorialManager != null)
        {
            // Use correct dialog depending on why isDone
            if (moving == false && Vector3.Distance(transform.position, target) < 0.1f)
            {
                tutorialManager.ShowWade("Well done! You let the pedestrian cross safely.");
            }
            yield return new WaitForSeconds(waitTime);
            tutorialManager.HideWade();
        }
        Destroy(gameObject);
    }
}
