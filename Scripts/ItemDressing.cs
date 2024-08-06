using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JsonManager))]

public class ItemDressing : MonoBehaviour
{
    private JsonManager jsonManager;
    private PlayerData playerData;
    private Renderer gameObjectRenderer;
    [SerializeField] private List<SellableItemData> sellableItemDatas = new List<SellableItemData>();

    void Start()
    {
        jsonManager = GetComponent<JsonManager>();

        gameObjectRenderer = gameObject.GetComponent<Renderer>();

        Dressing();
    }

    void Dressing()
    {
        //
    }
}
