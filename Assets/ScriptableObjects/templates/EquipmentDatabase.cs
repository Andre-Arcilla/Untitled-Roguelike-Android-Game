using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "Database/Equipment Database")]
public class EquipmentDatabase : ScriptableObject
{
    public List<EquipmentDataSO> allEquipments;
}
