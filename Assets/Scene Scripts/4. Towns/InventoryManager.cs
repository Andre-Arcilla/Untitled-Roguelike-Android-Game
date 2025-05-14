using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] public GameObject equipmentPrefab;

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

    private void SpawnItem(EquipmentDataSO equipment, InventorySlot slot)
    {
        GameObject newEquipment = Instantiate(equipmentPrefab, slot.transform);
        newEquipment.GetComponent<EquipmentInfo>().Setup(equipment);
    }
}
