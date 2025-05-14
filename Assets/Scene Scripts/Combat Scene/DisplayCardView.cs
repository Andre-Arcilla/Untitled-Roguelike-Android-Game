using System.Collections.Generic;
using UnityEngine;

public class DisplayCardView : MonoBehaviour
{
    [SerializeField] private GameObject cardView;

    private void OnMouseDown()
    {
        if (CharacterManager.Instance.characterList.Contains(cardView.transform.parent.gameObject))
        {
            CharacterManager.Instance.DisplayCardView(cardView);
        }
    }

    public void SetCardView(GameObject cardView)
    {
        this.cardView = cardView;
    }
}
