using System.Collections.Generic;
using UnityEngine;

public class DisplayCardView : MonoBehaviour
{
    [SerializeField] private GameObject cardView;

    private void OnMouseDown()
    {
        Transform parentTransform = transform.parent;

        if (CharacterManager.Instance.characterList.Contains(parentTransform.gameObject))
        {
            CharacterManager.Instance.DisplayCardView(cardView);
        }
    }
}
