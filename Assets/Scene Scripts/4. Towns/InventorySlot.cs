using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] public EquipmentType allowedSlotType;
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite emptySlotSprite;
    [SerializeField] private Sprite filledSlotSprite;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        EquipmentInfo draggedEquipment = draggedObject.GetComponent<EquipmentInfo>();
        if (draggedEquipment == null) return;

        // Check if dragged item is allowed in this slot
        if (draggedEquipment.equipment.slotType != allowedSlotType && allowedSlotType != EquipmentType.None) return;

        // Check if this slot already has an item
        if (transform.childCount > 0)
        {
            GameObject existingObject = transform.GetChild(0).gameObject;
            EquipmentInfo existingEquipment = existingObject.GetComponent<EquipmentInfo>();

            if (existingEquipment != null)
            {
                // Get the InventorySlot where the dragged item came from
                InventorySlot originalSlot = draggedEquipment.parentAfterDrag.GetComponent<InventorySlot>();

                // Check if the existing item can be moved to the dragged item's original slot
                if (originalSlot == null && existingEquipment.equipment.slotType == originalSlot.allowedSlotType && allowedSlotType != EquipmentType.None)
                {
                    return;
                }
                else
                {
                    existingEquipment.transform.SetParent(originalSlot.transform);
                }
            }
        }

        draggedEquipment.parentAfterDrag = transform;
        slotImage.sprite = emptySlotSprite;
    }

    public void ForceEmptyIcon()
    {
        slotImage.sprite = emptySlotSprite;
    }

    public void ForceFilledIcon()
    {
        slotImage.sprite = filledSlotSprite;
    }
}