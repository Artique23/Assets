using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinningInventory : MonoBehaviour
{
    [Header("Winning Slots")]
    public InventorySlot[] winningSlots;  // Array of the 3 slots where road signs should be dropped
    public TextMeshProUGUI[] slotLabels;  // Text labels showing the name of each required road sign

    [Header("Road Sign Prefabs")]
    public List<GameObject> roadSignPrefabs;  // List of all available road sign prefabs

    [Header("Game Controls")]
    public Button nextButton;  // Button to proceed to next round
    public int totalRounds = 3;  // Total number of rounds to play

    [Header("References")]
    public Transform roadSignsContainer;  // Container where road signs are stored/spawned

    [Header("Additional Controls")]
    public Button resetButton; // Button to reset the current round
    private List<GameObject> spawnedSigns = new List<GameObject>(); // Track spawned signs

    private List<GameObject> currentRequiredSigns = new List<GameObject>();  // Signs needed for current round
    private List<string> currentRequiredSignNames = new List<string>();  // Names of required signs
    private int currentRound = 0;
    private bool roundComplete = false;

    [Header("Grid References")]
    public GameObject gridContainer; // The grid containing the inventory slots
    public InventorySlot[] initialSlots; // The initial slots where each road sign belongs



void Start()
{
    // Hide next button initially
    if (nextButton != null)
    {
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }
    
    // Setup reset button
    if (resetButton != null)
    {
        resetButton.onClick.AddListener(ResetCurrentRound);
    }

    // Validate setup
    if (winningSlots.Length < 3)
    {
        Debug.LogError("Not enough winning slots assigned. Need exactly 3.");
        return;
    }

    if (slotLabels.Length < 3)
    {
        Debug.LogError("Not enough slot labels assigned. Need exactly 3.");
        return;
    }
    
    // Log initial slot assignments
    if (initialSlots != null && initialSlots.Length > 0)
    {
        Debug.Log("Initial slot assignments:");
        for (int i = 0; i < initialSlots.Length; i++)
        {
            if (initialSlots[i] != null)
            {
                if (i < roadSignPrefabs.Count && roadSignPrefabs[i] != null)
                    Debug.Log($"Slot {i}: {initialSlots[i].name} for sign {roadSignPrefabs[i].name}");
                else
                    Debug.Log($"Slot {i}: {initialSlots[i].name} (no sign assigned)");
            }
        }
    }

    // Start first round
    StartNextRound();
}

    void Update()
    {
        // Check if all slots have the correct signs
        if (!roundComplete && AreAllSlotsCorrect())
        {
            roundComplete = true;
            OnRoundComplete();
        }
    }

    private void StartNextRound()
{
    // Safety checks
    if (winningSlots == null || winningSlots.Length < 3)
    {
        Debug.LogError("Not enough winning slots assigned!");
        return;
    }
    
    if (slotLabels == null || slotLabels.Length < 3)
    {
        Debug.LogError("Not enough slot labels assigned!");
        return;
    }

    // Reset state
    roundComplete = false;
    currentRequiredSigns.Clear();
    currentRequiredSignNames.Clear();

    // First, collect all signs from winning slots
    List<GameObject> signsToReturn = new List<GameObject>();
    
    foreach (InventorySlot slot in winningSlots)
    {
        if (slot == null) continue;
        
        // Get all children (should be at most one per slot)
        foreach (Transform child in slot.transform)
        {
            signsToReturn.Add(child.gameObject);
        }
    }
    
    // Now clear the slots
    foreach (InventorySlot slot in winningSlots)
    {
        if (slot == null) continue;
        
        // Clear the slot by removing all children
        foreach (Transform child in slot.transform)
        {
            child.SetParent(null); // Temporarily detach
        }
    }
    
    // Return signs to their proper places
    foreach (GameObject sign in signsToReturn)
    {
        if (sign != null)
            ReturnSignToContainer(sign);
    }

    // Hide next button
    if (nextButton != null)
        nextButton.gameObject.SetActive(false);

    // Select 3 random road signs from available prefabs
    SelectRandomRoadSigns();

    // Safety check before setting labels
    if (currentRequiredSignNames.Count < 3)
    {
        Debug.LogError("Could not select 3 required signs!");
        return;
    }

    // Display road sign names in the slot labels
    for (int i = 0; i < slotLabels.Length && i < currentRequiredSignNames.Count; i++)
    {
        if (slotLabels[i] != null)
        {
            // Make sure text is visible
            slotLabels[i].text = currentRequiredSignNames[i];
            slotLabels[i].color = Color.black;
            slotLabels[i].fontSize = 18;

            Debug.Log($"Setting text for slot {i+1} to: {currentRequiredSignNames[i]}");
        }
        else
        {
            Debug.LogError($"Slot label {i} is null!");
        }
    }
    
    // Increment round counter
    currentRound++;
}
    // Inside SelectRandomRoadSigns method, modify it like this:
    private void SelectRandomRoadSigns()
    {
        // Safety check
        if (roadSignPrefabs == null || roadSignPrefabs.Count == 0)
        {
            Debug.LogError("No road sign prefabs assigned!");
            return;
        }

        // Create a copy of the road sign prefabs list
        List<GameObject> availableSigns = new List<GameObject>();

        // Only add non-null prefabs
        foreach (GameObject prefab in roadSignPrefabs)
        {
            if (prefab != null)
                availableSigns.Add(prefab);
        }

        if (availableSigns.Count == 0)
        {
            Debug.LogError("No valid road sign prefabs found!");
            return;
        }

        // Randomly select 3 signs (or fewer if we don't have enough)
        int signsToSelect = Mathf.Min(3, availableSigns.Count);

        Debug.Log($"==== REQUIRED SIGNS FOR ROUND {currentRound + 1} ====");

        for (int i = 0; i < signsToSelect; i++)
        {
            // Get a random index
            int randomIndex = Random.Range(0, availableSigns.Count);

            // Add the sign to our required signs
            GameObject selectedSign = availableSigns[randomIndex];
            currentRequiredSigns.Add(selectedSign);

            // Store clean name and log it
            string cleanName = selectedSign.name;
            Debug.Log($"Slot {i + 1} needs: {cleanName}");

            // Store the name for comparison later
            currentRequiredSignNames.Add(cleanName);

            // Remove from available signs to avoid duplicates
            availableSigns.RemoveAt(randomIndex);
        }
    }

    private bool AreAllSlotsCorrect()
    {
        // Check if winningSlots array is properly initialized
        if (winningSlots == null || winningSlots.Length < 3)
        {
            Debug.LogError("Winning slots array is null or has less than 3 slots!");
            return false;
        }

        // Check if we have enough required names
        if (currentRequiredSignNames == null || currentRequiredSignNames.Count < 3)
        {
            Debug.LogError("Not enough required sign names selected!");
            return false;
        }

        // Debug what we're checking
        Debug.Log($"Checking slots: Required signs are {string.Join(", ", currentRequiredSignNames)}");

        // Check all 3 slots
        for (int i = 0; i < 3 && i < winningSlots.Length; i++)
        {
            // Safely check if the slot exists
            if (winningSlots[i] == null)
            {
                Debug.LogError($"Slot {i} is null!");
                return false;
            }

            // Check if the slot has a child (a sign has been placed)
            if (winningSlots[i].transform.childCount == 0)
            {
                Debug.Log($"Slot {i} is empty");
                return false;
            }

            // Safely get the name of the child
            Transform child = winningSlots[i].transform.GetChild(0);
            if (child == null)
            {
                Debug.LogError($"Child in slot {i} is null!");
                return false;
            }

            // Get the name and clean it
            string signName = child.name;
            if (signName.EndsWith("(Clone)"))
                signName = signName.Substring(0, signName.Length - 7);

            // Debug what we found vs what we expected
            Debug.Log($"Slot {i} contains: {signName}, expecting: {currentRequiredSignNames[i]}");

            // Check if this matches the required sign
            if (i < currentRequiredSignNames.Count && signName != currentRequiredSignNames[i])
            {
                Debug.Log($"Wrong sign in slot {i}: found {signName}, expected {currentRequiredSignNames[i]}");
                return false;
            }
        }

        Debug.Log("All slots have correct signs!");
        return true;
    }

    private void OnRoundComplete()
    {
        Debug.Log("Round Complete!");

        // Show next button if we have more rounds to play
        if (currentRound < totalRounds && nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            // Game complete!
            Debug.Log("All rounds complete!");

            // Implement what happens when all rounds are complete
            // For example, show a "Continue" button to go back to the quiz
        }
    }

    private void OnNextButtonClicked()
    {
        // Start the next round
        StartNextRound();
    }

    // Public method to check if a specific road sign can be placed in a specific slot
    public bool CanPlaceSignInSlot(GameObject roadSign, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= currentRequiredSignNames.Count)
            return false;

        string signName = roadSign.name;

        // Remove "(Clone)" suffix if it exists
        if (signName.EndsWith("(Clone)"))
            signName = signName.Substring(0, signName.Length - 7);

        return signName == currentRequiredSignNames[slotIndex];
    }

    // Method to get the index of a winning slot
    public int GetSlotIndex(InventorySlot slot)
    {
        for (int i = 0; i < winningSlots.Length; i++)
        {
            if (winningSlots[i] == slot)
                return i;
        }
        return -1;
    }

    public void ResetCurrentRound()
{
    Debug.Log("Resetting current round...");

    // First, collect all signs from winning slots
    List<GameObject> signsToReturn = new List<GameObject>();
    
    foreach (InventorySlot slot in winningSlots)
    {
        if (slot == null) continue;
        
        // Get all children (should be at most one per slot)
        foreach (Transform child in slot.transform)
        {
            signsToReturn.Add(child.gameObject);
        }
    }
    
    // Now return each sign to its proper place
    foreach (GameObject sign in signsToReturn)
    {
        ReturnSignToContainer(sign);
    }

    // Hide next button
    if (nextButton != null)
        nextButton.gameObject.SetActive(false);

    // Reset round complete flag
    roundComplete = false;
}

    private void ReturnAllSignsToContainer()
    {
        // Find all DraggableItems in the scene
        DraggableItem[] allDraggableItems = FindObjectsOfType<DraggableItem>();

        foreach (DraggableItem item in allDraggableItems)
        {
            ReturnSignToContainer(item.gameObject);
        }
    }

    private void ReturnSignToContainer(GameObject sign)
    {
        if (sign == null) return;

        DraggableItem draggable = sign.GetComponent<DraggableItem>();
        if (draggable == null) return;

        // Find the original slot for this sign
        InventorySlot targetSlot = FindOriginalSlotForSign(sign);

        if (targetSlot != null)
        {
            // Return to specific slot
            sign.transform.SetParent(targetSlot.transform);

            // Center in slot
            RectTransform rectTransform = sign.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }

            Debug.Log($"Returning {sign.name} to its original slot: {targetSlot.name}");
        }
        else if (roadSignsContainer != null)
        {
            // If no specific slot found, return to general container
            sign.transform.SetParent(roadSignsContainer);

            // Randomize position within container
            RectTransform rectTransform = sign.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Get container size
                RectTransform containerRect = roadSignsContainer as RectTransform;
                if (containerRect != null)
                {
                    float width = containerRect.rect.width;
                    float height = containerRect.rect.height;

                    // Set random position within container
                    rectTransform.anchoredPosition = new Vector2(
                        Random.Range(-width / 2 + 50, width / 2 - 50),
                        Random.Range(-height / 2 + 50, height / 2 - 50)
                    );
                }
            }

            Debug.Log($"Returning {sign.name} to general container (no original slot found)");
        }

        // Re-enable dragging
        if (draggable != null)
        {
            CanvasGroup canvasGroup = draggable.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }

    // Helper method to find the original slot for a sign
    private InventorySlot FindOriginalSlotForSign(GameObject sign)
    {
        if (initialSlots == null || initialSlots.Length == 0)
            return null;

        // Get clean name
        string signName = sign.name;
        if (signName.EndsWith("(Clone)"))
            signName = signName.Substring(0, signName.Length - 7);

        // Try to find a matching slot by index in the prefabs list
        for (int i = 0; i < roadSignPrefabs.Count; i++)
        {
            if (roadSignPrefabs[i] != null && roadSignPrefabs[i].name == signName)
            {
                // If we have a matching slot for this index, return it
                if (i < initialSlots.Length && initialSlots[i] != null)
                    return initialSlots[i];
            }
        }

        // If no match by index, try to find an empty slot
        foreach (InventorySlot slot in initialSlots)
        {
            if (slot != null && slot.transform.childCount == 0)
                return slot;
        }

        return null;
    }


}