using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;        // Reference to the player's transform
    public float detectionRange = 10f; // Distance at which the enemy detects the player
    public float shootingInterval = 1f; // Time between shots
    public GameObject projectile;   // The projectile to shoot
    public Transform shootPoint;    // The point from where the projectile will be fired
    public float projectileSpeed = 20f; // Speed of the projectile

    private bool playerInRange = false;
    private float timeSinceLastShot = 0f;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                StartShooting();
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                StopShooting();
            }
        }

        if (playerInRange)
        {
            timeSinceLastShot += Time.deltaTime;

            if (timeSinceLastShot >= shootingInterval)
            {
                Shoot();
                timeSinceLastShot = 0f;
            }
        }
    }

    void StartShooting()
    {
        // Optionally, add any logic that should occur when the enemy starts shooting
        Debug.Log("Enemy detected the player and started shooting!");
    }

    void StopShooting()
    {
        // Optionally, add any logic that should occur when the enemy stops shooting
        Debug.Log("Enemy lost the player and stopped shooting!");
    }

    void Shoot()
    {
        if (projectile != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Vector3 dirToPlayer = (player.position - shootPoint.position).normalized;

            if (rb != null)
            {
                rb.velocity = dirToPlayer * projectileSpeed;
            }
        }
    }
}