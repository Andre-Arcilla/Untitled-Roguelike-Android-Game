using SerializeReferenceEditor;
using System.Collections.Generic;
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

    [Tooltip("Price of the equipment")]
    public int price;

    [Tooltip("The type of equipment: Weapon, Armor, Accessory, etc.")]
    public EquipmentType slotType;

    [Tooltip("The required class to use this equipment")]
    public EquipmentClass classType;

    [Header("Stat Bonuses")]
    [Tooltip("Bonus HP granted by the equipment.")]
    public int bonusHP;

    [Tooltip("Bonus EN granted by the equipment.")]
    public int bonusEN;

    [Tooltip("Bonus PWR granted by the equipment.")]
    public int bonusPWR;

    [Tooltip("Bonus SPD granted by the equipment.")]
    public int bonusSPD;

    [Header("Cards Provided")]
    [Tooltip("Additional cards gained when equipment is used")]
    public List<CardDataSO> cards;

    [Header("Effects Provided")]
    [Tooltip("Effects provided when equipment is used")]
    [SerializeReference, SR]
    public List<IStatusEffect> effects;

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

public enum EquipmentClass
{
    Any,
    Fire_Mage,
    Earth_Mage,
    Water_Mage,
    Barrier_Mage,
    Sword_God_Style,
    North_God_Style,
    Water_God_Style
}