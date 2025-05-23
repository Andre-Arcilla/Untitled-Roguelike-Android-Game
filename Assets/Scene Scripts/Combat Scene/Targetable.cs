using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    [Header("Target Settings")]
    public Collider2D targetCollider;
    public Team team;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject highlightEffect;

    void Awake()
    {
        // Get the Collider2D component automatically if it's not set
        if (targetCollider == null)
        {
            targetCollider = GetComponentInChildren<Collider2D>();
        }
    }

    public void SetHighlight(bool active)
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(active);
    }
}

public enum Team
{
    Player,
    Enemy
}