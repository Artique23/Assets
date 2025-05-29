using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// This makes the PlayerManager stick around between scenes
public class PlayerManager : MonoBehaviour
{
    // This is a Singleton pattern - it means there's only one PlayerManager in the whole game
    public static PlayerManager Instance { get; private set; }
    
    // Player information
    [SerializeField] private int playerCurrency = 0;
    [SerializeField] private int carsUnlocked = 1; // Start with 1 car unlocked
    [SerializeField] private bool[] unlockedCars = new bool[3]; // Track which cars are unlocked
    [SerializeField] private int selectedCar = 0; // Which car is selected now
    
    // Path to save file
    private string saveFilePath;
    
    // This special function runs whenever you change values in the Inspector during play mode
    private void OnValidate()
    {
        // Only run this code during play mode (not in edit mode)
        if (Application.isPlaying && Instance == this)
        {
            // Save any changes made in Inspector
            SavePlayerData();
            
            // Find all GameManagerSaveAndLoad scripts and update their UI
            var managers = FindObjectsOfType<GameManagerSaveAndLoad>();
            if (managers != null && managers.Length > 0)
            {
                foreach (var manager in managers)
                {
                    // Call the UpdateUI method on each manager found
                    if (manager != null)
                    {
                        manager.SendMessage("UpdateUI", null, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            
            Debug.Log("Updated currency to: " + playerCurrency);
        }
    }

    void Awake()
    {
        // If there's already a PlayerManager, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Otherwise, this is THE player manager
        Instance = this;

        // Make it survive when switching scenes
        DontDestroyOnLoad(gameObject);

        // Set save file location
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerProgress.json");

        // Set the first car to be always unlocked
        unlockedCars[0] = true;

        // Load player data when game starts
        LoadPlayerData();
        
        Debug.Log("Save file is at: " + saveFilePath);
    }
    
    // Functions for other scripts to use
    
    public int GetCurrency()
    {
        return playerCurrency;
    }
    
    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        SavePlayerData();
    }
    
    public bool SpendCurrency(int amount)
    {
        // Check if player has enough money
        if (playerCurrency >= amount)
        {
            playerCurrency -= amount;
            SavePlayerData();
            return true; // Purchase successful
        }
        
        return false; // Not enough money
    }
    
    public bool IsCarUnlocked(int carIndex)
    {
        if (carIndex >= 0 && carIndex < unlockedCars.Length)
        {
            return unlockedCars[carIndex];
        }
        return false;
    }
    
    public void UnlockCar(int carIndex)
    {
        if (carIndex >= 0 && carIndex < unlockedCars.Length)
        {
            if (!unlockedCars[carIndex])
            {
                unlockedCars[carIndex] = true;
                carsUnlocked++;
                SavePlayerData();
            }
        }
    }
    
    public int GetSelectedCar()
    {
        return selectedCar;
    }
    
    public void SetSelectedCar(int carIndex)
    {
        if (carIndex >= 0 && carIndex < unlockedCars.Length)
        {
            selectedCar = carIndex;
            SavePlayerData();
        }
    }
    
    // For testing - set currency directly
    public void SetCurrency(int amount)
    {
        playerCurrency = Mathf.Max(0, amount); // Prevent negative values
        SavePlayerData();
    }
    
    // Reset all game progress (for testing)
    public void ResetProgress()
    {
        playerCurrency = 0;
        carsUnlocked = 1;
        selectedCar = 0;
        
        // Reset car unlock states
        for (int i = 0; i < unlockedCars.Length; i++)
        {
            unlockedCars[i] = (i == 0); // Only first car is unlocked
        }
        
        SavePlayerData();
        Debug.Log("Game progress has been reset!");
    }
    
    // Save & Load functions
    
    private void SavePlayerData()
    {
        try
        {
            // Create a new PlayerSaveData object with our current values
            PlayerSaveData data = new PlayerSaveData
            {
                currency = playerCurrency,
                carsUnlocked = carsUnlocked,
                unlockedCars = unlockedCars,
                selectedCarIndex = selectedCar
            };
            
            // Convert it to JSON text
            string jsonData = JsonUtility.ToJson(data, true);
            
            // Save to file
            File.WriteAllText(saveFilePath, jsonData);
            
            Debug.Log("Player data saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save player data: " + e.Message);
        }
    }
    
    private void LoadPlayerData()
    {
        try
        {
            // Check if save file exists
            if (File.Exists(saveFilePath))
            {
                // Read the file contents
                string jsonData = File.ReadAllText(saveFilePath);
                
                // Convert JSON text back to data
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(jsonData);
                
                // Apply the loaded values
                playerCurrency = data.currency;
                carsUnlocked = data.carsUnlocked;
                
                // Copy the unlock states if available
                if (data.unlockedCars != null && data.unlockedCars.Length > 0)
                {
                    // Make sure array sizes match
                    int count = Mathf.Min(unlockedCars.Length, data.unlockedCars.Length);
                    
                    for (int i = 0; i < count; i++)
                    {
                        unlockedCars[i] = data.unlockedCars[i];
                    }
                }
                
                // Set selected car
                if (data.selectedCarIndex >= 0 && data.selectedCarIndex < unlockedCars.Length)
                {
                    selectedCar = data.selectedCarIndex;
                }
                
                Debug.Log("Player data loaded successfully! Currency: " + playerCurrency);
            }
            else
            {
                Debug.Log("No save file found. Using default values.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load player data: " + e.Message);
        }
    }
}

// This class defines what gets saved to file
[System.Serializable]
public class PlayerSaveData
{
    public int currency;
    public int carsUnlocked;
    public bool[] unlockedCars;
    public int selectedCarIndex;
}