using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownSystem : MonoBehaviour
{
    public static TownSystem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] public TownDataSO currentTown;
}
