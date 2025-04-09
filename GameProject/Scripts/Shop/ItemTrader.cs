using UnityEngine;
using System.Collections.Generic;

public class ItemTrader : MonoBehaviour
{
    [System.Serializable]
    public class TradeItem
    {
        public string id;
        public string nameKey;
        public string descriptionKey;
        public ItemType type;
        public int price;
        public int stock;
        public bool isSpecial;
        public Sprite icon;
    }

    public enum ItemType
    {
        DogFood,
        DogToy,
        DogTreat,
        CatchingNet,
        StealthDevice,
        VehiclePart,
        SpecialItem
    }

    [Header("Shop Settings")]
    public float restockTime = 1800f; // 30 minutes
    public int maxRegularItems = 8;
    public int maxSpecialItems = 2;

    [Header("Available Items")]
    public TradeItem[] regularItems = new TradeItem[]
    {
        // Dog Food
        new TradeItem {
            id = "food_premium",
            nameKey = "ITEM_PREMIUM_FOOD",
            type = ItemType.DogFood,
            price = 100,
            stock = 10
        },
        new TradeItem {
            id = "food_deluxe",
            nameKey = "ITEM_DELUXE_FOOD",
            type = ItemType.DogFood,
            price = 200,
            stock = 5
        },

        // Dog Toys
        new TradeItem {
            id = "toy_ball",
            nameKey = "ITEM_TOY_BALL",
            type = ItemType.DogToy,
            price = 50,
            stock = 15
        },
        new TradeItem {
            id = "toy_bone",
            nameKey = "ITEM_TOY_BONE",
            type = ItemType.DogToy,
            price = 75,
            stock = 10
        },

        // Dog Treats
        new TradeItem {
            id = "treat_basic",
            nameKey = "ITEM_BASIC_TREAT",
            type = ItemType.DogTreat,
            price = 30,
            stock = 20
        },
        new TradeItem {
            id = "treat_special",
            nameKey = "ITEM_SPECIAL_TREAT",
            type = ItemType.DogTreat,
            price = 150,
            stock = 8
        },

        // Catching Equipment
        new TradeItem {
            id = "net_basic",
            nameKey = "ITEM_BASIC_NET",
            type = ItemType.CatchingNet,
            price = 500,
            stock = 3
        },
        new TradeItem {
            id = "stealth_basic",
            nameKey = "ITEM_BASIC_STEALTH",
            type = ItemType.StealthDevice,
            price = 800,
            stock = 2
        },

        // Vehicle Parts
        new TradeItem {
            id = "engine_part",
            nameKey = "ITEM_ENGINE_PART",
            type = ItemType.VehiclePart,
            price = 300,
            stock = 5
        },
        new TradeItem {
            id = "turbo_part",
            nameKey = "ITEM_TURBO_PART",
            type = ItemType.VehiclePart,
            price = 1000,
            stock = 2
        }
    };

    [Header("Special Items")]
    public TradeItem[] specialItems = new TradeItem[]
    {
        new TradeItem {
            id = "master_net",
            nameKey = "ITEM_MASTER_NET",
            type = ItemType.CatchingNet,
            price = 5000,
            stock = 1,
            isSpecial = true
        },
        new TradeItem {
            id = "stealth_pro",
            nameKey = "ITEM_STEALTH_PRO",
            type = ItemType.StealthDevice,
            price = 8000,
            stock = 1,
            isSpecial = true
        },
        new TradeItem {
            id = "legendary_treat",
            nameKey = "ITEM_LEGENDARY_TREAT",
            type = ItemType.DogTreat,
            price = 3000,
            stock = 1,
            isSpecial = true
        }
    };

    private List<TradeItem> currentRegularStock = new List<TradeItem>();
    private List<TradeItem> currentSpecialStock = new List<TradeItem>();
    private float nextRestockTime;

    void Start()
    {
        RestockShop();
    }

    void Update()
    {
        if (Time.time >= nextRestockTime)
        {
            RestockShop();
        }
    }

    void RestockShop()
    {
        currentRegularStock.Clear();
        currentSpecialStock.Clear();

        // Add random regular items
        List<TradeItem> availableRegular = new List<TradeItem>(regularItems);
        for (int i = 0; i < maxRegularItems && availableRegular.Count > 0; i++)
        {
            int index = Random.Range(0, availableRegular.Count);
            currentRegularStock.Add(availableRegular[index]);
            availableRegular.RemoveAt(index);
        }

        // Add random special items
        List<TradeItem> availableSpecial = new List<TradeItem>(specialItems);
        for (int i = 0; i < maxSpecialItems && availableSpecial.Count > 0; i++)
        {
            int index = Random.Range(0, availableSpecial.Count);
            currentSpecialStock.Add(availableSpecial[index]);
            availableSpecial.RemoveAt(index);
        }

        nextRestockTime = Time.time + restockTime;
        UpdateShopDisplay();
    }

    void UpdateShopDisplay()
    {
        // Update UI to show current stock
        UIManager.Instance.UpdateShopItems(currentRegularStock, currentSpecialStock);
    }

    public bool PurchaseItem(string itemId)
    {
        TradeItem item = FindItem(itemId);
        if (item != null && item.stock > 0)
        {
            if (GameManager.Instance.SpendCoins(item.price))
            {
                item.stock--;
                GameManager.Instance.AddInventoryItem(itemId);
                
                if (item.stock <= 0)
                {
                    if (item.isSpecial)
                        currentSpecialStock.Remove(item);
                    else
                        currentRegularStock.Remove(item);
                }

                UpdateShopDisplay();
                UIManager.Instance.ShowMessage("ITEM_PURCHASED");
                return true;
            }
            else
            {
                UIManager.Instance.ShowMessage("NOT_ENOUGH_MONEY");
            }
        }
        return false;
    }

    TradeItem FindItem(string itemId)
    {
        TradeItem item = currentRegularStock.Find(i => i.id == itemId);
        if (item == null)
        {
            item = currentSpecialStock.Find(i => i.id == itemId);
        }
        return item;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ShowMessage("PRESS_E_TO_TRADE");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.HideMessage();
        }
    }
}
