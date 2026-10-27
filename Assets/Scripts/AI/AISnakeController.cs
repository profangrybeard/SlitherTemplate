/*
 * GAME 220: Slither Template -- AISnakeController
 * An AI snake. It has the same List, for, and foreach patterns as PlayerSnakeController.
 * The only real difference: a timer picks the direction instead of the pointer.
 * Open the two files side by side. Most of this one should look familiar.
 *
 * Read alongside: LOOPS_AND_LISTS_CONCEPTS.md section 8
 * Session 6: compare this file with PlayerSnakeController
 */

using System.Collections.Generic;
using UnityEngine;

public class AISnakeController : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // SETTINGS - Tweak these on the AISnake prefab
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 150f;

    [Header("Body")]
    public int startingSegments = 3;
    public GameObject segmentPrefab;             // Drag SnakeSegment.prefab here
    public float segmentSpacing = 0.5f;

    [Header("AI Behavior")]
    public float directionChangeInterval = 2f;   // Seconds between new random directions

    // ─────────────────────────────────────────────────────────────────────────
    // STATE - Tracked while playing
    // ─────────────────────────────────────────────────────────────────────────

    private List<SnakeSegment> bodySegments = new List<SnakeSegment>(); // Same List as the player's
    private Vector3 moveDirection;
    private float directionChangeTimer;          // Counts DOWN to 0, then a new direction is picked
    private bool isAlive = true;

    // ─────────────────────────────────────────────────────────────────────────
    // UNITY MESSAGES - Unity calls these automatically
    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        ChangeDirection();
        directionChangeTimer = directionChangeInterval;
        CreateStartingBody();
    }

    // TIMERS: subtract Time.deltaTime (the seconds since last frame) every frame.
    //   When the timer reaches 0, that much real time has passed. Do the thing, then reset it.
    // This takes the place of the player's steerAction.ReadValue() in PlayerSnakeController.Update().
    void Update()
    {
        if (!isAlive) return;

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            // Why reset the timer here? What would happen if this line were missing?
            ChangeDirection();
            directionChangeTimer = directionChangeInterval;
        }
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        MoveHead();
        CheckBoundaries();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MOVEMENT - A timer instead of input
    // ─────────────────────────────────────────────────────────────────────────

    // Random.insideUnitCircle can return a short arrow. Why .normalized before using it as a direction?
    void ChangeDirection()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        moveDirection = new Vector3(randomDir.x, randomDir.y, 0);
    }

    // Line for line the same as PlayerSnakeController.MoveHead()
    void MoveHead()
    {
        transform.position += moveDirection * moveSpeed * Time.fixedDeltaTime;

        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BODY LIST - The same pattern as the player
    // ─────────────────────────────────────────────────────────────────────────

    void CreateStartingBody()
    {
        for (int i = 0; i < startingSegments; i++)                    // [LOOP]
        {
            AddSegment();
        }
    }

    // Compare with PlayerSnakeController.AddSegment(). Only one line is really different. Which one?
    void AddSegment()
    {
        Vector3 spawnPosition;
        Transform targetToFollow;

        if (bodySegments.Count == 0)
        {
            spawnPosition = transform.position - transform.up * segmentSpacing;
            targetToFollow = transform;
        }
        else
        {
            SnakeSegment lastSegment = bodySegments[bodySegments.Count - 1];
            spawnPosition = lastSegment.transform.position;
            targetToFollow = lastSegment.transform;
        }

        GameObject newSegmentObj = Instantiate(segmentPrefab, spawnPosition, Quaternion.identity);
        SnakeSegment newSegment = newSegmentObj.GetComponent<SnakeSegment>();

        if (newSegment != null)
        {
            newSegment.SetTarget(targetToFollow, segmentSpacing);

            SpriteRenderer headSprite = GetComponent<SpriteRenderer>();
            SpriteRenderer segmentSprite = newSegmentObj.GetComponent<SpriteRenderer>();
            if (headSprite != null && segmentSprite != null)
            {
                segmentSprite.color = headSprite.color;
            }

            newSegmentObj.tag = "AISnake";
            CircleCollider2D collider = newSegmentObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            collider.isTrigger = true;

            bodySegments.Add(newSegment);
        }
    }

    // Called by FoodPellet with the pellet's growthAmount
    public void Grow(int amount)
    {
        for (int i = 0; i < amount; i++)                              // [LOOP]
        {
            AddSegment();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // EDGES, COLLISION, CLEANUP
    // ─────────────────────────────────────────────────────────────────────────

    void CheckBoundaries()
    {
        if (!BoundsHelper.IsInBounds(transform.position))
        {
            Vector3 oldPosition = transform.position;
            transform.position = BoundsHelper.WrapPosition(transform.position);
            Vector3 teleportOffset = transform.position - oldPosition;

            foreach (SnakeSegment segment in bodySegments)                // [LOOP]
            {
                if (segment != null)
                {
                    segment.Teleport(teleportOffset);
                }
            }

            // The player doesn't do this after wrapping. Why does the AI pick a new direction here?
            ChangeDirection();
        }
    }

    // The player's body segments are tagged "Player" too (see PlayerSnakeController.AddSegment()),
    // so this head dies on touching any part of the player.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }

    void Die()
    {
        // Why guard against a second call? How many of the player's colliders can this head touch at once?
        if (!isAlive) return;
        isAlive = false;

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnAISnakeDied(this);     // Keeps GameManager's List in sync
        }

        foreach (SnakeSegment segment in bodySegments)                    // [LOOP]
        {
            if (segment != null)
            {
                Destroy(segment.gameObject);
            }
        }

        Destroy(gameObject);
    }

    // Die() already destroys the segments. Why does OnDestroy() need its own loop? Look at GameManager.GameOver().
    void OnDestroy()
    {
        foreach (SnakeSegment segment in bodySegments)                    // [LOOP]
        {
            if (segment != null)
            {
                Destroy(segment.gameObject);
            }
        }
    }
}
