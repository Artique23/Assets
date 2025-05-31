using UnityEngine;

public class ParkingZone : MonoBehaviour
{
    public Stage1TutorialManager tutorialManager;
    public GameObject highlightVisual; // assign your visual highlight object
    public float minParkTime = 2.0f;   // How long car must stay still in zone to "finish"
    public float maxParkSpeed = 0.5f;  // Max allowed speed for being "parked"
    private float parkedTimer = 0f;
    private bool playerInZone = false;
    private Rigidbody playerRb;

    private bool highlightActive = false;

    public GameObject finalGameMarker; // Assign in Inspector: the marker GameObject
    private bool markerGone = false;


    void Start()
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(false);
    }

    public void ActivateParkingHighlight()
    {
        if (highlightVisual != null)
        {
            highlightVisual.SetActive(true);
            highlightActive = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerRb = other.attachedRigidbody;
            parkedTimer = 0f;

            // Show highlight if not already active
            if (highlightVisual != null && !highlightActive)
            {
                highlightVisual.SetActive(true);
                highlightActive = true;
            }

            if (tutorialManager != null)
                tutorialManager.ShowWade("Park your car inside the highlighted space and stop.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            parkedTimer = 0f;
            playerRb = null;
            if (tutorialManager != null)
                tutorialManager.ShowWade("Try again! Park fully in the space and come to a stop.");
        }
    }

    void Update()
    {
        // Check if the marker is gone (destroyed or inactive)
        if (!markerGone && finalGameMarker == null)
        {
            markerGone = true;
            if (highlightVisual != null)
                highlightVisual.SetActive(true); // Show highlight as soon as marker is gone
            if (tutorialManager != null)
                tutorialManager.ShowWade("Proceed to the highlighted parking spot!");
        }

        if (playerInZone && playerRb != null)
        {
            if (playerRb.velocity.magnitude < maxParkSpeed)
            {
                parkedTimer += Time.deltaTime;
                if (parkedTimer >= minParkTime)
                {
                    // Success!
                    if (tutorialManager != null)
                    StageScoreManager.Instance.AddPoints(100);
                    tutorialManager.ShowWade("Congratulations! You parked successfully!\nTotal Points: " + StageScoreManager.Instance.GetPoints());// Add points for successful parking
                    if (highlightVisual != null)
                        highlightVisual.SetActive(false);
                    enabled = false;
                }
            }
            else
            {
                parkedTimer = 0f;
            }
        }
    }

}
