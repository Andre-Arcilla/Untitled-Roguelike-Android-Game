using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCardView : MonoBehaviour
{
    [SerializeField] private GameObject cardView;

    private void OnMouseDown()
    {
        //call a method in CharacterManager and send cardView
        CharacterManager.Instance.DisplayCardView(cardView);
    }

}
