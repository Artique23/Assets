using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// This makes the PlayerManager stick around between scenes
public class PlayerManager : MonoBehaviour
{
    // This is a Singleton pattern - it means there's only one PlayerManager in the whole game
    private Dictionary<int, int> selectedCarColors = new Dictionary<int, int>();
    public List<int> selectedCarColorIndices;
    public static PlayerManager Instance { get; private set; }
    private int[] selectedColorIndices;
    // Player information
    [SerializeField] private int playerCurrency = 20;
    [SerializeField] private bool[] hasPlayedStage = new bool[4];
    [SerializeField] private int carsUnlocked = 1; // Start with 1 car unlocked
    [SerializeField] private bool[] unlockedCars = new bool[3]; // Track which cars are unlocked
    [SerializeField] private int selectedCar = 0; // Which car is selected now
    [SerializeField] private bool[] unlockedLevels = new bool[4];
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

// Adjusted method: subtract 1 to match hasPlayedStage array
    public bool HasPlayedStage(int sceneBuildIndex)
    {
        int stageIndex = sceneBuildIndex - 1;
        if (stageIndex >= 0 && stageIndex < hasPlayedStage.Length)
            return hasPlayedStage[stageIndex];
        return false;
    }

        
    public void MarkStageAsPlayed(int sceneBuildIndex)
    {
        int stageIndex = sceneBuildIndex - 1;
        if (stageIndex >= 0 && stageIndex < hasPlayedStage.Length)
        {
            hasPlayedStage[stageIndex] = true;
            SavePlayerData();
        }
    }


    void Awake()
    {
            if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            selectedColorIndices = new int[5]; // Replace 5 with the total number of cars
            for (int i = 0; i < selectedColorIndices.Length; i++)
                selectedColorIndices[i] = 0; // default color index
        }
        else
        {
            Destroy(gameObject);
        }
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
        saveFilePath = System.IO.Path.Combine(Application.persistentDataPath, "player_save.json");

        // Set the first car to be always unlocked
        for (int i = 0; i < unlockedCars.Length; i++)
        {
             unlockedCars[i] = (i == 0);
        }
            unlockedLevels[0] = true;
        for (int i = 1; i < unlockedLevels.Length; i++)
        unlockedLevels[i] = false;

        // Load player data when game starts
        LoadPlayerData();


        
        Debug.Log("Save file is at: " + saveFilePath);
    }

    public int GetCarColorIndex(int carIndex)
    {
        if (!selectedCarColors.ContainsKey(carIndex))
        {
            selectedCarColors[carIndex] = 3; // ✅ Default to 4th color
        }
        return selectedCarColors[carIndex];
    }

    public void SetCarColorIndex(int carIndex, int colorIndex)
    {
        selectedCarColors[carIndex] = colorIndex;
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
        Debug.Log($"Attempting to spend {amount} currency. Current balance: {playerCurrency}");
        
        // Check if player has enough money
        if (playerCurrency >= amount)
        {
            playerCurrency -= amount;
            SavePlayerData();
            Debug.Log($"Purchase successful! New balance: {playerCurrency}");
            return true; // Purchase successful
        }
        
        Debug.Log("Purchase failed - insufficient funds!");
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
    
        public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < unlockedLevels.Length)
            return unlockedLevels[levelIndex];
        return false;
    }
public void UnlockLevel(int levelIndex)
{
    if (levelIndex >= 0 && levelIndex < unlockedLevels.Length)
    {
        if (!unlockedLevels[levelIndex])
        {
            Debug.Log($"✅ Unlocking Level {levelIndex}");
            unlockedLevels[levelIndex] = true;
            SavePlayerData();
        }
        else
        {
            Debug.Log($"ℹ️ Level {levelIndex} is already unlocked.");
        }
    }
    else
    {
        Debug.LogWarning($"❌ Invalid level index: {levelIndex}. Max allowed: {unlockedLevels.Length - 1}");
    }
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
            // Reset level unlock states
        for (int i = 0; i < unlockedLevels.Length; i++)
        {
            unlockedLevels[i] = (i == 0); // Only first level is unlocked
        }

        // Reset played stages
        for (int i = 0; i < hasPlayedStage.Length; i++)
        {
            hasPlayedStage[i] = false;
        }

        SavePlayerData();
        Debug.Log("Game progress has been reset!");
    }
    
    // Save & Load functions
    
public void SavePlayerData()
{
   
    try
        {
            // Create a new PlayerSaveData object with our current values

            PlayerSaveData data = new PlayerSaveData
            {
                currency = playerCurrency,
                carsUnlocked = carsUnlocked,
                unlockedCars = unlockedCars,
                unlockedLevels = unlockedLevels,
                hasPlayedStage = hasPlayedStage,
                selectedCarIndex = selectedCar,
                selectedCarColorIndices = new List<int>()
                
            };

            for (int i = 0; i < unlockedCars.Length; i++)
            {
                int colorIndex = GetCarColorIndex(i);
                data.selectedCarColorIndices.Add(colorIndex);
            }
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
            if (File.Exists(saveFilePath))
            {
                string jsonData = File.ReadAllText(saveFilePath);
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(jsonData);

                playerCurrency = data.currency;
                carsUnlocked = data.carsUnlocked;

                if (data.unlockedCars != null && data.unlockedCars.Length > 0)
                {
                    int count = Mathf.Min(unlockedCars.Length, data.unlockedCars.Length);
                    for (int i = 0; i < count; i++)
                    {
                        unlockedCars[i] = data.unlockedCars[i];
                    }
                }

                if (data.selectedCarIndex >= 0 && data.selectedCarIndex < unlockedCars.Length)
                {
                    selectedCar = data.selectedCarIndex;
                }

                // ✅ Load color data
                if (data.selectedCarColorIndices != null)
                {
                    for (int i = 0; i < data.selectedCarColorIndices.Count; i++)
                    {
                        selectedCarColors[i] = data.selectedCarColorIndices[i];
                    }
                }
                if (data.unlockedLevels != null && data.unlockedLevels.Length > 0)
                {
                    int count = Mathf.Min(unlockedLevels.Length, data.unlockedLevels.Length);
                    for (int i = 0; i < count; i++)
                        unlockedLevels[i] = data.unlockedLevels[i];
                }
                if (data.hasPlayedStage != null && data.hasPlayedStage.Length == hasPlayedStage.Length)
                {
                    for (int i = 0; i < data.hasPlayedStage.Length; i++)
                        hasPlayedStage[i] = data.hasPlayedStage[i];
                }
            }
            else
            {

            }
        }
        catch (System.Exception e)
        {

        }
}

public void UnlockAllStages()
{
    for (int i = 0; i < unlockedLevels.Length; i++)
    {
        unlockedLevels[i] = true;
    }
    SavePlayerData();
    Debug.Log("✅ All stages unlocked!");
}

    // Add this method to your PlayerManager class
    public void Add10Currency()
    {
        // Add 10 currency
        playerCurrency += 10;

        // Save the data immediately
        SavePlayerData();

        // Find and update all GameManagerSaveAndLoad instances
        var managers = FindObjectsOfType<GameManagerSaveAndLoad>();
        foreach (var manager in managers)
        {
            if (manager != null)
            {
                manager.UpdateUI(); // Call UpdateUI directly instead of using SendMessage
            }
        }

        Debug.Log($"Added 10 currency. New total: {playerCurrency}");
    }
}


// This class defines what gets saved to file
[System.Serializable]
public class PlayerSaveData
{
    public int currency;
    public int carsUnlocked;
    public bool[] unlockedCars;
    public bool[] hasPlayedStage;
    public bool[] unlockedLevels;
    public int selectedCarIndex;
    public List<int> selectedCarColorIndices;
}
[System.Serializable]
public class StageStarEntry
{
    public int stageIndex;
    public int starsEarned;
}