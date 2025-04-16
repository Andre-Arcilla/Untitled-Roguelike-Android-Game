using UnityEngine;

[CreateAssetMenu]
public class RaceDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string RaceName;
    public string description;

    [Header("Stats")]

    [Range(1, 99)]
    [Tooltip("hit points of the unit")]
    public int HP = 1;

    [Range(1, 99)]
    [Tooltip("energy to use cards, recovers each turn")]
    public int EN = 1;

    [Range(1, 99)]
    [Tooltip("modifier that determines a card's strength, 10 is 1x modifier (card damage/power * PWR)")]
    public int PWR = 1;

    [Range(1, 99)]
    [Tooltip("determines who goes first in a turn")]
    public int SPD = 1;
}