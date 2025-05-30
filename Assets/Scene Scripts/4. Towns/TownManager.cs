using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    public static TownManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (townTo == null)
        {
            townTo = fallBackTown;
        }
    }

    [SerializeField] private TownDataSO fallBackTown;
    [SerializeField] public TownDataSO townFrom;
    [SerializeField] public TownDataSO townTo;
    [SerializeField] public bool isLabyrinth;
    [SerializeField] public int waves = 1;
}