using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] public int waves;
}