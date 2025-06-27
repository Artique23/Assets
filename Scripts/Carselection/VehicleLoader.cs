using UnityEngine;

public class VehicleLoader : MonoBehaviour
{
    public GameObject[] vehiclePrefabs; // If you're using flattened prefabs (one per color)
    public GameManagerSaveAndLoad.CarInfo[] carCatalog; // If you're referencing colorVariants
    public Transform vehicleMountPoint; // Empty object under PlayerModel

    private CarControls carControls;

    void Start()
    {
        carControls = GetComponent<CarControls>();
        if (carControls == null)
        {
            Debug.LogError("❌ CarControls script not found on PlayerModel.");
            return;
        }

        int selectedIndex = PlayerManager.Instance.GetSelectedCar();
        int selectedColor = PlayerManager.Instance.GetCarColorIndex(selectedIndex); // Add this method to your PlayerManager

        // Safety check
        if (selectedIndex < 0 || selectedIndex >= carCatalog.Length)
        {
            Debug.LogWarning("❗ Invalid car index, defaulting to 0");
            selectedIndex = 0;
        }

    if (selectedColor < 0 || selectedColor >= carCatalog[selectedIndex].colorVariants.Length)
    {
        Debug.LogWarning("❗ Invalid color index, defaulting to 3");
        selectedColor = 3;
    }

        // Instantiate selected color variant of selected car
        GameObject carModelToSpawn = carCatalog[selectedIndex].colorVariants[selectedColor];
        GameObject spawnedModel = Instantiate(carModelToSpawn, vehicleMountPoint.position, vehicleMountPoint.rotation);
        spawnedModel.transform.SetParent(vehicleMountPoint);

        RebindWheels(spawnedModel);
    }

    void RebindWheels(GameObject model)
    {
        Transform wheelColliderParent = model.transform.FindDeepChild("Wheel Collider");
        Transform wheelTransformParent = model.transform.FindDeepChild("Wheel Trannsform"); // typo handled below

        if (wheelColliderParent != null)
        {
            carControls.frontLeftWheelCollider = wheelColliderParent.Find("FL")?.GetComponent<WheelCollider>();
            carControls.frontRightWheelCollider = wheelColliderParent.Find("FR")?.GetComponent<WheelCollider>();
            carControls.rearLeftWheelCollider = wheelColliderParent.Find("BL")?.GetComponent<WheelCollider>();
            carControls.rearRightWheelCollider = wheelColliderParent.Find("BR")?.GetComponent<WheelCollider>();
        }
        else
        {
            Debug.LogError("❌ 'Wheel Collider' not found in model.");
        }

        if (wheelTransformParent != null)
        {
            carControls.frontLeftWheelTransform = wheelTransformParent.Find("FL");
            carControls.frontRightWheelTransform = wheelTransformParent.Find("FR");
            carControls.rearLeftWheelTransform = wheelTransformParent.Find("BL");
            carControls.rearRightWheelTransform = wheelTransformParent.Find("BR");
        }
        else
        {
            // Also try fixing typo if needed
            wheelTransformParent = model.transform.FindDeepChild("Wheel Transform");
            if (wheelTransformParent != null)
            {
                carControls.frontLeftWheelTransform = wheelTransformParent.Find("FL");
                carControls.frontRightWheelTransform = wheelTransformParent.Find("FR");
                carControls.rearLeftWheelTransform = wheelTransformParent.Find("BL");
                carControls.rearRightWheelTransform = wheelTransformParent.Find("BR");
            }
            else
            {
                Debug.LogError("❌ 'Wheel Transform' not found in model.");
            }
        }

        Debug.Log("✅ Vehicle loaded and wheels rebound.");
    }
}
