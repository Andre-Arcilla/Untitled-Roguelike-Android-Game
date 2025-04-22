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
    public EquipmentType type;

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
    public Sprite icon;
}

public enum EquipmentType
{
    ChestArmor,     // Armor for the torso (e.g., armor plates, tunics)
    Headgear,       // Head protection (e.g., helmets, hoods)
    Gloves,         // Hand protection (e.g., gauntlets)
    Leggings,       // Armor for the legs (e.g., armored pants)
    Boots,          // Footwear (e.g., boots, shoes)

    MainHand,       // Primary weapon (e.g., sword, staff)
    OffHand,        // Secondary weapon or shield
    Accessory1,     // First accessory (e.g., ring, amulet)
    Accessory2,     // Second accessory (e.g., ring, necklace)
    Accessory3,     // Third accessory (e.g., bracelet, charm)
}