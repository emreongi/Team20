using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(JsonManager))]

public class EconomyManager : MonoBehaviour
{
    private JsonManager jsonManager;
    private PlayerData playerData;
    private List<int> purchasedIdItems = new List<int>(); // Satın alınan itemlerin listesi.
    private int moneyCount = 0;
    [HideInInspector] public int itemMoneyCount;
    public int selectItemId;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text selectText;

    void Start()
    {
        jsonManager = GetComponent<JsonManager>();
        GetMoneyCount();
    }

    void GetMoneyCount()
    {
        playerData = jsonManager.ReadJson();
        moneyCount = playerData.playerMoney;
        moneyText.text = moneyCount.ToString();

        purchasedIdItems = playerData.purchasedIdItems;
    }

    void SetMoneyCount()
    {
        playerData.playerMoney = moneyCount;
        jsonManager.UpdateJson(playerData);
        moneyText.text = moneyCount.ToString();
    }

    public void SetItemSelect()
    {
        if (purchasedIdItems.Contains(selectItemId))
        {
            selectText.text = "Use";
        }
        else
        {
            selectText.text = "Buy";
        }
    }

    public void ItemBuy()
    {
        if (!purchasedIdItems.Contains(selectItemId) && moneyCount >= itemMoneyCount)
        {
            moneyCount -= itemMoneyCount;
            purchasedIdItems.Add(selectItemId);

            // Satılan itemi satılan itemlerin arasına ekliyoruz.
            playerData.purchasedIdItems = purchasedIdItems;
            SetMoneyCount();

            // SellableItemData'daki isPurchasing değerini true yap
            SellableItemData purchasedItem = FindItemById(selectItemId);
            if (purchasedItem != null)
            {
                purchasedItem.isPurchasing = true;

                // İlgili ItemCell'i bul ve güncelle
                UpdateItemCell(purchasedItem.id);
            }

            // JSON'a güncellemeyi yaz
            jsonManager.UpdateJson(playerData);

            selectText.text = "Use";
        }
        else if (moneyCount < itemMoneyCount)
        {
            Debug.Log("Not enough money to buy this item.");
        }
        else
        {
            Debug.Log("Item already purchased.");
        }
    }

    private SellableItemData FindItemById(int id)
    {
        // SellableItemData listesini arayarak ilgili itemi bul
        ItemsPanelManager itemsPanelManager = FindObjectOfType<ItemsPanelManager>();
        if (itemsPanelManager != null)
        {
            foreach (SellableItemData item in itemsPanelManager.GetSellableItems())
            {
                if (item.id == id)
                {
                    return item;
                }
            }
        }
        return null;
    }

    private void UpdateItemCell(int id)
    {
        ItemsPanelManager itemsPanelManager = FindObjectOfType<ItemsPanelManager>();
        if (itemsPanelManager != null)
        {
            foreach (Transform child in itemsPanelManager.ItemsView)
            {
                ItemCell itemCell = child.GetComponent<ItemCell>();
                if (itemCell != null && itemCell.itemData.id == id)
                {
                    itemCell.UpdateCell();
                    break;
                }
            }
        }
    }
}