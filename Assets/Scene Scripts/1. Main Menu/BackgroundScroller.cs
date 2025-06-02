using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Transform[] tiles;         // Assign 4 tile GameObjects in the inspector
    public float scrollSpeed = 2f;

    private float tileWidth;

    private void Start()
    {
        if (tiles.Length == 0)
        {
            Debug.LogError("No background tiles assigned!");
            return;
        }

        // Get the width of the first tile using its Renderer
        Renderer rend = tiles[0].GetComponent<Renderer>();
        if (rend != null)
        {
            tileWidth = rend.bounds.size.x;
        }
        else
        {
            Debug.LogError("Tile object missing Renderer component!");
        }
    }

    private void Update()
    {
        foreach (var tile in tiles)
        {
            tile.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // If tile has moved past its own width to the left, move it to the right
            if (tile.position.x < GetLeftResetPosition())
            {
                tile.position = new Vector3(GetRightmostX() + tileWidth, tile.position.y, tile.position.z);
            }
        }
    }

    private float GetLeftResetPosition()
    {
        // The reset threshold is one full tile width to the left of screen view
        return Camera.main.ViewportToWorldPoint(Vector3.zero).x - tileWidth;
    }

    private float GetRightmostX()
    {
        float maxX = tiles[0].position.x;
        foreach (var tile in tiles)
        {
            if (tile.position.x > maxX)
                maxX = tile.position.x;
        }
        return maxX;
    }
}
