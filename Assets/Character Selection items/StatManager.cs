using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text ENText;
    [SerializeField] private TMP_Text PWRText;
    [SerializeField] private TMP_Text SPDText;
    [SerializeField] private TMP_Text StatText;

    [SerializeField] private Button incHP;
    [SerializeField] private Button decHP;

    [SerializeField] private Button incEN;
    [SerializeField] private Button decEN;

    [SerializeField] private Button incPWR;
    [SerializeField] private Button decPWR;

    [SerializeField] private Button incSPD;
    [SerializeField] private Button decSPD;
    public string raceName { get; private set; }
    private int raceHP;
    private int raceEN;
    private int racePWR;
    private int raceSPD;

    private int allocatedHP;
    private int allocatedEN;
    private int allocatedPWR;
    private int allocatedSPD;

    [SerializeField] private int statPoints;
    [SerializeField] private List<Racebtn> raceButtons;
    [SerializeField] private Racebtn defaultRace;

    private void Start()
    {
        defaultRace.SendStats();
        UpdateText();
    }

    public void UpdateStats(string race, int hp, int en, int pwr, int spd)
    {
        raceName = race;
        raceHP = hp;
        raceEN = en;
        racePWR = pwr;
        raceSPD = spd;

        UpdateText();
        UpdateRaceButtons();
    }

    private void UpdateRaceButtons()
    {
        foreach (var btn in raceButtons)
        {
            btn.SetInteractable(btn.RaceName != raceName);
        }
    }

    private void UpdateText()
    {
        int hp = raceHP + allocatedHP;
        int en = raceEN + allocatedEN;
        int pwr = racePWR + allocatedPWR;
        int spd = raceSPD + allocatedSPD;
        int stats = statPoints - (allocatedHP + allocatedEN + allocatedPWR + allocatedSPD);

        HPText.text = hp.ToString("D2");
        ENText.text = en.ToString("D2");
        PWRText.text = pwr.ToString("D2");
        SPDText.text = spd.ToString("D2");
        StatText.text = stats.ToString();

        MaxStats();
    }

    private void MaxStats()
    {
        int total = allocatedHP + allocatedEN + allocatedPWR + allocatedSPD;

        bool canIncrease = total < statPoints;

        // HP
        incHP.interactable = canIncrease;
        decHP.interactable = allocatedHP > 0;

        // EN
        incEN.interactable = canIncrease;
        decEN.interactable = allocatedEN > 0;

        // PWR
        incPWR.interactable = canIncrease;
        decPWR.interactable = allocatedPWR > 0;

        // SPD
        incSPD.interactable = canIncrease;
        decSPD.interactable = allocatedSPD > 0;
    }

    public void IncHPbtn()
    {
        allocatedHP++;
        UpdateText();
    }

    public void DecHPbtn()
    {
        allocatedHP--;
        UpdateText();
    }

    public void IncENbtn()
    {
        allocatedEN++;
        UpdateText();
    }

    public void DecENbtn()
    {
        allocatedEN--;
        UpdateText();
    }

    public void IncPWRbtn()
    {
        allocatedPWR++;
        UpdateText();
    }

    public void DecPWRbtn()
    {
        allocatedPWR--;
        UpdateText();
    }

    public void IncSPDbtn()
    {
        allocatedSPD++;
        UpdateText();
    }

    public void DecSPDbtn()
    {
        allocatedSPD--;
        UpdateText();
    }
}
