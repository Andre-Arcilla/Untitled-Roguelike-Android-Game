using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaceDatabase", menuName = "Database/Race Database")]
public class RaceDatabase : ScriptableObject
{
    public List<RaceDataSO> allRaces;
}

