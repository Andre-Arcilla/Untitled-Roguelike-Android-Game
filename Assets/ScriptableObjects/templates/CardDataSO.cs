using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardData")]
public class CardDataSO : ScriptableObject
{
    [Header("Card Information")]
    [Tooltip("The name of the card")]
    public string cardName;

    [Tooltip("Flavor text")]
    [TextArea]
    public string description;

    public Sprite cardSprite;

    [Header("Gameplay Stats")]
    [Tooltip("The cost to play this card (e.g., energy points required)")]
    public int cost;

    [Tooltip("The power of the card, affecting damage or effect strength")]
    public int power;
}
