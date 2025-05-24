using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public float driftSpeed = 1f;             // Shared speed for all clouds
    public float bufferDistance = 2f;         // Extra distance to avoid tight overlaps
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Move the entire group
        transform.Translate(Vector3.left * driftSpeed * Time.deltaTime);

        foreach (Transform cloud in transform)
        {
            float cloudRightEdge = cloud.position.x + cloud.GetComponent<SpriteRenderer>().bounds.size.x / 2f;
            float camLeftEdge = mainCam.transform.position.x - (mainCam.orthographicSize * mainCam.aspect);

            if (cloudRightEdge < camLeftEdge)
            {
                // Find the rightmost cloud
                Transform rightmost = GetRightmostCloud();

                float width = cloud.GetComponent<SpriteRenderer>().bounds.size.x;
                Vector3 newPos = cloud.position;
                newPos.x = rightmost.position.x + rightmost.GetComponent<SpriteRenderer>().bounds.size.x + bufferDistance;
                cloud.position = newPos;
            }
        }
    }

    private Transform GetRightmostCloud()
    {
        Transform rightmost = transform.GetChild(0);
        foreach (Transform cloud in transform)
        {
            if (cloud.position.x > rightmost.position.x)
            {
                rightmost = cloud;
            }
        }
        return rightmost;
    }
}
