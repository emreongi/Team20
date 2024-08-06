using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Interactor))]

public class InventoryManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string itemTagName = "Item";
    [SerializeField] private int slotNumber = 12;
    [SerializeField] private int radius = 200;
    [SerializeField] private InventoryData inventoryData;
    private int currentSlotIndex = 0;
    private float angleStep;
    [SerializeField] private string inventoryPanelName = "InventoryPanel";
    private RectTransform inventoryPanel;
    private List<GameObject> slots = new List<GameObject>();
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private string spawnPointName = "ItemSpawn";
    private Transform spawnPoint;
    [SerializeField] private Transform itemDropPoint;
    private GameObject equippedItem = null;
    private GameObject previouslySelectedItem = null; // Önceki seçili öğeyi tutmak için
    private SuccessManager successManager;

    private Interactor interactor;

    void Start()
    {
        if (!photonView.IsMine) return;

        successManager = GameObject.Find("GameManager").GetComponent<SuccessManager>();

        interactor = GetComponent<Interactor>();

        inventoryData.ResetInventoryData();

        InitializeInventoryPanel();
        InitializeSpawnPoint();
        ValidateInputs();

        slotNumber = inventoryData.slots.Count;
        angleStep = 360f / slotNumber;

        CreateSlots();
        SelectItem();
        UpdateUISlots();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        HandleScrollInput();
        HandleItemDeletion();
        HandleItemDrop();
        HandleMultiItemDrop();
    }

    private void InitializeInventoryPanel()
    {
        GameObject inventoryPanelObject = GameObject.Find(inventoryPanelName);
        if (inventoryPanelObject == null)
        {
            Debug.LogError("Inventory Panel object not found.");
            return;
        }

        inventoryPanel = inventoryPanelObject.GetComponent<RectTransform>();
    }

    private void InitializeSpawnPoint()
    {
        GameObject spawnPointObject = GameObject.Find(spawnPointName);
        if (spawnPointObject == null)
        {
            Debug.LogError("Spawn Point object not found.");
            return;
        }

        spawnPoint = spawnPointObject.transform;
    }

    private void ValidateInputs()
    {
        if (inventoryData == null)
        {
            Debug.LogError("InventoryData is not assigned.");
        }

        if (slotNumber <= 0 || radius <= 0)
        {
            Debug.LogError("Number of slots/cells and Radius must be greater than 0.");
        }

        if (inventoryPanel == null)
        {
            Debug.LogError($"{inventoryPanelName} object not found.");
        }

        if (spawnPoint == null)
        {
            Debug.LogError($"{spawnPointName} object not found.");
        }
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            RotateInventory(scroll);
        }
    }

    private void HandleItemDeletion()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteItem(currentSlotIndex);
        }
    }

    private void HandleItemDrop()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DropItem(currentSlotIndex, itemDropPoint.position);
        }
    }

    private void HandleMultiItemDrop()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < inventoryData.slots.Count; i++)
            {
                MultiDropItem(i, itemDropPoint.position);
            }
        }
    }

    private void RotateInventory(float scroll)
    {
        currentSlotIndex = (currentSlotIndex + (scroll > 0 ? 1 : -1) + slotNumber) % slotNumber;
        float targetAngle = currentSlotIndex * angleStep;
        inventoryPanel.transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        foreach (GameObject slot in slots)
        {
            slot.transform.localRotation = Quaternion.Euler(0, 0, -targetAngle);
        }

        SelectItem();
    }

    private void CreateSlots()
    {
        for (int i = 0; i < slotNumber; i++)
        {
            float angle = -i * angleStep + 90;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            GameObject newSlot = Instantiate(slotPrefab, inventoryPanel.transform.position, Quaternion.identity);
            newSlot.name = $"{i}.slot";
            newSlot.transform.SetParent(inventoryPanel, false);
            newSlot.transform.localPosition = new Vector3(x, y, 0);
            newSlot.transform.localScale = Vector3.one;

            UISlot uISlot = newSlot.GetComponent<UISlot>();
            uISlot.itemIcon.enabled = false;

            slots.Add(newSlot);
        }

        SelectItem();
    }

    private void UpdateUISlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            UISlot uiSlot = slots[i].GetComponent<UISlot>();

            if (inventoryData.slots[i].itemCount > 0)
            {
                uiSlot.itemIcon.enabled = true;
                uiSlot.itemIcon.sprite = inventoryData.slots[i].itemData.icon;
                uiSlot.itemName.text = inventoryData.slots[i].itemData.name;

                if (inventoryData.slots[i].itemData.stackable)
                {
                    uiSlot.itemCount.text = inventoryData.slots[i].itemCount.ToString();
                }
                else
                {
                    uiSlot.itemCount.text = "";
                }
            }
            else
            {
                uiSlot.itemIcon.enabled = false;
                uiSlot.itemCount.text = "";
                uiSlot.itemName.text = "";
            }
        }
    }

    private void DeleteItem(int index)
    {
        inventoryData.DestroyItem(index);
        UpdateUISlots();
    }

    private void DropItem(int index, Vector3 position)
    {
        inventoryData.DropItem(index, position, photonView.Owner);
        UpdateUISlots();
        DestroyEquippedItem();
    }

    public void MultiDropItem(int index, Vector3 position)
    {
        inventoryData.MultiDropItem(index, position, photonView.Owner);
        UpdateUISlots();
        DestroyEquippedItem();
    }

    private void SelectItem()
    {
        if (photonView.IsMine)
        {
            DeselectItem(); // Önceki öğeyi deselect et

            GameObject selectedItem = inventoryData.SelectItem(currentSlotIndex, spawnPoint);
            if (selectedItem != null)
            {
                photonView.RPC("RPC_SetEquippedItem", RpcTarget.All, selectedItem.GetPhotonView().ViewID);
                previouslySelectedItem = selectedItem; // Yeni seçili öğeyi kaydet
            }
        }
    }

    private void DeselectItem()
    {
        if (previouslySelectedItem != null)
        {
            PhotonView photonView = previouslySelectedItem.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                PhotonNetwork.Destroy(previouslySelectedItem);
            }

            previouslySelectedItem = null; // Önceki seçili öğeyi null yap
        }
    }

[PunRPC]
private void RPC_SetEquippedItem(int viewID)
{
    if (equippedItem != null)
    {
        Destroy(equippedItem);
    }

    PhotonView view = PhotonView.Find(viewID);
    if (view != null)
    {
        GameObject itemObject = view.gameObject;
        if (itemObject != null)
        {
            equippedItem = itemObject;
            equippedItem.transform.SetParent(spawnPoint);
            equippedItem.transform.localPosition = Vector3.zero;
            equippedItem.transform.localRotation = Quaternion.identity;
            equippedItem.transform.localScale = Vector3.one;

            // Sadece yerel oyuncu kontrol edebilir
            if (view.IsMine)
            {
                PhotonTransformView photonTransformView = equippedItem.GetComponent<PhotonTransformView>();
                if (photonTransformView != null)
                {
                    photonTransformView.m_SynchronizePosition = true;
                    photonTransformView.m_SynchronizeRotation = true;
                }
            }
        }
    }
}


    private void OnTriggerEnter(Collider collider)
    {
        if (!photonView.IsMine) return;

        if (collider.gameObject.CompareTag(itemTagName) && collider.gameObject.name != "Health(Clone)" && interactor.interactionRequest)
        {
            Item item = collider.gameObject.GetComponent<Item>();
            if (item != null)
            {
                if (inventoryData.AddItem(item.itemData))
                {
                    photonView.RPC("RPC_DestroyItem", RpcTarget.All, collider.gameObject.GetPhotonView().ViewID);
                    UpdateUISlots();
                }
            }
        }

        if (collider.gameObject.name == "CrystalFragmentable(Clone)"  && interactor.interactionRequest)
        {
            Item item = collider.gameObject.GetComponent<Item>();
            if (item != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (inventoryData.AddItem(item.itemData))
                    {
                        UpdateUISlots();
                    }
                }

                photonView.RPC("RPC_DestroyItem", RpcTarget.All, collider.gameObject.GetPhotonView().ViewID);
            }
        }

/*
        if (collider.gameObject.name == "Coin(Clone)")
        {
            Item item = collider.gameObject.GetComponent<Item>();
            if (item != null)
            {
                if (inventoryData.AddItem(item.itemData))
                {
                    photonView.RPC("RPC_DestroyItem", RpcTarget.All, collider.gameObject.GetPhotonView().ViewID);
                    successManager.UpdateCoin();
                    UpdateUISlots();
                }
            }
        }

        if (collider.gameObject.name == "MoaiStatue(Clone)")
        {
            Item item = collider.gameObject.GetComponent<Item>();
            if (item != null)
            {
                if (inventoryData.AddItem(item.itemData))
                {
                    photonView.RPC("RPC_DestroyItem", RpcTarget.All, collider.gameObject.GetPhotonView().ViewID);
                    successManager.UpdateMoaiStatue();
                    UpdateUISlots();
                }
            }
        }

        if (collider.gameObject.name == "RS1(Clone)")
        {
            Item item = collider.gameObject.GetComponent<Item>();
            if (item != null)
            {
                if (inventoryData.AddItem(item.itemData))
                {
                    photonView.RPC("RPC_DestroyItem", RpcTarget.All, collider.gameObject.GetPhotonView().ViewID);
                    successManager.UpdateRS1Robot();
                    UpdateUISlots();
                }
            }
        }
*/
    }

    [PunRPC]
    private void RPC_DestroyItem(int viewID)
    {
        GameObject itemObject = PhotonView.Find(viewID).gameObject;
        if (itemObject != null)
        {
            PhotonNetwork.Destroy(itemObject);
        }
    }

    private void DestroyEquippedItem()
    {
        if (equippedItem != null)
        {
            PhotonNetwork.Destroy(equippedItem);
            equippedItem = null;
        }
    }
}