using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObject/InventoryData")]
public class InventoryData : ScriptableObject
{
    [Tooltip("Determines the maximum number of slots/cells that can be in the inventory.")]
    public List<Slot> slots = new List<Slot>();
    public int maxStack = 5;
    private Transform currentSelectedItem;
    public OriginalInventoryData originalInventoryData;

    public void ResetInventoryData()
    {
        slots.Clear();
        foreach (Slot slot in originalInventoryData.slots)
        {
            Slot newSlot = new Slot();
            newSlot.itemData = slot.itemData;
            newSlot.itemCount = slot.itemCount;
            newSlot.isFull = slot.isFull;
            slots.Add(newSlot);
        }
    }

    public void DestroyItem(int index)
    {
        Slot slot = slots[index];
        slot.isFull = false;
        if (slot.itemCount > 0)
        {
            slot.itemCount--;

            if (slot.itemCount == 0)
            {
                slot.itemData = null;
            }
        }
    }

public void DropItem(int index, Vector3 dropPoint, Player player)
{
    Slot slot = slots[index];
    if (slot.itemCount > 0)
    {
        GameObject droppedItem = PhotonNetwork.Instantiate(slot.itemData.itemPrefab.name, dropPoint, Quaternion.identity);
        PhotonView photonView = droppedItem.GetComponent<PhotonView>();
        if (photonView != null)
        {
            // Drop edilen itemin sahipliğini serbest bırak
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
        DestroyItem(index);
    }
}

public void MultiDropItem(int index, Vector3 dropPoint, Player player)
{
    Slot slot = slots[index];
    for (int i = 0; i < slot.itemCount; i++)
    {
        Vector3 randomDropPoint = dropPoint + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        GameObject droppedItem = PhotonNetwork.Instantiate(slot.itemData.itemPrefab.name, randomDropPoint, Quaternion.identity);
        PhotonView photonView = droppedItem.GetComponent<PhotonView>();
        if (photonView != null)
        {
            photonView.TransferOwnership(player);

            // PhotonTransformView bileşenini kontrol et
            PhotonTransformView photonTransformView = droppedItem.GetComponent<PhotonTransformView>();
            if (photonTransformView != null)
            {
                photonTransformView.m_SynchronizePosition = true;
                photonTransformView.m_SynchronizeRotation = true;
            }
        }
    }

    slot.itemCount = 0;
    slot.itemData = null;
    slot.isFull = false;
}

public GameObject SelectItem(int index, Transform parent)
{
    if (index < 0 || index >= slots.Count)
    {
        Debug.LogError("Index out of range.");
        return null;
    }

    Slot slot = slots[index];
    if (slot == null)
    {
        Debug.LogError("Slot is null.");
        return null;
    }

    if (slot.itemData == null)
    {
        Debug.LogError("ItemData is null.");
        return null;
    }

    if (slot.itemData.itemPrefab == null)
    {
        Debug.LogError("ItemPrefab is null.");
        return null;
    }

    if (currentSelectedItem != null)
    {
        PhotonNetwork.Destroy(currentSelectedItem.gameObject);
    }

    GameObject itemObject = PhotonNetwork.Instantiate(slot.itemData.itemPrefab.name, parent.position, Quaternion.identity, 0);
    if (itemObject == null)
    {
        Debug.LogError("Failed to instantiate item.");
        return null;
    }

    itemObject.transform.SetParent(parent);
    itemObject.transform.localPosition = Vector3.zero;
    itemObject.transform.localRotation = Quaternion.identity;
    itemObject.transform.localScale = Vector3.one;

    currentSelectedItem = itemObject.transform;
    PhotonView itemPhotonView = itemObject.GetComponent<PhotonView>();
    if (itemPhotonView != null)
    {
        itemPhotonView.RequestOwnership();
        itemPhotonView.RPC("RPC_SetVisibility", RpcTarget.AllBuffered, true);
    }

    BoxCollider itemCollider = itemObject.GetComponent<BoxCollider>();
    if (itemCollider != null)
    {
        itemCollider.enabled = false;
    }
    else
    {
        Debug.LogError("BoxCollider not found on item.");
    }

    return itemObject;
}

[PunRPC]
private void RPC_SetVisibility(bool isVisible)
{
    if (currentSelectedItem != null)
    {
        currentSelectedItem.gameObject.SetActive(isVisible);
    }
}

    public bool AddItem(ItemData itemData)
    {
        foreach (Slot slot in slots)
        {
            if (slot.itemData == itemData && itemData.stackable && slot.itemCount < maxStack)
            {
                slot.itemCount++;
                slot.isFull = slot.itemCount >= maxStack;
                return true;
            }
            else if (slot.itemCount == 0)
            {
                slot.AddItemToSlot(itemData);
                return true;
            }
        }

        return false;
    }
}

[System.Serializable]
public class Slot
{
    public bool isFull;
    public int itemCount;
    public ItemData itemData;

    public void AddItemToSlot(ItemData itemData)
    {
        this.itemData = itemData;
        itemCount++;
        isFull = !itemData.stackable || itemCount >= 1;
    }
}