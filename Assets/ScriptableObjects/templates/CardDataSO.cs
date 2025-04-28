using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardData")]
public class CardDataSO : ScriptableObject
{
    [Header("Card Information")]
    [Tooltip("Name of the card")]
    public string cardName;

    [Tooltip("Flavor text or description of the card")]
    [TextArea]
    public string description;

    [Tooltip("Who this card targets when played")]
    public Target target;

    [Header("Gameplay Stats")]
    [Tooltip("Cost to play this card (e.g., energy points required)")]
    [Range(0, 99)]
    public int cost = 0;

    [Tooltip("The power of the card, affecting damage or effect strength")]
    [Range(0, 99)]
    public int power = 0;

    [Header("Sprite")]
    [Tooltip("Artwork or image representing the card")]
    public Sprite cardSprite;
}

public enum Target
{
    Ally,
    Enemy
}
