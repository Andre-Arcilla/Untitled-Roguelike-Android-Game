using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemInfo : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemPrice;
    [SerializeField] private EquipmentDataSO equipment;

    public void Setup(EquipmentDataSO newEquipment)
    {
        equipment = newEquipment;
        itemImage.sprite = newEquipment.sprite;
        itemName.text = newEquipment.equipmentName;
        itemPrice.text = $"{newEquipment.price.ToString()}g";
    }

    public void SendEquipmentData()
    {
        MerchantManager.Instance.SendEquipmentData(equipment, this.gameObject);
    }
}
