using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private int amountOfItems;
    [SerializeField] private EquipmentDatabase equipmentDatabase;
    [SerializeField] private List<EquipmentDataSO> equipmentList;
    [SerializeField] private GameObject listObjectPrefab;
    [SerializeField] private Transform listParent;
    [SerializeField] private SelectedStoreItem itemDisplay;

    private void Start()
    {
        GenerateRandomItems();
        GenerateItemsList();
        SendFirstItemData();
    }

    //generate random list of items
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
    private void GenerateItemsList()
    {
        foreach (EquipmentDataSO item in equipmentList)
        {
            GameObject listObject = Instantiate(listObjectPrefab, listParent);
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
        listParent.GetChild(0).GetComponent<StoreItemInfo>().SendEquipmentData();
    }

    public void RemoveItemFromList(GameObject listObject)
    {
        Destroy(listObject);
        SendFirstItemData();
    }
}
