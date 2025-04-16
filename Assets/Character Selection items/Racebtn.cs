using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Racebtn : MonoBehaviour
{
    [SerializeField] private RaceDataSO raceData; // Reference to the RaceDataSO
    [SerializeField] private Button btn;
    [SerializeField] private StatManager statsManager;

    public string RaceName => raceData.RaceName; // Get the RaceName from the RaceDataSO

    // Use the RaceDataSO to send stats to the StatManager
    public void SendStats()
    {
        statsManager.UpdateStats(raceData.RaceName, raceData.HP, raceData.EN, raceData.PWR, raceData.SPD);
    }

    // Set the button interactable based on the parameter
    public void SetInteractable(bool value)
    {
        btn.interactable = value;
    }
}