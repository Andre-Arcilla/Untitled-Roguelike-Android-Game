using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewParty", menuName = "Party")]
public class PlayerPartySO : ScriptableObject
{
    [Header("Party Members")]
    public CharacterDataSO character1;
    public CharacterDataSO character2;
    public CharacterDataSO character3;
    public CharacterDataSO character4;
}
