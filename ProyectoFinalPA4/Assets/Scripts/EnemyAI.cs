using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float detectionRadius = 7f;
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 3.8f;
    public float damageRadius = 1.2f;
    public float damageCooldown = 1.5f;

    private Transform player;
    private int currentWaypointIndex = 0;
    private bool isChasing = false;
    private float damageTimer;

    void Start()
    {
        // Automatically find the player (tagged "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' not found. Enemy will only patrol.");
        }

        damageTimer = damageCooldown; // so damage can happen after cooldown from start
    }

    void Update()
    {
        // Check distance to player if player exists
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRadius)
            {
                isChasing = true;
            }
            else
            {
                isChasing = false;
            }
        }

        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        // Damage timer
        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }
    }

    void ChasePlayer()
    {
        // Move towards player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        // Rotate to look at player (only on Y-axis)
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0; // keep level
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), 10f * Time.deltaTime);
        }

        // Damage if close enough and cooldown ready
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < damageRadius && damageTimer <= 0f)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerHitByEnemy();
            }
            damageTimer = damageCooldown;
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * patrolSpeed * Time.deltaTime;

        // Optional: rotate towards waypoint
        Vector3 lookDirection = targetWaypoint.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), 10f * Time.deltaTime);
        }

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}