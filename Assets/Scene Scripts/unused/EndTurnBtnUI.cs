using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EndTurnBtnUI : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("ssssss");
        EnemyTurnGA enemyTurnGA = new();
        Debug.Log("gfgsg");
        ActionSystem.Instance.Perform(enemyTurnGA);
        Debug.Log("sdasdas");
    }
}
