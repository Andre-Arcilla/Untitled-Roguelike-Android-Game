using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewRace", menuName = "RaceData")]
public class RaceDataSO : ScriptableObject
{
    [Header("Race Information")]
    [Tooltip("Name of the race")]
    [FormerlySerializedAs("RaceName")]
    public string raceName;

    [Tooltip("Short description or lore about the race")]
    public string description;

    [Header("Base Stats")]
    [Tooltip("Base health points of this race. 1 point equals 5 HP")]
    [Range(1, 99)]
    public int HP = 1;

    [Tooltip("Base energy used for playing cards; regenerates each turn")]
    [Range(1, 99)]
    public int EN = 1;

    [Tooltip("Power modifier for card effectiveness; 10 is a 1x multiplier")]
    [Range(1, 99)]
    public int PWR = 1;

    [Tooltip("Determines turn order speed; higher goes first")]
    [Range(1, 99)]
    public int SPD = 1;
}