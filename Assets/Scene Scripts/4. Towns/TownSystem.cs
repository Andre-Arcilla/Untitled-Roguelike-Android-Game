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
    [SerializeField] public Button labyrinthEntrance;

    private void OnEnable()
    {
        if (currentTown.labyrinthCleared)
        {
            labyrinthEntrance.transform.GetChild(0).gameObject.SetActive(false);
            labyrinthEntrance.interactable = false;
        }
        else
        {
            labyrinthEntrance.transform.GetChild(0).gameObject.SetActive(true);
            labyrinthEntrance.interactable = true;
        }
    }
}
