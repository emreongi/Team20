using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemPriceText;
    [SerializeField] private Image itemImage;

    [HideInInspector] public SellableItemData itemData;
    private SkinTryoutCabinet skinTryoutCabinet;
    private EconomyManager economyManager;
    private int itemMoneyCount;

    void Start()
    {
        economyManager = GameObject.Find("EconomyManager").GetComponent<EconomyManager>();
    }

    public void Initialize(SellableItemData data, SkinTryoutCabinet cabinet)
    {
        itemData = data;
        skinTryoutCabinet = cabinet;
        UpdateCell();
    }

    public void UpdateCell()
    {
        itemNameText.text = itemData.name;
        itemPriceText.text = itemData.isPurchasing ? "Taken" : itemData.itemPrice.ToString();
        itemMoneyCount = itemData.itemPrice;
        itemImage.sprite = itemData.itemImage;
    }

    public void OnItemClick()
    {
        skinTryoutCabinet.SetMaterial(itemData.itemMaterial);
        economyManager.selectItemId = itemData.id;
        economyManager.itemMoneyCount = itemMoneyCount;
        economyManager.SetItemSelect();
    }
}