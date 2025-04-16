using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardData")]
public class CardDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;
    [TextArea] public string description;
    public Sprite cardSprite;

    [Header("Gameplay Stats")]
    public int cost;
    public int power;
}
