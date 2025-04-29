using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Database/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    public List<CharacterDataSO> allEnemies;
}


