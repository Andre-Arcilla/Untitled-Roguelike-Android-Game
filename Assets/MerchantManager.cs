using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MerchantManager : MonoBehaviour
{
    public static MerchantManager Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only one allowed
            return;
        }

        Instance = this;
    }

    [SerializeField] private TMP_Text switchTxt;
    [SerializeField] public GameObject buyPanel;
    [SerializeField] private GameObject buyButton;
    [SerializeField] public GameObject sellPanel;
    [SerializeField] private GameObject sellButton;
    [SerializeField] private int amountOfItems;
    [SerializeField] private EquipmentDatabase equipmentDatabase;
    [SerializeField] private List<EquipmentDataSO> equipmentList;
    [SerializeField] private GameObject listObjectPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private SelectedStoreItem itemDisplay;

    private void Start()
    {
        parent = buyPanel.transform.Find("List Mask/List Holder").transform;

        GenerateBuyPanel();
        GenerateInventoryList();
        SendFirstItemData();
    }

    private void GenerateBuyPanel()
    {
        GenerateRandomItems();
        GenerateItemsList(equipmentList, buyPanel.transform.Find("List Mask/List Holder").transform);
    }

    private void GenerateSellPanel()
    {
        GenerateInventoryList();
    }

    //generate random list of items for buy panel
    private void GenerateRandomItems()
    {
        int amount = Random.Range(5, amountOfItems);

        List<EquipmentDataSO> itemPool = new List<EquipmentDataSO>(equipmentDatabase.allEquipments);

        for (int i = 0; i < amountOfItems; i++)
        {
            int j = Random.Range(0, itemPool.Count);
            equipmentList.Add(itemPool[j]);
            itemPool.RemoveAt(j);
        }
    }

    //generate items to display
    private void GenerateItemsList(List<EquipmentDataSO> list, Transform parent)
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        foreach (EquipmentDataSO item in list)
        {
            GameObject listObject = Instantiate(listObjectPrefab, parent);
            listObject.GetComponentInChildren<StoreItemInfo>().Setup(item);
        }
    }

    //send selected item's data to display
    public void SendEquipmentData(EquipmentDataSO equipment, GameObject listObject)
    {
        itemDisplay.Setup(equipment, listObject);
    }

    //automatically send 1st item's data
    private void SendFirstItemData()
    {
        if (parent != null && parent.childCount > 0)
        {
            parent.GetChild(0).GetComponent<StoreItemInfo>().SendEquipmentData();
            itemDisplay.gameObject.SetActive(true);
        }
        else
        {
            itemDisplay.gameObject.SetActive(false);
        }
    }

    //removes an item from the list
    public void RemoveItemFromList(GameObject listObject)
    {
        listObject.transform.SetParent(InventoryManager.Instance.trashcan);
        Destroy(listObject);
        SendFirstItemData();
    }

    //generates a List<EquipmentDataSO> of inventory items
    public void GenerateInventoryList()
    {
        List<EquipmentDataSO> inventoryItems = new List<EquipmentDataSO>();

        foreach (string itemString in PlayerDataHolder.Instance.partyInventory)
        {
            EquipmentDataSO selectedEquipment = equipmentDatabase.allEquipments.Find(e => e.equipmentName == itemString);

            if (selectedEquipment == null)
            {
                Debug.LogWarning($"No match found for: {itemString}");
                continue;
            }
            inventoryItems.Add(selectedEquipment);
        }

        GenerateItemsList(inventoryItems, sellPanel.transform.Find("List Mask/List Holder").transform);
    }
    
    //switches between buy and sell panels
    public void SwitchPanelAction()
    {
        if (buyPanel.activeSelf)
        {
            buyPanel.SetActive(false);
            buyButton.SetActive(false);

            sellPanel.SetActive(true);
            sellButton.SetActive(true);

            switchTxt.text = "BUY\nITEMS";
            parent = sellPanel.transform.Find("List Mask/List Holder").transform;
            SendFirstItemData();
        }
        else if (sellPanel.activeSelf)
        {
            sellPanel.SetActive(false);
            sellButton.SetActive(false);

            buyPanel.SetActive(true);
            buyButton.SetActive(true);

            switchTxt.text = "SELL\nITEMS";
            parent = buyPanel.transform.Find("List Mask/List Holder").transform;
            SendFirstItemData();
        }
    }
}
