using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TownData : MonoBehaviour
{
    [SerializeField] private TownDataSO town;
    [SerializeField] private TMP_Text townText;

    private void Start()
    {
        town = TownManager.Instance.currentTown;
        townText.text = town.name;
    }
}
