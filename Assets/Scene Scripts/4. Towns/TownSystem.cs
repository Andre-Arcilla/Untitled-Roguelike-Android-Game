using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] public GameObject gateOpen;
    [SerializeField] public GameObject gateClosed;
private IEnumerator Start()
{
    // Wait until PlayerDataHolder.Instance is set
    while (PlayerDataHolder.Instance == null)
    {
        yield return null;
    }

    if (!string.IsNullOrEmpty(currentTown?.sceneName))
    {
        PlayerDataHolder.Instance.partyCurrentTown = currentTown.sceneName;
    }

    InitializeTown();
}

    private void InitializeTown()
    {
        bool hasCleared = false;

        if (currentTown != null)
        {
            string townName = currentTown.townName;
            if (PlayerDataHolder.Instance == null)
            {
                Debug.LogError("PlayerDataHolder.Instance is null!");
                return;
            }

            if (PlayerDataHolder.Instance.partyClearedTowns == null)
            {
                Debug.LogError("partyClearedTowns list is null!");
                return;
            }

            var entry = PlayerDataHolder.Instance.partyClearedTowns.Find(e =>
            {
                if (e == null)
                {
                    Debug.LogWarning("Found null entry in partyClearedTowns list.");
                    return false;
                }
                return e.townName == townName;
            });
            if (entry != null)
            {
                hasCleared = entry.hasCleared;
            }
            else
            {
                Debug.LogWarning($"Town '{townName}' not found in clearedTowns list.");
            }
        }

        gateClosed.SetActive(hasCleared);
        gateOpen.SetActive(!hasCleared);
        gateOpen.GetComponent<Button>().interactable = !hasCleared;
    }
}
