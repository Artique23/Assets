using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReplaceWithSelectedCar : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject[] carPrefabs; // Make sure the order matches your catalog

    void Start()
    {
        // 1. Get selected car index from PlayerManager
        int selectedCarIndex = PlayerManager.Instance.GetSelectedCar();

        // 2. Instantiate the selected car
        GameObject spawnedCar;
        if (selectedCarIndex >= 0 && selectedCarIndex < carPrefabs.Length)
        {
            spawnedCar = Instantiate(carPrefabs[selectedCarIndex], spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Invalid car index. Spawning default car.");
            spawnedCar = Instantiate(carPrefabs[0], spawnPoint.position, spawnPoint.rotation);
        }

        // 3. Find the ChallengeModeManager in the scene
        ChallengeModeManager challengeManager = FindObjectOfType<ChallengeModeManager>();
        if (challengeManager == null)
        {
            Debug.LogWarning("ChallengeModeManager not found in the scene.");
            return;
        }

        // 4. Get CarControls from the spawned car
        CarControls controls = spawnedCar.GetComponentInChildren<CarControls>();
        if (controls != null)
        {
            challengeManager.carControls = controls;
            controls.carPoweredOn = true;

            // Assign gear shift controller from the scene, if any
            controls.gearShiftController = challengeManager.GetComponentInChildren<GearShiftController>();

            // Assign pedal buttons from the scene
            controls.acceleratorButton = challengeManager.acceleratorButton;
            controls.brakeButton = challengeManager.brakeButton;

            // Re-apply button events
            AddButtonEvents(challengeManager.acceleratorButton, controls.OnAcceleratorDown, controls.OnAcceleratorUp);
            AddButtonEvents(challengeManager.brakeButton, controls.OnBrakeDown, controls.OnBrakeUp);

            Debug.Log("CarControls successfully connected to ChallengeModeManager.");
        }
        else
        {
            Debug.LogWarning("CarControls not found on the spawned car.");
        }
    }

    private void AddButtonEvents(Button button, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
    {
        if (button == null) return;

        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // Clear previous triggers to prevent duplicates
        trigger.triggers.Clear();

        // PointerDown
        EventTrigger.Entry entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        entryDown.callback.AddListener((eventData) => onDown());
        trigger.triggers.Add(entryDown);

        // PointerUp
        EventTrigger.Entry entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entryUp.callback.AddListener((eventData) => onUp());
        trigger.triggers.Add(entryUp);
    }
}
