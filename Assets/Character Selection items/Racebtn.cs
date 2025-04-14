using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Racebtn : MonoBehaviour
{
    [SerializeField] private string race;
    [SerializeField] private int HP, EN, PWR, SPD;
    [SerializeField] private Button btn;
    [SerializeField] private StatManager statsManager;

    public string RaceName => race;

    public void SendStats()
    {
        statsManager.UpdateStats(race, HP, EN, PWR, SPD);
    }

    public void SetInteractable(bool value)
    {
        btn.interactable = value;
    }
}
