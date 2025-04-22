using UnityEngine;

public class TownManager : MonoBehaviour
{
    public static TownManager Instance;
    public TownDataSO currentTown;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}