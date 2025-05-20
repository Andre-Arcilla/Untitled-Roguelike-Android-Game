using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] public InventorySlot[] inventorySlots;
    [SerializeField] public InventorySlot[] equipmentSlots;
    [SerializeField] public GameObject equipmentPrefab;
    [SerializeField] public Transform trashcan;

    public bool AddItem(EquipmentDataSO equipment)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            EquipmentInfo itemInSlot = slot.GetComponentInChildren<EquipmentInfo>();

            if (itemInSlot == null)
            {
                SpawnItem(equipment, slot);
                return true;
            }
        }
        return false;
    }

    public void AddEquippedItem(EquipmentDataSO equipment)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            InventorySlot slot = equipmentSlots[i];
            EquipmentInfo itemInSlot = slot.GetComponentInChildren<EquipmentInfo>();

            if (itemInSlot == null && slot.allowedSlotType == equipment.slotType)
            {
                SpawnItem(equipment, slot);
                return;
            }
        }
    }

    private void SpawnItem(EquipmentDataSO equipment, InventorySlot slot)
    {
        GameObject newEquipment = Instantiate(equipmentPrefab, slot.transform);
        newEquipment.GetComponent<EquipmentInfo>().Setup(equipment);
        newEquipment.name = equipment.equipmentName;
    }

    public bool RemoveItem(EquipmentDataSO equipment)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            EquipmentInfo itemInSlot = slot.GetComponentInChildren<EquipmentInfo>();

            if (itemInSlot != null && itemInSlot.equipment == equipment)
            {
                PlayerDataHolder.Instance.partyInventory.Remove(itemInSlot.equipment.equipmentName);
                itemInSlot.transform.SetParent(trashcan);
                Destroy(itemInSlot.gameObject);
                return true;
            }
        }
        return false;
    }

    public void ClearEquipmentSlots()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].transform.childCount > 0)
            {
                Transform child = equipmentSlots[i].transform.GetChild(0);
                child.SetParent(trashcan); // Detach from UI
                Destroy(child.gameObject); // Still destroys at end of frame
            }
            equipmentSlots[i].ForceFilledIcon();
        }
    }
}
