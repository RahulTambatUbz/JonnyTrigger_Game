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
    [SerializeField] BulletUI bulletUI;
    [SerializeField] GameObject endGameUI;
    [SerializeField] GameObject StartUI;
    [SerializeField] bool isGameStarted;
    public enum PlayerState
    {
        Running,
        Flipping,
        Moving,
        Jumping
    }

    void OnTriggerEnter(Collider other)
    {
        FlipPath flipPath = other.gameObject.GetComponent<FlipPath>();
        if (flipPath != null)
        {
            pointB = flipPath.GetPathPoint();
            flipDuration = 2f;

            if (flipPath.currentState == PlayerStates.Flipping && !isFlipping)
            {
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
            else if (flipPath.currentState == PlayerStates.Moving)
            {
                currentState = PlayerState.Moving;
                pointA = transform.position;
                flipTime = 0f;
                Debug.Log("fix");
            }
        }
        else
        {
            Debug.LogError("FlipPath component not found on the trigger object.");
        }

        if(other.gameObject.CompareTag("MOVETRIGGER"))
        {

            Time.timeScale = 0f;
            endGameUI.SetActive(true);


        }
    }

    private void Awake()
    {
       // animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ammo = GetComponent<Ammo>();
        ammoText.text = ammo.currentAmmo.ToString();
        endGameUI.SetActive(false);
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        isGameStarted = false;
    }

    private void Start()
    {
        currentState = PlayerState.Running;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
                {
            if (!isGameStarted)
            {
                StartUI.SetActive(false);
                isGameStarted = true;
            }
        }

        if (isGameStarted)
        {
            switch (currentState)
            {
                case PlayerState.Running:
                    HandleRunning();
                    break;

                case PlayerState.Moving:
                    HandleMoving();
                    break;

                case PlayerState.Jumping:
                    // HandleJumping();
                    break;

                case PlayerState.Flipping:
                    HandleFlipping();
                    break;
            }
        }
    }

    private void HandleRunning()
    {
        animator.SetBool("IsRunning", true);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void HandleFlipping()
    {
        if (isFlipping)
        {
            animator.SetBool("IsRunning", false);
            flipTime += Time.deltaTime * (0.2f / slowMoTimeScale);
            UpdateLineRenderer();
            lineRenderer.enabled = true;

            float t = flipTime / flipDuration;

            // Avoid division by zero
            if (flipDuration == 0)
            {
                Debug.LogError("flipDuration is zero, which will cause a divide by zero error.");
                return;

            }

            float arc = Mathf.Sin(Mathf.PI * t) * arcHeight;

            Vector3 targetPosition = Vector3.Lerp(pointA, pointB.position, t);
            targetPosition.y += arc;

            transform.position = targetPosition;

            float angle = Mathf.Lerp(0, 360, t);
            transform.rotation = Quaternion.Euler(angle, 0, 0) * Quaternion.LookRotation(pointB.position - pointA);

            if (Input.GetButtonDown("Fire1"))
            {
                ShootBullet();
            }

            if (flipTime >= flipDuration)
            {
                isFlipping = false;
                currentState = PlayerState.Running;
                rb.useGravity = true;
                animator.SetBool("IsFiring", isFlipping);
                transform.position = pointB.position;
                transform.rotation = Quaternion.LookRotation(pointB.position - pointA);
                StartCoroutine(EndSlowMotion());
                lineRenderer.enabled = false;
            }
        }
    }

    private void HandleMoving()
    {
        flipTime += Time.deltaTime / flipDuration; // Normalized time based on flipDuration

        if (flipDuration == 0)
        {
            Debug.LogError("flipDuration is zero, which will cause a divide by zero error.");
            return;
        }

        float t = Mathf.Clamp01(flipTime); // Ensure t is clamped between 0 and 1

        // Lerp the position without the arc and rotation
        transform.position = Vector3.Lerp(pointA, pointB.position, t);

        if (Input.GetButtonDown("Fire1"))
        {
            ShootBullet();
        }

        if (flipTime >= 1f)
        {
            currentState = PlayerState.Running;
            lineRenderer.enabled = false;
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
            if (ammo.currentAmmo > 0)
            {
                ammo.currentAmmo--;
                ammoText.text = ammo.currentAmmo.ToString();
                bulletUI.UpdateUI();

                GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                if (bulletRb != null)
                {
                    bulletRb.velocity = shootingPoint.forward * bulletSpeed;
                }
            }
            else
            {
                Debug.Log("Out of Ammo");
                ReloadAmmo();
                bulletUI.ReloadBulletUI();
            }
        }
    }


    IEnumerator SmoothSlowMotion(float targetTimeScale, float transitionDuration)
    {
        float start = Time.timeScale;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            Time.timeScale = Mathf.Lerp(start, targetTimeScale, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
       
    }

    IEnumerator EndSlowMotion()
    {
        float start = Time.timeScale;
        float elapsedTime = 0f;

        while (elapsedTime < slowMoTransitionDuration)
        {
            Time.timeScale = Mathf.Lerp(start, originalTimeScale, elapsedTime / slowMoTransitionDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = originalTimeScale;
    }


    private void UpdateLineRenderer()
    {
        lineRenderer.SetPosition(0, shootingPoint.position);
        lineRenderer.SetPosition(1, shootingPoint.position + shootingPoint.forward * shootingProjectilePathLength);
    }

   public void ReloadAmmo()
    {

        if(ammo.currentAmmo <=0)
        {
            ammo.currentAmmo = ammo.maxAmmo;


        }


    }
}
