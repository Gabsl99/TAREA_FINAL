using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public float smoothSpeed = 10f;
    public Vector3 offset = new Vector3(0, 6f, -8f);
    public Vector3 lookOffset = new Vector3(0, 1.2f, 0);

    private Transform target;

    void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            // Teleport camera immediately to offset position
            transform.position = target.position + offset;
            transform.LookAt(target.position + lookOffset);
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' not found. Camera will not follow anything.");
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            // Try to find player again
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                // Teleport immediately when found
                transform.position = target.position + offset;
                transform.LookAt(target.position + lookOffset);
            }
            return;
        }

        // Desired position
        Vector3 targetPos = target.position + offset;
        // Smoothly interpolate
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        // Look at player with look offset
        transform.LookAt(target.position + lookOffset);
    }
}