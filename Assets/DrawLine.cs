using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public static DrawLine Instance { get; private set; }

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

    [Header("Line Settings")]
    [SerializeField, Tooltip("Number of points used to define the curve smoothness.")]
    private int curveResolution = 20;

    [SerializeField, Tooltip("Width of the rendered line.")]
    private float lineWidth = 0.1f;

    [SerializeField, Tooltip("how high the line goes before curving.")]
    private float upwardLength = 1f;

    [SerializeField, Tooltip("Starting color of the line gradient.")]
    private Color lineColorStart = Color.white;

    [SerializeField, Tooltip("Ending color of the line gradient.")]
    private Color lineColorEnd = Color.white;

    private Dictionary<GameObject, LineRenderer> lineByCard = new Dictionary<GameObject, LineRenderer>();

    public void CreateCurvedLine(Transform senderParent, GameObject card, Vector3 start, Vector3 end, Color? senderColor = null, Color? targetColor = null)
    {
        GameObject lineObj = new GameObject("CurvedLine");
        lineObj.transform.SetParent(senderParent);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = curveResolution;
        lr.widthMultiplier = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = senderColor ?? lineColorStart;
        lr.endColor = targetColor ?? lineColorEnd;
        lr.useWorldSpace = true;
        lr.sortingLayerName = "Default";
        lr.sortingOrder = -1;

        // Two control points: one up, one down
        Vector3 control1 = start + Vector3.up * upwardLength;
        Vector3 control2 = end + Vector3.down * upwardLength;

        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);

            // Cubic Bézier formula
            Vector3 point =
                Mathf.Pow(1 - t, 3) * start +
                3 * Mathf.Pow(1 - t, 2) * t * control1 +
                3 * (1 - t) * Mathf.Pow(t, 2) * control2 +
                Mathf.Pow(t, 3) * end;

            point.z = start.z - 0.1f;
            lr.SetPosition(i, point);
        }

        lineByCard[card] = lr;
    }

    public void UpdateAllLineStarts()
    {
        foreach (var kvp in lineByCard)
        {
            GameObject card = kvp.Key;
            LineRenderer lr = kvp.Value;

            if (card == null || lr == null) continue;

            // Get new start position with same logic as DrawLineCoroutine:
            Vector3 start = Vector3.zero;
            var collider2D = card.GetComponentInChildren<Collider2D>();
            if (collider2D != null)
            {
                start = collider2D.transform.position;
                start.y += 1f;
                start.z -= 0.5f;
            }

            // Get current end position from the last point in the LineRenderer
            Vector3 end = lr.GetPosition(lr.positionCount - 1);

            // Recalculate control points and positions for Bézier curve
            Vector3 control1 = start + Vector3.up * upwardLength;
            Vector3 control2 = end + Vector3.down * upwardLength;

            for (int i = 0; i < curveResolution; i++)
            {
                float t = i / (float)(curveResolution - 1);

                Vector3 point =
                    Mathf.Pow(1 - t, 3) * start +
                    3 * Mathf.Pow(1 - t, 2) * t * control1 +
                    3 * (1 - t) * Mathf.Pow(t, 2) * control2 +
                    Mathf.Pow(t, 3) * end;

                point.z = start.z - 0.1f;
                lr.SetPosition(i, point);
            }
        }
    }

    public void RemoveLineForCard(GameObject card)
    {
        if (lineByCard.TryGetValue(card, out LineRenderer lr))
        {
            lr.transform.localScale = Vector3.zero;
            Destroy(lr.gameObject);
            lineByCard.Remove(card);
        }
    }

    public void RemoveAllLines()
    {
        foreach (var lr in lineByCard.Values)
        {
            if (lr != null)
            {
                Destroy(lr.gameObject);
            }
        }
        lineByCard.Clear();
    }
}
