using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCard : MonoBehaviour
{
    private Vector3 originalPosition; // Store the original position
    private Vector3 offset; // Offset for dragging the object

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        // Store the original position at the start
        originalPosition = transform.position;
        // Calculate the offset for dragging
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        // Update the object's position as the mouse is dragged
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
    }

    void OnMouseUp()
    {
        // Return the object to its original position when the drag ends
        transform.position = originalPosition;
    }
}
