using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
    }

    //performers

    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        Debug.Log("enemy turn");
        yield return new WaitForSeconds(2f);
        Debug.Log("turn end");
    }
}
