using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WinningInventory : MonoBehaviour
{
    [Header("Game Setup")]
    public InventorySlot[] winningSlots;  // Target slots for correct answers
    public TextMeshProUGUI[] slotLabels;  // Labels for each slot
    public List<GameObject> roadSignPrefabs;  // All available road signs
    public Transform roadSignsContainer;  // Where to spawn road signs
    public Button nextButton;  
    public Button resetButton;  

    [Header("Game Settings")]
    public int totalRounds;

    // Private variables
    private List<GameObject> spawnedSigns = new List<GameObject>();
    private List<string> currentRoundSignNames = new List<string>();
    private int currentRound = 0;
    private bool roundComplete = false;
    private bool isQuizPanelActive = false;

    [Header("References")]
    public GameObject quizPanel; // Reference to the quiz panel

    void Start()
    {
        // Set up buttons
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(StartNextRound);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetCurrentRound);
        }

        // Do an initial shuffle when the game starts
        ShuffleRoadSigns();

        // Start first round
        StartNextRound();
    }

    void Update()
    {
        // Check if round is complete
        if (!roundComplete && AreAllSlotsCorrect())
        {
            roundComplete = true;
            OnRoundComplete();
        }
    }

    public int CurrentRound
    {
        get { return currentRound; }
    }

    public void ShuffleRoadSigns()
    {
        // Find all draggable items in the container
        List<DraggableItem> roadSigns = new List<DraggableItem>();
        foreach (Transform child in roadSignsContainer)
        {
            DraggableItem item = child.GetComponentInChildren<DraggableItem>();
            if (item != null)
            {
                roadSigns.Add(item);
            }
        }

        if (roadSigns.Count <= 1)
        {
            Debug.LogWarning("Not enough road signs to shuffle");
            return;
        }

        // Just use instant shuffle
        InstantlyShuffle(roadSigns);
    }

    private void InstantlyShuffle(List<DraggableItem> signs)
    {
        // Get all current slots
        List<Transform> slots = new List<Transform>();
        foreach (DraggableItem sign in signs)
        {
            slots.Add(sign.transform.parent);
        }

        // Store original scales
        List<Vector3> originalScales = new List<Vector3>();
        foreach (DraggableItem sign in signs)
        {
            originalScales.Add(sign.transform.localScale);
        }

        // Shuffle the slots list
        for (int i = 0; i < slots.Count; i++)
        {
            int randomIndex = Random.Range(i, slots.Count);
            Transform temp = slots[i];
            slots[i] = slots[randomIndex];
            slots[randomIndex] = temp;
        }

        // Assign each sign to a new slot
        for (int i = 0; i < signs.Count; i++)
        {
            signs[i].transform.SetParent(slots[i]);
            signs[i].transform.localPosition = Vector3.zero;
            signs[i].transform.localRotation = Quaternion.identity;
            
            // Make sure the scale is correct (use original scale)
            signs[i].transform.localScale = originalScales[i];

            // Update the original parent reference
            signs[i].originalParent = slots[i];
        }
    }

    public void StartNextRound()
    {
        // Reset state
        roundComplete = false;
        currentRoundSignNames.Clear();

        // Return any signs in winning slots to container
        ReturnSignsToContainer();

        // Hide next button
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        // Shuffle the road signs in the container
        ShuffleRoadSigns();

        // Select random signs for this round
        SelectRandomSigns();

        // Set up winning slots with expected sign names
        for (int i = 0; i < winningSlots.Length && i < currentRoundSignNames.Count; i++)
        {
            if (winningSlots[i] != null)
            {
                winningSlots[i].expectedSignName = currentRoundSignNames[i];
            }

            // Update label text
            if (slotLabels != null && i < slotLabels.Length && slotLabels[i] != null)
            {
                slotLabels[i].text = currentRoundSignNames[i];
            }
        }

        // Increment round counter
        currentRound++;
        Debug.Log($"Starting Round {currentRound}");
    }

    private List<string> previousRoundSignNames = new List<string>();

    private void SelectRandomSigns()
    {
        // Create list of available signs
        List<GameObject> availableSigns = new List<GameObject>(roadSignPrefabs);

        // Try to avoid signs from the previous round if possible
        if (availableSigns.Count > winningSlots.Length && previousRoundSignNames.Count > 0)
        {
            availableSigns.RemoveAll(sign =>
                previousRoundSignNames.Contains(sign.name) &&
                availableSigns.Count - 1 >= winningSlots.Length
            );
        }

        // Select random signs
        int signsToSelect = Mathf.Min(winningSlots.Length, availableSigns.Count);

        // Clear previous round signs and prepare to store new ones
        previousRoundSignNames.Clear();

        for (int i = 0; i < signsToSelect; i++)
        {
            if (availableSigns.Count == 0) break;

            int randomIndex = Random.Range(0, availableSigns.Count);
            GameObject selected = availableSigns[randomIndex];

            // Store name
            string signName = selected.name;
            currentRoundSignNames.Add(signName);
            previousRoundSignNames.Add(signName); // Store for next round's reference

            // Remove from available pool
            availableSigns.RemoveAt(randomIndex);

            Debug.Log($"Selected sign for slot {i}: {signName}");
        }
    }

    private bool AreAllSlotsCorrect()
    {
        // First make sure we have signs assigned
        if (currentRoundSignNames.Count == 0)
        {
            return false;
        }

        // Check each winning slot
        for (int i = 0; i < winningSlots.Length && i < currentRoundSignNames.Count; i++)
        {
            InventorySlot slot = winningSlots[i];

            // If slot is null, log an error but continue checking others
            if (slot == null)
            {
                Debug.LogError($"Winning slot {i} is null!");
                return false;
            }

            // If slot is empty, not correct
            if (slot.transform.childCount == 0)
            {
                return false;
            }

            // Get the sign in this slot
            Transform child = slot.transform.GetChild(0);
            if (child == null)
            {
                Debug.LogError($"Child in slot {i} is null!");
                return false;
            }

            string signName = child.name;

            // Remove "(Clone)" suffix if present
            if (signName.EndsWith("(Clone)"))
                signName = signName.Substring(0, signName.Length - 7);

            // Compare with expected name
            if (signName != currentRoundSignNames[i])
            {
                Debug.Log($"Slot {i} has wrong sign: expected {currentRoundSignNames[i]}, got {signName}");
                return false;
            }
        }

        Debug.Log("All slots have correct signs!");
        return true;
    }

    private void OnRoundComplete()
    {
        Debug.Log("Round Complete!");

        // Show next button, regardless of round number
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
        
        // Optional: Display a "congratulations" message if desired
        if (currentRound >= totalRounds)
        {
            Debug.Log($"Reached round {currentRound} of {totalRounds}! Keep going!");
            // You can add additional UI feedback here if desired
        }
    }

    public void ResetCurrentRound()
    {
        // Return signs to container
        ReturnSignsToContainer();

        // Reset round state
        roundComplete = false;

        // Hide next button
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    private void ReturnSignsToContainer()
    {
        // Find all draggable items in winning slots
        foreach (InventorySlot slot in winningSlots)
        {
            if (slot == null) continue;

            // Get all children
            List<Transform> children = new List<Transform>();
            foreach (Transform child in slot.transform)
            {
                children.Add(child);
            }

            // Return each child to its original inventory slot
            foreach (Transform child in children)
            {
                if (child == null) continue;

                DraggableItem draggable = child.GetComponent<DraggableItem>();
                if (draggable != null)
                {
                    // Use the ReturnToOriginalSlot method to return to original slot
                    draggable.ReturnToOriginalSlot();
                }
                else
                {
                    // Fallback if no DraggableItem component (shouldn't happen)
                    child.SetParent(roadSignsContainer);
                    Debug.LogWarning($"No DraggableItem component on {child.name}");
                }
            }
        }
    }

    public void SetQuizPanelActive(bool active)
    {
        isQuizPanelActive = active;
    }
}