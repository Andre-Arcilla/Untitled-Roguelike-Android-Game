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

    public RaceDataSO selectedRace {  get; private set; }
    public int allocatedHP { get; private set; }
    public int allocatedEN { get; private set; }
    public int allocatedPWR { get; private set; }
    public int allocatedSPD { get; private set; }

    [SerializeField] private int statPoints;
    [SerializeField] private List<Racebtn> raceButtons;
    [SerializeField] private Racebtn defaultRace; public int RemainingStatPoints
    {
        get
        {
            return statPoints - (allocatedHP + allocatedEN + allocatedPWR + allocatedSPD);
        }
    }

    private void Start()
    {
        defaultRace.SendStats();
        UpdateText();
    }

    public void UpdateStats(RaceDataSO race)
    {
        selectedRace = race;

        UpdateText();
        UpdateRaceButtons();
    }

    private void UpdateRaceButtons()
    {
        foreach (var btn in raceButtons)
        {
            btn.SetInteractable(btn.raceName != selectedRace.raceName);
        }
    }

    private void UpdateText()
    {
        int hp = selectedRace.HP + allocatedHP;
        int en = selectedRace.EN + allocatedEN;
        int pwr = selectedRace.PWR + allocatedPWR;
        int spd = selectedRace.SPD + allocatedSPD;
        int stats = statPoints - (allocatedHP + allocatedEN + allocatedPWR + allocatedSPD);

        HPText.text = hp.ToString("D2");
        ENText.text = en.ToString("D2");
        PWRText.text = pwr.ToString("D2");
        SPDText.text = spd.ToString("D2");
        StatText.text = stats.ToString("D2");

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
