using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ItemsPanelManager : MonoBehaviour
{
    [SerializeField] private List<SellableItemData> sellableItemDatas = new List<SellableItemData>();
    [SerializeField] private GameObject itemCellPrefab;
    [SerializeField] private Transform itemsView;
    [SerializeField] private SkinTryoutCabinet skinTryoutCabinet;

    public Transform ItemsView => itemsView;

    void Start()
    {
        SortItems();
        ItemCellCreate();
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            CloseItemShop();
        }
    }

    void SortItems()
    {
        sellableItemDatas.Sort((a, b) => a.ranking.CompareTo(b.ranking));
    }

    void ItemCellCreate()
    {
        foreach (var itemData in sellableItemDatas)
        {
            GameObject cellObject = Instantiate(itemCellPrefab, itemsView);
            ItemCell itemCell = cellObject.GetComponent<ItemCell>();
            itemCell.Initialize(itemData, skinTryoutCabinet);
        }
    }

    public List<SellableItemData> GetSellableItems()
    {
        return sellableItemDatas;
    }

    public void CloseItemShop()
    {
        SceneManager.LoadScene(1);
    }
}