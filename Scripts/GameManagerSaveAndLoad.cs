using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoadScript : MonoBehaviour
{
    [SerializeField] private GameObject[] carPrefabs; // Array of car prefabs
    
    private int currentCarIndex = 0;
    private GameObject currentCarInstance;
    private string saveFilePath;
    
    void Start()
    {
        // Set up save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "carSelection.json");
        
        // Load previous car selection if available
        LoadCarSelection();
        
        // Spawn the current car
        SpawnCurrentCar();
    }
    
    public void NextCar()
    {
        currentCarIndex++;
        if (currentCarIndex >= carPrefabs.Length)
            currentCarIndex = 0;
            
        SpawnCurrentCar();
        SaveCarSelection();
    }
    
    public void PreviousCar()
    {
        currentCarIndex--;
        if (currentCarIndex < 0)
            currentCarIndex = carPrefabs.Length - 1;
            
        SpawnCurrentCar();
        SaveCarSelection();
    }
    
    private void SpawnCurrentCar()
    {
        // Destroy previous car instance if it exists
        if (currentCarInstance != null)
            Destroy(currentCarInstance);
        
        // Instantiate selected car prefab at its default position
        currentCarInstance = Instantiate(carPrefabs[currentCarIndex]);
        
        // Car will spawn at the position and rotation that is set in the prefab
    }
    
    private void SaveCarSelection()
    {
        CarSelectionData data = new CarSelectionData
        {
            selectedCarIndex = currentCarIndex
        };
        
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, jsonData);
    }
    
    private void LoadCarSelection()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            CarSelectionData data = JsonUtility.FromJson<CarSelectionData>(jsonData);
            
            // Make sure the loaded index is valid
            if (data.selectedCarIndex >= 0 && data.selectedCarIndex < carPrefabs.Length)
                currentCarIndex = data.selectedCarIndex;
        }
    }
}


[System.Serializable]
public class CarSelectionData
{
    public int selectedCarIndex;
}