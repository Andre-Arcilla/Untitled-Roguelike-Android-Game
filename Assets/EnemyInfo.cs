using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] public EnemySO enemyInfo;
    [SerializeField] public int totalHP;
    [SerializeField] public int totalPWR;
    [SerializeField] public int totalSPD;

    private void Start()
    {
        totalHP = enemyInfo.HP * 5;
        totalPWR = enemyInfo.PWR / 5;
        totalSPD = enemyInfo.SPD;
    }
}
