using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInfo : MonoBehaviour
{
    public CharacterInfo[] characterInfo;
    public GameObject[] CardHolder;
    public Text Name;
    public Text Race;
    public Text Level;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();
    }

    // Update is called once per frame
    public void UpdateDisplay()
    {
        for (int i = 0; i < characterInfo.Length; i++)
        {
            if (CardHolder[i].activeSelf)
            {
                CharacterInfo character = characterInfo[i];
            }
        }
    }
}
