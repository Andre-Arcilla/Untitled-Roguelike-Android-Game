using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "EnemyData")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public int HP;
    public int PWR;
    public int SPD;
}
