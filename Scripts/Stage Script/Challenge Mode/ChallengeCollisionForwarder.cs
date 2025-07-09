using UnityEngine;

public class ChallengeCollisionForwarder : MonoBehaviour
{
    private ChallengeModeManager challengeManager;

    void Start()
    {
        challengeManager = FindObjectOfType<ChallengeModeManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (challengeManager != null)
        {
            challengeManager.OnChallengeCollision(collision.gameObject.tag);
        }
    }
}
