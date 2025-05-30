using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentInfo : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] private Canvas canvas => GetComponentInParent<Canvas>();

    [SerializeField] public EquipmentDataSO equipment { get; private set; }
    [SerializeField] private Image image;

    public void Setup(EquipmentDataSO newEquipment)
    {
        equipment = newEquipment;
        image.sprite = newEquipment.sprite;
    }


    //drag logic
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.equipmentDisplay.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        InventoryManager.Instance.SendEquipmentInfo(equipment);
        parentAfterDrag = transform.parent;
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        parentAfterDrag.GetComponent<InventorySlot>().ForceFilledIcon();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mouseScreenPos = eventData.position;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        transform.position = mouseWorldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.equipmentDisplay.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        parentAfterDrag.GetComponent<InventorySlot>().ForceEmptyIcon();
        PartyMenuManager.Instance.UpdateCharacterItems();
    }
}
