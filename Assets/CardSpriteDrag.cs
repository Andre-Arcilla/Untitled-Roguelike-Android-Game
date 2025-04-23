using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteDrag : MonoBehaviour
{
    public static CardSpriteDrag Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only one allowed
            return;
        }

        Instance = this;
    }

    [SerializeField] private GameObject allyTarget;
    [SerializeField] private GameObject enemyTarget;

    public void CheckIfInDropZone(CardSprite card)
    {
        Collider2D dropCollider = enemyTarget.GetComponentInChildren<Collider2D>();
        Collider2D cardCollider = card.GetComponentInChildren<Collider2D>();

        if (dropCollider != null && cardCollider != null)
        {
            Bounds dropBounds = dropCollider.bounds;
            Bounds cardBounds = cardCollider.bounds;

            bool overlapsXY =
                cardBounds.min.x < dropBounds.max.x && cardBounds.max.x > dropBounds.min.x &&
                cardBounds.min.y < dropBounds.max.y && cardBounds.max.y > dropBounds.min.y;

            //put them in a list before doing the action

            if (overlapsXY)
            {
                CharacterInfo character = card.GetComponentInParent<CharacterInfo>();
                EnemyInfo enemy = dropCollider.GetComponentInParent<EnemyInfo>();

                if (character.currentMana >= card.card.mana)
                {
                    ActionSystem.Instance.AddCardToList(card);
                    card.gameObject.SetActive(false);
                    CardSpriteHover.Instance.Drag(false);
                    CardSpriteHover.Instance.Hide();
                    Debug.Log("card used: " + card.card.cardName);

                    character.currentMana -= card.card.mana;
                    Debug.Log("new current mana: " + character.currentMana);

                    enemy.totalHP -= card.card.power * character.stats.totalPWR;
                    Debug.Log("new current mana: " + character.currentMana);
                }
                else
                {
                    Debug.Log("not enough mana for: " + card.card.cardName);
                }
                Debug.Log("Card is in drop zone! ");
            }
            else
            {
                Debug.Log("Card is outside drop zone.");
            }
        }
    }
}