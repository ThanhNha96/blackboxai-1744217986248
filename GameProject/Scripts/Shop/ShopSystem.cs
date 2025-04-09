using UnityEngine;
using System.Collections.Generic;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }

    [System.Serializable]
    public class VehicleItem
    {
        public string id;
        public string nameKey;
        public string descriptionKey;
        public VehicleController.VehicleType type;
        public int basePrice;
        public float maxSpeed;
        public float acceleration;
        public float handling;
        public Sprite icon;
        public bool isUnlocked;
    }

    [System.Serializable]
    public class HouseItem
    {
        public string id;
        public string nameKey;
        public string descriptionKey;
        public int purchasePrice;
        public int rentPrice;
        public int size; // 1-5, affects storage and customization options
        public string location;
        public Sprite preview;
        public bool isOwned;
        public bool isRented;
    }

    [System.Serializable]
    public class ClothingItem
    {
        public string id;
        public string nameKey;
        public ClothingType type;
        public int price;
        public Sprite icon;
        public bool isUnlocked;
    }

    [System.Serializable]
    public class VehicleUpgrade
    {
        public string id;
        public string nameKey;
        public UpgradeType type;
        public int price;
        public float improvement;
        public int level;
        public Sprite icon;
    }

    public enum ClothingType
    {
        Hat, Shirt, Pants, Shoes, Collar, Backpack, Glasses
    }

    public enum UpgradeType
    {
        Engine, Suspension, Tires, Brakes, FuelTank, Nitro
    }

    [Header("Vehicles")]
    public VehicleItem[] vehicles = new VehicleItem[]
    {
        // Scooters
        new VehicleItem { id = "scooter_basic", nameKey = "VEHICLE_SCOOTER_BASIC", type = VehicleController.VehicleType.Scooter, basePrice = 1000, maxSpeed = 8f, acceleration = 15f, handling = 8f },
        new VehicleItem { id = "scooter_pro", nameKey = "VEHICLE_SCOOTER_PRO", type = VehicleController.VehicleType.Scooter, basePrice = 2500, maxSpeed = 10f, acceleration = 18f, handling = 9f },
        
        // Cars
        new VehicleItem { id = "car_basic", nameKey = "VEHICLE_CAR_BASIC", type = VehicleController.VehicleType.Car, basePrice = 5000, maxSpeed = 12f, acceleration = 20f, handling = 7f },
        new VehicleItem { id = "car_sport", nameKey = "VEHICLE_CAR_SPORT", type = VehicleController.VehicleType.Car, basePrice = 10000, maxSpeed = 15f, acceleration = 25f, handling = 8f },
        new VehicleItem { id = "car_luxury", nameKey = "VEHICLE_CAR_LUXURY", type = VehicleController.VehicleType.Car, basePrice = 20000, maxSpeed = 18f, acceleration = 30f, handling = 9f },
        
        // Bicycles
        new VehicleItem { id = "bike_basic", nameKey = "VEHICLE_BIKE_BASIC", type = VehicleController.VehicleType.Bicycle, basePrice = 500, maxSpeed = 6f, acceleration = 10f, handling = 9f },
        new VehicleItem { id = "bike_pro", nameKey = "VEHICLE_BIKE_PRO", type = VehicleController.VehicleType.Bicycle, basePrice = 1500, maxSpeed = 8f, acceleration = 12f, handling = 10f },
        
        // Skateboards
        new VehicleItem { id = "skateboard_basic", nameKey = "VEHICLE_SKATEBOARD_BASIC", type = VehicleController.VehicleType.Skateboard, basePrice = 200, maxSpeed = 5f, acceleration = 8f, handling = 7f },
        new VehicleItem { id = "skateboard_pro", nameKey = "VEHICLE_SKATEBOARD_PRO", type = VehicleController.VehicleType.Skateboard, basePrice = 800, maxSpeed = 7f, acceleration = 10f, handling = 9f }
    };

    [Header("Houses")]
    public HouseItem[] houses = new HouseItem[]
    {
        new HouseItem { id = "apartment_small", nameKey = "HOUSE_APARTMENT_SMALL", purchasePrice = 15000, rentPrice = 500, size = 1, location = "Downtown" },
        new HouseItem { id = "apartment_medium", nameKey = "HOUSE_APARTMENT_MEDIUM", purchasePrice = 25000, rentPrice = 800, size = 2, location = "Suburb" },
        new HouseItem { id = "house_small", nameKey = "HOUSE_SMALL", purchasePrice = 40000, rentPrice = 1200, size = 3, location = "Residential" },
        new HouseItem { id = "house_large", nameKey = "HOUSE_LARGE", purchasePrice = 75000, rentPrice = 2000, size = 4, location = "Luxury" },
        new HouseItem { id = "mansion", nameKey = "HOUSE_MANSION", purchasePrice = 150000, rentPrice = 5000, size = 5, location = "Elite" }
    };

    [Header("Clothing")]
    public Dictionary<ClothingType, int> clothingPrices = new Dictionary<ClothingType, int>()
    {
        { ClothingType.Hat, 100 },
        { ClothingType.Shirt, 150 },
        { ClothingType.Pants, 150 },
        { ClothingType.Shoes, 120 },
        { ClothingType.Collar, 80 },
        { ClothingType.Backpack, 200 },
        { ClothingType.Glasses, 100 }
    };

    [Header("Vehicle Upgrades")]
    public Dictionary<UpgradeType, int> baseUpgradePrices = new Dictionary<UpgradeType, int>()
    {
        { UpgradeType.Engine, 1000 },
        { UpgradeType.Suspension, 800 },
        { UpgradeType.Tires, 500 },
        { UpgradeType.Brakes, 600 },
        { UpgradeType.FuelTank, 400 },
        { UpgradeType.Nitro, 2000 }
    };

    [Header("Gas Station")]
    public float fuelPricePerLiter = 2.5f;
    public float maxFuelCapacity = 50f;
    private Dictionary<string, float> vehicleFuelLevels = new Dictionary<string, float>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool PurchaseVehicle(string vehicleId)
    {
        VehicleItem vehicle = System.Array.Find(vehicles, v => v.id == vehicleId);
        if (vehicle != null && !vehicle.isUnlocked && GameManager.Instance.SpendCoins(vehicle.basePrice))
        {
            vehicle.isUnlocked = true;
            UIManager.Instance.ShowMessage("VEHICLE_PURCHASED");
            return true;
        }
        return false;
    }

    public bool PurchaseHouse(string houseId)
    {
        HouseItem house = System.Array.Find(houses, h => h.id == houseId);
        if (house != null && !house.isOwned && GameManager.Instance.SpendCoins(house.purchasePrice))
        {
            house.isOwned = true;
            house.isRented = false;
            UIManager.Instance.ShowMessage("HOUSE_PURCHASED");
            return true;
        }
        return false;
    }

    public bool RentHouse(string houseId)
    {
        HouseItem house = System.Array.Find(houses, h => h.id == houseId);
        if (house != null && !house.isOwned && !house.isRented && GameManager.Instance.SpendCoins(house.rentPrice))
        {
            house.isRented = true;
            UIManager.Instance.ShowMessage("HOUSE_RENTED");
            return true;
        }
        return false;
    }

    public bool PurchaseClothing(ClothingType type, string itemId)
    {
        if (clothingPrices.ContainsKey(type) && GameManager.Instance.SpendCoins(clothingPrices[type]))
        {
            // Add clothing to inventory
            GameManager.Instance.AddClothingItem(type, itemId);
            UIManager.Instance.ShowMessage("CLOTHING_PURCHASED");
            return true;
        }
        return false;
    }

    public bool PurchaseUpgrade(string vehicleId, UpgradeType type)
    {
        if (baseUpgradePrices.ContainsKey(type))
        {
            VehicleUpgrade upgrade = GetVehicleUpgrade(vehicleId, type);
            int price = CalculateUpgradePrice(upgrade);
            
            if (GameManager.Instance.SpendCoins(price))
            {
                ApplyUpgrade(vehicleId, type);
                UIManager.Instance.ShowMessage("UPGRADE_PURCHASED");
                return true;
            }
        }
        return false;
    }

    public bool RefuelVehicle(string vehicleId, float amount)
    {
        if (!vehicleFuelLevels.ContainsKey(vehicleId))
        {
            vehicleFuelLevels[vehicleId] = maxFuelCapacity;
        }

        float currentFuel = vehicleFuelLevels[vehicleId];
        float maxRefill = maxFuelCapacity - currentFuel;
        float actualRefill = Mathf.Min(amount, maxRefill);
        float cost = actualRefill * fuelPricePerLiter;

        if (GameManager.Instance.SpendCoins((int)cost))
        {
            vehicleFuelLevels[vehicleId] += actualRefill;
            UIManager.Instance.ShowMessage("REFUEL_COMPLETE");
            return true;
        }
        return false;
    }

    private VehicleUpgrade GetVehicleUpgrade(string vehicleId, UpgradeType type)
    {
        // Get current upgrade level for vehicle
        return GameManager.Instance.GetVehicleUpgrade(vehicleId, type);
    }

    private int CalculateUpgradePrice(VehicleUpgrade upgrade)
    {
        // Price increases with each level
        return baseUpgradePrices[upgrade.type] * (upgrade.level + 1);
    }

    private void ApplyUpgrade(string vehicleId, UpgradeType type)
    {
        GameManager.Instance.UpgradeVehicle(vehicleId, type);
    }

    public float GetFuelLevel(string vehicleId)
    {
        return vehicleFuelLevels.ContainsKey(vehicleId) ? vehicleFuelLevels[vehicleId] : maxFuelCapacity;
    }

    public void ConsumeFuel(string vehicleId, float amount)
    {
        if (vehicleFuelLevels.ContainsKey(vehicleId))
        {
            vehicleFuelLevels[vehicleId] = Mathf.Max(0, vehicleFuelLevels[vehicleId] - amount);
        }
    }
}
