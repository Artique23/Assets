using System.Collections;
using UnityEngine;

public class PedestrianMover_CM : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 target;
    private bool moving = false;

    private bool isDone = false;

    public ChallengeModeManager challengeManager; // Assign in Inspector

    public void Init(Vector3 targetPos)
    {
        target = targetPos;
        moving = true;
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

                // ✅ Reward for safe crossing
                StageScoreManager.Instance.AddPoints(200);
                Destroy(gameObject, 1f);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !isDone)
        {
            isDone = true;
            moving = false;

            // ❌ Life loss instead of point penalty
            if (challengeManager != null)
                challengeManager.ApplyPunishment();

            Destroy(gameObject, 1f);
        }
    }
}
