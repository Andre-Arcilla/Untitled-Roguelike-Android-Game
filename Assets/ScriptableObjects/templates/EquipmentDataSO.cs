using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "EquipmentData")]
public class EquipmentDataSO : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Name of the equipment.")]
    public string equipmentName;

    [Tooltip("Flavor text")]
    [TextArea]
    public string description;

    [Tooltip("The type of equipment: Weapon, Armor, Accessory, etc.")]
    public EquipmentType slotType;

    [Header("Stat Bonuses")]
    [Tooltip("Bonus HP granted by the equipment.")]
    public int bonusHP;

    [Tooltip("Bonus EN granted by the equipment.")]
    public int bonusEN;

    [Tooltip("Bonus PWR granted by the equipment.")]
    public int bonusPWR;

    [Tooltip("Bonus SPD granted by the equipment.")]
    public int bonusSPD;

    [Header("Other")]
    [Tooltip("Optional sprite/icon for the equipment.")]
    public Sprite sprite;
}

public enum EquipmentType
{
    None,
    Armor,
    Weapon,
    Accessory
}