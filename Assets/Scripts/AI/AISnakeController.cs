/*
 * GAME 220: Slither.io Template
 * Session 6: AI Snake Controller
 *
 * TEACHING FOCUS:
 * - Code reuse (similar structure to PlayerSnakeController)
 * - AI decision making with timers
 * - Random direction generation
 * - Same List and Queue patterns as player
 *
 * This script creates an AI-controlled snake that:
 * - Wanders randomly around the play area
 * - Changes direction periodically
 * - Has a body that follows like the player
 * - Can eat food and grow
 */

using System.Collections.Generic;
using UnityEngine;

public class AISnakeController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f; // Slightly slower than player
    public float rotationSpeed = 150f;

    [Header("Body Settings")]
    public int startingSegments = 3;
    public GameObject segmentPrefab;

    [Header("AI Settings")]
    // TEACHING: How often the AI picks a new random direction
    public float directionChangeInterval = 2f;
    // TEACHING: Timer counts down to next direction change
    private float directionChangeTimer;

    // TEACHING: These are the same as PlayerSnakeController!
    // This is code reuse - same patterns work for player and AI
    private List<SnakeSegment> bodySegments = new List<SnakeSegment>();
    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private int maxHistorySize = 100;

    // AI movement
    private Vector3 targetDirection;
    private bool isAlive = true;

    void Start()
    {
        InitializeBody();

        // Start with a random direction
        ChangeDirection();
        directionChangeTimer = directionChangeInterval;
    }

    // TEACHING: Update for AI decision-making (doesn't need fixed timing)
    void Update()
    {
        if (!isAlive) return;

        // TEACHING: Timer counts down using Time.deltaTime
        // Time.deltaTime is the time since last frame (about 0.016s at 60fps)
        directionChangeTimer -= Time.deltaTime;

        // TEACHING: if statement checks if timer reached zero
        if (directionChangeTimer <= 0)
        {
            ChangeDirection();
            // Reset timer
            directionChangeTimer = directionChangeInterval;
        }
    }

    // TEACHING: FixedUpdate for movement and physics (same as player)
    void FixedUpdate()
    {
        if (!isAlive) return;

        MoveInDirection();
        RecordPosition();
        UpdateBodySegments();
        CheckBoundaries();
    }

    // ============================================
    // AI DECISION MAKING
    // ============================================

    void ChangeDirection()
    {
        // TEACHING: Random.insideUnitCircle gives a random point inside a circle
        // radius 1. We normalize it to get a random direction vector
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        targetDirection = new Vector3(randomDir.x, randomDir.y, 0);

        // TEACHING: Alternative - pick from 8 cardinal directions
        // int directionIndex = Random.Range(0, 8);
        // float angle = directionIndex * 45f;
        // targetDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
    }

    // ============================================
    // MOVEMENT (same pattern as player)
    // ============================================

    void MoveInDirection()
    {
        // TEACHING: Move in the current direction
        transform.position += targetDirection * moveSpeed * Time.fixedDeltaTime;

        // TEACHING: Rotate to face movement direction
        if (targetDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }

    // ============================================
    // BODY MANAGEMENT (same as player)
    // ============================================

    void InitializeBody()
    {
        // TEACHING: Same for loop as PlayerSnakeController
        for (int i = 0; i < startingSegments; i++)
        {
            AddSegment();
        }
    }

    void AddSegment()
    {
        GameObject newSegmentObj = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
        SnakeSegment segment = newSegmentObj.GetComponent<SnakeSegment>();

        if (segment != null)
        {
            bodySegments.Add(segment);
        }
    }

    public void Grow(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddSegment();
        }
    }

    void UpdateBodySegments()
    {
        // TEACHING: Same foreach loop as player
        int index = 0;
        foreach (SnakeSegment segment in bodySegments)
        {
            Vector3 targetPos = GetHistoricalPosition(index);
            segment.FollowPosition(targetPos);
            index++;
        }
    }

    // ============================================
    // POSITION HISTORY (same as player)
    // ============================================

    void RecordPosition()
    {
        positionHistory.Enqueue(transform.position);

        // TEACHING: Same while loop to limit queue size
        while (positionHistory.Count > maxHistorySize)
        {
            positionHistory.Dequeue();
        }
    }

    Vector3 GetHistoricalPosition(int segmentIndex)
    {
        int spacing = 5;
        int historyIndex = segmentIndex * spacing;

        if (historyIndex >= positionHistory.Count)
        {
            historyIndex = positionHistory.Count - 1;
        }

        Vector3[] historyArray = positionHistory.ToArray();
        return historyArray[historyIndex];
    }

    // ============================================
    // COLLISION & BOUNDARIES
    // ============================================

    void CheckBoundaries()
    {
        if (!BoundsHelper.IsInBounds(transform.position))
        {
            // TEACHING: AI wraps around boundaries (doesn't die)
            transform.position = BoundsHelper.WrapPosition(transform.position);

            // Also change direction when hitting boundary
            ChangeDirection();
        }
    }

    void Die()
    {
        isAlive = false;

        // TEACHING: foreach loop destroys all body segments
        foreach (SnakeSegment segment in bodySegments)
        {
            Destroy(segment.gameObject);
        }

        // Destroy the AI snake itself
        Destroy(gameObject);
    }

    // TEACHING: AI can be killed by player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Player collides with AI - AI loses
            Die();
        }
    }

    // ============================================
    // CLEANUP
    // ============================================

    void OnDestroy()
    {
        // TEACHING: Clean up all segments when AI is destroyed
        foreach (SnakeSegment segment in bodySegments)
        {
            if (segment != null)
            {
                Destroy(segment.gameObject);
            }
        }
    }
}
