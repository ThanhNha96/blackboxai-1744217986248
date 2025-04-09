using UnityEngine;

public class GasStation : MonoBehaviour
{
    public float refuelSpeed = 1f; // Liters per second
    public float priceMultiplier = 1f; // Local price modifier
    public Transform refuelPoint;
    public ParticleSystem fuelEffect;
    public AudioClip refuelSound;
    
    private bool isRefueling;
    private VehicleController currentVehicle;
    private float basePrice;

    void Start()
    {
        basePrice = ShopSystem.Instance.fuelPricePerLiter;
        SetupGasStation();
    }

    void SetupGasStation()
    {
        // Add trigger collider for vehicle detection
        BoxCollider2D trigger = gameObject.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(5f, 3f); // Area where vehicles can refuel
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vehicle"))
        {
            VehicleController vehicle = other.GetComponent<VehicleController>();
            if (vehicle != null)
            {
                ShowRefuelPrompt(vehicle);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Vehicle"))
        {
            HideRefuelPrompt();
            StopRefueling();
        }
    }

    void ShowRefuelPrompt(VehicleController vehicle)
    {
        currentVehicle = vehicle;
        float currentFuel = ShopSystem.Instance.GetFuelLevel(vehicle.vehicleId);
        float maxFuel = ShopSystem.Instance.maxFuelCapacity;
        float localPrice = basePrice * priceMultiplier;

        string message = string.Format(
            LocalizationManager.Instance.GetLocalizedText("REFUEL_PROMPT"),
            currentFuel.ToString("F1"),
            maxFuel.ToString("F1"),
            localPrice.ToString("F2")
        );

        UIManager.Instance.ShowRefuelPrompt(message, StartRefueling, CancelRefueling);
    }

    void HideRefuelPrompt()
    {
        UIManager.Instance.HideRefuelPrompt();
    }

    void StartRefueling()
    {
        if (currentVehicle != null)
        {
            isRefueling = true;
            if (fuelEffect != null)
                fuelEffect.Play();
            if (refuelSound != null)
                AudioManager.Instance.PlaySFX(refuelSound);
            
            StartCoroutine(RefuelRoutine());
        }
    }

    void StopRefueling()
    {
        isRefueling = false;
        if (fuelEffect != null)
            fuelEffect.Stop();
        currentVehicle = null;
    }

    void CancelRefueling()
    {
        StopRefueling();
        HideRefuelPrompt();
    }

    System.Collections.IEnumerator RefuelRoutine()
    {
        while (isRefueling && currentVehicle != null)
        {
            float currentFuel = ShopSystem.Instance.GetFuelLevel(currentVehicle.vehicleId);
            float maxFuel = ShopSystem.Instance.maxFuelCapacity;
            
            if (currentFuel < maxFuel)
            {
                float fuelToAdd = refuelSpeed * Time.deltaTime;
                float cost = fuelToAdd * basePrice * priceMultiplier;
                
                if (GameManager.Instance.CanSpendCoins((int)cost))
                {
                    if (ShopSystem.Instance.RefuelVehicle(currentVehicle.vehicleId, fuelToAdd))
                    {
                        // Update UI
                        UpdateRefuelUI(currentFuel + fuelToAdd, maxFuel);
                    }
                    else
                    {
                        // Not enough money
                        StopRefueling();
                        UIManager.Instance.ShowMessage("NOT_ENOUGH_MONEY");
                        break;
                    }
                }
                else
                {
                    // Tank is full
                    StopRefueling();
                    UIManager.Instance.ShowMessage("TANK_FULL");
                    break;
                }
            }
            
            yield return null;
        }
    }

    void UpdateRefuelUI(float currentFuel, float maxFuel)
    {
        string message = string.Format(
            LocalizationManager.Instance.GetLocalizedText("REFUELING_STATUS"),
            currentFuel.ToString("F1"),
            maxFuel.ToString("F1")
        );
        
        UIManager.Instance.UpdateRefuelStatus(message);
    }
}
