using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacter : MonoBehaviour
{
    public GameObject[] cardHolders;
    public GameObject thisCardHolder;
    public DisplayInfo displayInfo;

    void OnMouseDown()
    {
        ToggleCardHolders();
    }

    void ToggleCardHolders()
    {
        foreach (GameObject holder in cardHolders)
        {
            if (holder == thisCardHolder)
            {
                holder.SetActive(true);
                Invoke(nameof(UpdateDisplayDelayed), 0.05f);
            }
            else
            {
                holder.SetActive(false);
            }
        }
    }

    void UpdateDisplayDelayed()
    {
        displayInfo.UpdateDisplay();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
