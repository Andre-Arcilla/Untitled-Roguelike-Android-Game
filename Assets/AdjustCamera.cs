using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCamera : MonoBehaviour
{
    public MeshRenderer maze;

    void Start()
    {
        float targetHeight = maze.bounds.size.y * 0.5f;
        float targetWidth = maze.bounds.size.x * 0.5f;
        float screenAspect = (float)Screen.width / Screen.height;
        float targetAspect = maze.bounds.size.x / maze.bounds.size.y;

        if (screenAspect >= targetAspect)
        {
            // Fit vertically
            Camera.main.orthographicSize = targetHeight;
        }
        else
        {
            // Fit horizontally
            Camera.main.orthographicSize = targetWidth / screenAspect;
        }
    }
}