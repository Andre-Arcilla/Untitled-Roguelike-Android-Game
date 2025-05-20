using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitch : MonoBehaviour
{
    [SerializeField] private GameObject panelA;
    [SerializeField] private GameObject panelB;

    public void SwitchPanelAction()
    {
        if (panelA.activeSelf)
        {
            panelA.SetActive(false);
            panelB.SetActive(true);
        }
        else if (panelB.activeSelf)
        {
            panelB.SetActive(false);
            panelA.SetActive(true);
        }
    }
}
