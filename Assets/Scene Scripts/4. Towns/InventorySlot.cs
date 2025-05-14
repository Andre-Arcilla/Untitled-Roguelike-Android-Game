using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private EquipmentType slot;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        EquipmentInfo draggableItem = droppedItem.GetComponent<EquipmentInfo>();
        if (draggableItem == null) return;

        if (transform.childCount != 0)
        {
            GameObject currentItem = transform.GetChild(0).gameObject;
            EquipmentInfo currentDraggable = currentItem.GetComponent<EquipmentInfo>();

            if (currentDraggable != null)
                currentDraggable.transform.SetParent(draggableItem.parentAfterDrag);
        }

        draggableItem.parentAfterDrag = transform;
    }

}
