using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public float parallaxSpeed = 2f;
    public bool manualControl = true;

    private float spriteWidth;
    private Vector3 startPos;

    private void Start()
    {
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPos = transform.position;
    }

    private void Update()
    {
        if (!manualControl)
        {
            Scroll(Time.deltaTime);
        }
    }

    // Called manually by manager or automatically if manualControl is false
    public void Scroll(float deltaTime)
    {
        transform.position += Vector3.left * parallaxSpeed * deltaTime;

        if (transform.position.x < startPos.x - spriteWidth)
        {
            transform.position += new Vector3(spriteWidth * 2f, 0f, 0f);
        }
    }
}
