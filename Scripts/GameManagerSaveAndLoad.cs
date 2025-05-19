using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerSaveAndLoad : MonoBehaviour
{
    [System.Serializable]
    public class CarInfo
    {
        public GameObject carPrefab;
        public int price;
        public string carName;
    }
    
    [SerializeField] private CarInfo[] carCatalog; // Info about each car
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private GameObject selectButton; // Reference to the select button
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI carNameText;
    [SerializeField] private GameObject selectedIndicator; // Optional - an object that shows this car is selected
    
    private int currentDisplayIndex = 0;
    private GameObject currentCarInstance;
    
    // This is called when the shop screen loads
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
        
        // Show the current car
        DisplayCurrentCar();
        
        // Update all UI elements
        UpdateUI();
    }
    
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
    
    // Next and Previous car buttons
    public void NextCar()
    {
        currentDisplayIndex++;
        if (currentDisplayIndex >= carCatalog.Length)
            currentDisplayIndex = 0;

        DisplayCurrentCar();
        UpdateUI();
    }
    
    public void PreviousCar()
    {
        currentDisplayIndex--;
        if (currentDisplayIndex < 0)
            currentDisplayIndex = carCatalog.Length - 1;
            
        DisplayCurrentCar();
        UpdateUI();
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
            
            // Update UI to hide select button for currently selected car
            UpdateUI();
        }
        else
        {
            Debug.Log("Can't select locked car!");
        }
    }
    
    // Buy button was clicked
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
    
    // Add stars button (for testing)
    public void AddStars(int amount)
    {
        PlayerManager.Instance.AddCurrency(amount);
        UpdateUI();
    }
    
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
        // Remove old car if it exists
        if (currentCarInstance != null)
            Destroy(currentCarInstance);
        
        // Create new car if valid index
        if (currentDisplayIndex >= 0 && currentDisplayIndex < carCatalog.Length)
        {
            currentCarInstance = Instantiate(carCatalog[currentDisplayIndex].carPrefab);
        }
    }
}