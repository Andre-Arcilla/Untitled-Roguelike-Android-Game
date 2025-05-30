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

    private void OnEnable()
    {
        bool hasCleared = false;

        if (currentTown != null)
        {
            string townName = currentTown.townName;
            var entry = PlayerDataHolder.Instance.partyClearedTowns.Find(e => e.townName == townName);

            if (entry != null)
            {
                hasCleared = entry.hasCleared;
                Debug.Log(hasCleared);
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
