using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerSaveAndLoad : MonoBehaviour
{
    private int[] selectedColorIndices;

    #region Car Information
    [System.Serializable]
    public class CarInfo
    {
        public GameObject carPrefab;
        public int price;
        public string carName;
        public GameObject[] colorVariants;
    }
    #endregion

    #region Variables
    [SerializeField] private CarInfo[] carCatalog; // Info about each car

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private GameObject selectButton; // Reference to the select button
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI carNameText;
    [SerializeField] private GameObject selectedIndicator; // Optional - an object that shows this car is selected

    [Header("Selection Animation Settings")]
    [SerializeField] private float selectionPulseScale = 1.5f;      // How big the indicator gets when pulsing
    [SerializeField] private float selectionPulseDuration = 0.1f;   // How long each pulse lasts
    [SerializeField] private int selectionPulseCount = 2;           // How many pulses to perform
    [SerializeField] private bool useColorPulse = true;             // Whether to pulse color too
    [SerializeField] private Color selectionHighlightColor = new Color(1f, 0.8f, 0.2f); // Gold highlight color

    public int currentDisplayIndex = 0;
    private GameObject currentCarInstance;
    #endregion


    #region OnStart
    void Start()
    {
        // Make sure we have a player manager
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("No PlayerManager found! Make sure it exists in the scene.");
            return;
        }

        // Set the display index to the player's selected car
        currentDisplayIndex = PlayerManager.Instance.GetSelectedCar();
    selectedColorIndices = new int[carCatalog.Length];
        for (int i = 0; i < selectedColorIndices.Length; i++)
        {
            selectedColorIndices[i] = 3; // <-- Your actual default index
        }
        // Show the current car
        DisplayCurrentCar();

        // Update all UI elements
        UpdateUI();
    }
    #endregion

    #region Game Management Methods
    // Function to exit the game - can be connected to a UI button
    public void ExitGame()
    {
        Debug.Log("Exiting game...");

#if UNITY_EDITOR
        // If running in editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a build, quit the application
        Application.Quit();
#endif
    }
    #endregion

    #region Testing methods
    // For testing only
    public void ResetGame()
    {
        // Call the reset method in PlayerManager
        PlayerManager.Instance.ResetProgress();

        // Display the first car
        currentDisplayIndex = 0;
        DisplayCurrentCar();

        // Update UI
        UpdateUI();

        Debug.Log("TESTING: Game reset to default state!");
    }
    #endregion


    #region Car Selection Methods
    // Next and Previous car buttons
    public void NextCar()
    {
        currentDisplayIndex++;
        if (currentDisplayIndex >= carCatalog.Length)
            currentDisplayIndex = 0;

        DisplayCurrentCar();
        UpdateUI();

        // Find and refresh the MenusUIScript to update color panels if open
        MenusUIScript menuUI = FindObjectOfType<MenusUIScript>();
        if (menuUI != null)
        {
            menuUI.RefreshCustomizeMenu();
        }
    }

    public void PreviousCar()
    {
        currentDisplayIndex--;
        if (currentDisplayIndex < 0)
            currentDisplayIndex = carCatalog.Length - 1;

        DisplayCurrentCar();
        UpdateUI();

        // Find and refresh the MenusUIScript to update color panels if open
        MenusUIScript menuUI = FindObjectOfType<MenusUIScript>();
        if (menuUI != null)
        {
            menuUI.RefreshCustomizeMenu();
        }
    }

    // Select the current car (if unlocked)
    public void SelectCar()
    {
        // Only select if car is unlocked
        if (PlayerManager.Instance.IsCarUnlocked(currentDisplayIndex))
        {
            // Get car name before setting it as selected
            string carName = carCatalog[currentDisplayIndex].carName;

            // Set as selected car
            PlayerManager.Instance.SetSelectedCar(currentDisplayIndex);

            // Log detailed information
            Debug.Log("Selected car: " + carName + " (Index: " + currentDisplayIndex + ")");
            Debug.Log("This car will now be used in the game!");

            // Add visual feedback
            StartCoroutine(ShowSelectionConfirmation());

            // Update UI to hide select button for currently selected car
            UpdateUI();
        }
        else
        {
            Debug.Log("Can't select locked car!");
        }
    }

    // Add this method for visual confirmation
    private IEnumerator ShowSelectionConfirmation()
    {
        // Flash the selected indicator if it exists
        if (selectedIndicator != null)
        {
            // Make sure it's active
            selectedIndicator.SetActive(true);

            // Get the original scale and color
            Vector3 originalScale = selectedIndicator.transform.localScale;

            // Get image component if we're doing color changes
            Image indicatorImage = null;
            Color originalColor = Color.white;

            if (useColorPulse)
            {
                indicatorImage = selectedIndicator.GetComponent<Image>();
                if (indicatorImage != null)
                {
                    originalColor = indicatorImage.color;
                }
            }

            // Flash effect
            for (int i = 0; i < selectionPulseCount; i++)
            {
                // Scale up
                selectedIndicator.transform.localScale = originalScale * selectionPulseScale;

                // Change color if enabled
                if (useColorPulse && indicatorImage != null)
                {
                    indicatorImage.color = selectionHighlightColor;
                }

                yield return new WaitForSeconds(selectionPulseDuration);

                // Scale down
                selectedIndicator.transform.localScale = originalScale;

                // Restore color
                if (useColorPulse && indicatorImage != null)
                {
                    indicatorImage.color = originalColor;
                }

                yield return new WaitForSeconds(selectionPulseDuration);
            }

            // Ensure original state is restored
            selectedIndicator.transform.localScale = originalScale;
            if (useColorPulse && indicatorImage != null)
            {
                indicatorImage.color = originalColor;
            }
        }
    }

    #endregion


    #region Car Purchase Methods

    // Buy Car button was clicked
    public void BuyCar()
    {
        // Get the current car price
        int price = carCatalog[currentDisplayIndex].price;
        string carName = carCatalog[currentDisplayIndex].carName;

        // Try to spend currency
        if (PlayerManager.Instance.SpendCurrency(price))
        {
            // Purchase successful
            PlayerManager.Instance.UnlockCar(currentDisplayIndex);

            // Select this car automatically
            PlayerManager.Instance.SetSelectedCar(currentDisplayIndex);

            Debug.Log("Car purchased: " + carName + " for " + price + " stars!");
            Debug.Log("This car is now selected and will be used in the game.");

            // Update UI to show new state
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough stars to buy " + carName + "! (Costs: " + price + " stars)");
        }
    }

    #endregion

    #region PlayerManager/CarMenu Methods

    // Update all UI elements based on current state
    public void UpdateUI()
    {
        // Update currency display
        if (currencyText != null)
        {
            currencyText.text = PlayerManager.Instance.GetCurrency().ToString();
        }

        // Update car-specific UI
        if (currentDisplayIndex >= 0 && currentDisplayIndex < carCatalog.Length)
        {
            // Is this car unlocked?
            bool isUnlocked = PlayerManager.Instance.IsCarUnlocked(currentDisplayIndex);

            // Is this the currently selected car?
            bool isSelected = (PlayerManager.Instance.GetSelectedCar() == currentDisplayIndex);

            // Show/hide lock icon
            if (lockOverlay != null)
            {
                lockOverlay.SetActive(!isUnlocked);
            }

            // Show/hide buy button
            if (buyButton != null)
            {
                // Only show button if car is locked AND player has enough money
                bool canAfford = PlayerManager.Instance.GetCurrency() >= carCatalog[currentDisplayIndex].price;
                buyButton.SetActive(!isUnlocked && canAfford);
            }

            // Show/hide select button based on unlock status AND if it's already selected
            if (selectButton != null)
            {
                // Show select button only if:
                // 1. Car is unlocked AND
                // 2. Car is NOT currently selected
                selectButton.SetActive(isUnlocked && !isSelected);
            }
            else
            {
                Debug.LogWarning("Select Button is not assigned in the Inspector!");
            }

            // Show/hide selected indicator (if we have one)
            if (selectedIndicator != null)
            {
                selectedIndicator.SetActive(isSelected);
            }

            // Update price text
            if (priceText != null)
            {
                int price = carCatalog[currentDisplayIndex].price;
                priceText.text = price.ToString() + " Stars";
                priceText.gameObject.SetActive(!isUnlocked);
            }

            // Update car name
            if (carNameText != null)
            {
                carNameText.text = carCatalog[currentDisplayIndex].carName;
            }
        }
    }

    // Display the current car in the viewer
    private void DisplayCurrentCar()
    {
        if (currentCarInstance != null)
            Destroy(currentCarInstance);

        if (currentDisplayIndex >= 0 && currentDisplayIndex < carCatalog.Length)
        {
            int colorIndex = selectedColorIndices[currentDisplayIndex];
            colorIndex = Mathf.Clamp(colorIndex, 0, carCatalog[currentDisplayIndex].colorVariants.Length - 1);

            currentCarInstance = Instantiate(carCatalog[currentDisplayIndex].colorVariants[colorIndex]);
        }
    }


    // Get the current car index for external access
    public int GetCurrentCarIndex()
    {
        return currentDisplayIndex;
    }
    
    public void SetCarColorIndex(int colorIndex)
{
    if (currentDisplayIndex < 0 || currentDisplayIndex >= carCatalog.Length)
        return;

    // Clamp color index and update
    int maxIndex = carCatalog[currentDisplayIndex].colorVariants.Length - 1;
    selectedColorIndices[currentDisplayIndex] = Mathf.Clamp(colorIndex, 0, maxIndex);

    // Re-display car with new color
    DisplayCurrentCar();
}

    #endregion
}