/*
 * GAME 220: Slither.io Template
 * Session 1-6: Player Snake Controller
 *
 * TEACHING FOCUS:
 * - Update vs FixedUpdate (input vs physics)
 * - Lists for managing body segments
 * - Queue for position history
 * - for loops, foreach loops, while loops
 * - Collision detection
 *
 * This script demonstrates the complete player snake behavior including:
 * - Mouse-following movement
 * - Body segment management using Lists
 * - Smooth following using Queue
 * - Growth mechanics
 * - Collision detection (food, self, AI, boundaries)
 */

using System.Collections.Generic;
using UnityEngine;

public class PlayerSnakeController : MonoBehaviour
{
    [Header("Movement Settings")]
    // TEACHING: Public fields appear in the Unity Inspector for easy tweaking
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;

    [Header("Body Settings")]
    public int startingSegments = 3;
    public GameObject segmentPrefab;

    [Header("Collision Settings")]
    public float collisionDistance = 0.4f;

    // TEACHING: Lists store collections that can grow/shrink
    // We use this to track all body segments behind the head
    private List<SnakeSegment> bodySegments = new List<SnakeSegment>();

    // TEACHING: Queue is FIFO (First-In-First-Out) - perfect for position trails
    // Each segment follows a position from history based on its index
    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private int maxHistorySize = 100; // Limits memory usage

    // Input tracking
    private Vector3 mouseWorldPosition;
    private Vector3 moveDirection;

    // State
    private bool isAlive = true;

    void Start()
    {
        // TEACHING: Sessions 2-3 add body initialization
        // For Session 1, we just have the head
        InitializeBody();
    }

    // TEACHING: Update() runs every frame at variable rate (60fps, 120fps, etc.)
    // We use this for INPUT because we want immediate responsiveness
    void Update()
    {
        if (!isAlive) return;

        // Read mouse input every frame for smooth, responsive control
        GetMouseInput();
    }

    // TEACHING: FixedUpdate() runs at fixed rate (default 50Hz = 0.02 seconds)
    // We use this for PHYSICS and MOVEMENT for consistent, predictable behavior
    void FixedUpdate()
    {
        if (!isAlive) return;

        // All movement and physics happens here for consistency
        MoveTowardsMouse();
        RecordPosition(); // Session 3: Queue management
        UpdateBodySegments(); // Session 2-3: Update all segments
        CheckSelfCollision(); // Session 5: Collision detection
        CheckBoundaries(); // Session 5: Boundary handling
    }

    // ============================================
    // SESSION 1: Mouse Input & Movement
    // ============================================

    void GetMouseInput()
    {
        // TEACHING: Input.mousePosition gives screen coordinates (pixels)
        // We need to convert to world coordinates (Unity units) using the camera
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Keep at z=0 for 2D

        // Calculate direction from head to mouse
        moveDirection = (mouseWorldPosition - transform.position).normalized;
    }

    void MoveTowardsMouse()
    {
        // TEACHING: Move toward mouse position smoothly
        // Time.fixedDeltaTime is the time between FixedUpdate calls (0.02s)
        transform.position += moveDirection * moveSpeed * Time.fixedDeltaTime;

        // TEACHING: Rotate to face movement direction
        if (moveDirection != Vector3.zero)
        {
            // Atan2 converts direction vector to angle in radians
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;

            // Smoothly rotate toward target angle
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }

    // ============================================
    // SESSION 2: Body Segments with List
    // ============================================

    void InitializeBody()
    {
        // TEACHING: for loop creates starting segments
        // Notice how the index variable 'i' counts from 0 to startingSegments-1
        for (int i = 0; i < startingSegments; i++)
        {
            AddSegment();
        }
    }

    void AddSegment()
    {
        // TEACHING: Instantiate creates a new GameObject from a prefab
        GameObject newSegmentObj = Instantiate(segmentPrefab, transform.position, Quaternion.identity);

        // TEACHING: GetComponent<T>() retrieves a specific script from a GameObject
        SnakeSegment segment = newSegmentObj.GetComponent<SnakeSegment>();

        // TEACHING: List.Add() puts the new segment at the end of the list
        bodySegments.Add(segment);
    }

    // SESSION 2: Simple version - each segment follows the one in front
    // SESSION 3: Enhanced version - segments follow positions from queue
    void UpdateBodySegments()
    {
        // TEACHING: foreach loop - clean way to iterate through all items in a list
        // We don't need the index here, just access to each segment
        int index = 0;
        foreach (SnakeSegment segment in bodySegments)
        {
            // Session 3: Get historical position based on segment index
            Vector3 targetPos = GetHistoricalPosition(index);
            segment.FollowPosition(targetPos);
            index++;
        }
    }

    // ============================================
    // SESSION 3: Queue-Based Following
    // ============================================

    void RecordPosition()
    {
        // TEACHING: Enqueue adds to the back of the queue
        // Think of a queue like a line at a store - first in, first out
        positionHistory.Enqueue(transform.position);

        // TEACHING: while loop keeps running as long as condition is true
        // This prevents the queue from growing infinitely large
        while (positionHistory.Count > maxHistorySize)
        {
            // TEACHING: Dequeue removes from the front of the queue
            positionHistory.Dequeue();
        }
    }

    Vector3 GetHistoricalPosition(int segmentIndex)
    {
        // TEACHING: Calculate which position in history this segment should follow
        // Segments further back follow older positions
        int spacing = 5; // How many history entries between segments
        int historyIndex = segmentIndex * spacing;

        // Make sure we don't go beyond available history
        if (historyIndex >= positionHistory.Count)
        {
            historyIndex = positionHistory.Count - 1;
        }

        // TEACHING: Convert Queue to array to access by index
        // Queues don't support direct indexing, but arrays do
        Vector3[] historyArray = positionHistory.ToArray();
        return historyArray[historyIndex];
    }

    // ============================================
    // SESSION 4: Growth (Food Collection)
    // ============================================

    public void Grow(int amount)
    {
        // TEACHING: for loop adds multiple segments
        for (int i = 0; i < amount; i++)
        {
            AddSegment();
        }
    }

    // TEACHING: OnTriggerEnter2D is called automatically by Unity when colliders overlap
    // This requires: 1) Both objects have Collider2D, 2) At least one is a Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // SESSION 4: Food collection
        if (other.CompareTag("Food"))
        {
            FoodPellet food = other.GetComponent<FoodPellet>();
            if (food != null)
            {
                Grow(1);
                // Food pellet will handle its own destruction and respawning
            }
        }

        // SESSION 6: AI collision
        if (other.CompareTag("AISnake"))
        {
            Die();
        }
    }

    // ============================================
    // SESSION 5: Collision Detection
    // ============================================

    void CheckSelfCollision()
    {
        // TEACHING: for loop checks head against each body segment
        // We start at index 3 to skip segments too close to the head
        for (int i = 3; i < bodySegments.Count; i++)
        {
            // Calculate distance between head and this segment
            float distance = Vector3.Distance(transform.position, bodySegments[i].transform.position);

            if (distance < collisionDistance)
            {
                Die();
                // TEACHING: break exits the loop early - no need to check remaining segments
                break;
            }
        }
    }

    void CheckBoundaries()
    {
        // TEACHING: Check if snake is within playable area
        if (!BoundsHelper.IsInBounds(transform.position))
        {
            // Option 1: Wrap to opposite side (like Pac-Man)
            transform.position = BoundsHelper.WrapPosition(transform.position);

            // Option 2: Die (uncomment to use instead)
            // Die();
        }
    }

    void Die()
    {
        isAlive = false;
        Debug.Log("Player died! Score: " + bodySegments.Count);

        // Notify GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }

    // ============================================
    // HELPER METHODS
    // ============================================

    // For debugging - visualize collision range in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collisionDistance);
    }
}
