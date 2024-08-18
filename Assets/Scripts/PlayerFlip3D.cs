using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerFlip3D : MonoBehaviour
{
    [SerializeField] TMP_Text ammoText;
    public Transform pointB;          // The point where the player will land after the flip
    public float flipDuration = 1f;   // The duration of the flip
    public float arcHeight = 2f;      // The height of the arc
    public float slowMoTimeScale = 0.2f;  // The time scale during slow motion
    public float slowMoDuration = 1f;  // The duration of the slow-motion effect
    public float slowMoTransitionDuration = 0.5f;  // Duration of the transition to/from slow-mo
    [SerializeField] Rigidbody rb;
    public GameObject bulletPrefab;   // The bullet prefab to shoot
    public Transform shootingPoint;   // The point from where the bullet will be shot
    public float bulletSpeed = 20f;   // The speed of the bullet
    [SerializeField] Animator animator;
    private Ammo ammo;
    private bool isFlipping = false;
    private Vector3 pointA;
    private float flipTime = 0f;
    private float originalTimeScale;
    private LineRenderer lineRenderer;
    [SerializeField] private float shootingProjectilePathLength = 2f;
    private PlayerState currentState;
    [SerializeField] private float moveSpeed;




    public enum PlayerState
    {
        Running,
        Flipping,
        Moving,
        Jumping


    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FlipTrigger") && !isFlipping)
        {

            pointB = other.gameObject.GetComponent<FlipPath>().GetPathPoint();
            flipDuration = other.gameObject.GetComponent<FlipPath>().GetFlipDuration();
            // Start slow motion with smooth transition
            StartCoroutine(SmoothSlowMotion(slowMoTimeScale, slowMoTransitionDuration));
            pointA = transform.position;
            isFlipping = true;
            currentState = PlayerState.Flipping;
            animator.SetBool("IsFiring", isFlipping);
            rb.useGravity = false;
            flipTime = 0f;
            originalTimeScale = Time.timeScale;
        }
        if (other.CompareTag("MOVETRIGGER"))
        {
            pointB = other.gameObject.GetComponent<FlipPath>().GetPathPoint();
            flipDuration = other.gameObject.GetComponent<FlipPath>().GetFlipDuration();
            currentState = PlayerState.Moving;
            
        }
    }
    private void Awake()
    {

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ammo = GetComponent<Ammo>();
        ammoText.text = ammo.currentAmmo.ToString();
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();

        }
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }
    private void Start()
    {
        currentState = PlayerState.Running;
    }
    void Update()
    {
        switch (currentState)
        {


            case PlayerState.Running:
                HandelRunning();
                break;

            case PlayerState.Moving:
                HandleMoving();
                break;

            case PlayerState.Jumping:
                //HandelJumping();
                break;

            case PlayerState.Flipping:
                HandelFlipping();
                break;






        }

    }
    private void HandelRunning()
    {

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);



    }
    private void HandelFlipping()
    {
        if (isFlipping)
        {

            flipTime += Time.deltaTime * (0.2f / slowMoTimeScale);
            UpdateLineRenderer();
            lineRenderer.enabled = true;
            // Calculate the normalized time (t) for the flip
            float t = flipTime / flipDuration;

            // Calculate the arc height using a sine wave for a smooth curve
            float arc = Mathf.Sin(Mathf.PI * t) * arcHeight;

            // Perform the flip
            float angle = Mathf.Lerp(0, 360, t);  // Rotate from 0 to 360 degrees
            Vector3 targetPosition = Vector3.Lerp(pointA, pointB.position, t);
            targetPosition.y += arc;  // Add the arc height to the Y position
            transform.position = targetPosition;  // Move from point A to point B with the arc

            // Rotate around the forward axis (adjust as necessary for your game's needs)
            transform.rotation = Quaternion.Euler(angle, 0, 0) * Quaternion.LookRotation(pointB.position - pointA);

            // Check if the shoot button is pressed
            if (Input.GetButtonDown("Fire1")) // Default "Fire1" is left mouse button or Ctrl
            {
                ShootBullet();
            }

            if (flipTime >= flipDuration)
            {
                // End flip and start transition back to normal speed
                isFlipping = false;
                currentState = PlayerState.Running;
                rb.useGravity = true;
                animator.SetBool("IsFiring", isFlipping);
                transform.position = pointB.position;  // Ensure final position is point B
                transform.rotation = Quaternion.LookRotation(pointB.position - pointA);  // Reset rotation to look at point B
                StartCoroutine(EndSlowMotion());
                lineRenderer.enabled = false;
            }
        }



    }
    private void HandleMoving()
    {
        flipTime += Time.deltaTime * (0.2f / slowMoTimeScale);
        UpdateLineRenderer();
        lineRenderer.enabled = true;
        // Calculate the normalized time (t) for the flip
        float time = flipTime / flipDuration;
        // Check if the shoot button is pressed
        if (Input.GetButtonDown("Fire1")) // Default "Fire1" is left mouse button or Ctrl
        {
            ShootBullet();
        }
        //logic for moving from point A to B

        transform.position = Vector3.Lerp(pointA, pointB.position,time);

        if (flipTime >= flipDuration)
        {
            currentState = PlayerState.Running; // or whatever the next state should be
        }
    }

    void HandleJumping()
    {
        // Logic for jumping from point A to B to C to N
    }
    void ShootBullet()
    {
        if (bulletPrefab != null && shootingPoint != null)
        {
            if (ammo.currentAmmo != 0)
            {
                ammo.currentAmmo--;
                ammoText.text = ammo.currentAmmo.ToString();
                // Instantiate the bullet at the shooting point
                GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

                // Get the Rigidbody component and apply force to it
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = shootingPoint.forward * bulletSpeed;
                }

            }


        }
    }


    IEnumerator SmoothSlowMotion(float targetTimeScale, float duration)
    {
        float startTimeScale = Time.timeScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, elapsedTime / duration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = targetTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    IEnumerator EndSlowMotion()
    {
        // Wait for the additional slow-mo duration
        yield return new WaitForSecondsRealtime(slowMoDuration);

        // Transition back to normal speed
        StartCoroutine(SmoothSlowMotion(originalTimeScale, slowMoTransitionDuration));
    }

    void UpdateLineRenderer()
    {
        if (lineRenderer != null && shootingPoint != null)
        {
            lineRenderer.SetPosition(0, shootingPoint.position);
            lineRenderer.SetPosition(1, shootingPoint.position + shootingPoint.up * shootingProjectilePathLength);
        }
    }


}
