/*
 * PlayerSnakeController.cs
 *
 * LEARNING FOCUS: List<T> and Loops
 *
 * Key Concepts:
 * - List<T>: Dynamic collection that grows/shrinks
 * - for loop: Repeat code N times
 * - foreach loop: Process every item in a collection
 *
 * The snake body uses a "follow the leader" pattern:
 * HEAD → Seg0 → Seg1 → Seg2 → ...
 * Each segment follows the one ahead of it.
 */

using System.Collections.Generic;
using UnityEngine;

public class PlayerSnakeController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;

    [Header("Body")]
    public int startingSegments = 3;
    public GameObject segmentPrefab;
    public float segmentSpacing = 0.5f;

    // List<T> - A dynamic collection that can grow and shrink
    // We use it to track all body segments
    private List<SnakeSegment> bodySegments = new List<SnakeSegment>();

    private Vector3 moveDirection;
    private bool isAlive = true;

    void Start()
    {
        moveDirection = transform.up;
        CreateStartingBody();
    }

    void Update()
    {
        if (!isAlive) return;

        // Get mouse position and calculate direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        moveDirection = (mousePos - transform.position).normalized;
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        MoveHead();
        CheckBoundaries();
    }

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

    // FOR LOOP: Create starting body segments
    // Runs AddSegment() exactly startingSegments times
    void CreateStartingBody()
    {
        for (int i = 0; i < startingSegments; i++)
        {
            AddSegment();
        }
        Debug.Log($"Created snake with {bodySegments.Count} segments");
    }

    // Adds one segment to the end of the snake
    void AddSegment()
    {
        Vector3 spawnPosition;
        Transform targetToFollow;

        if (bodySegments.Count == 0)
        {
            // First segment follows the head
            spawnPosition = transform.position - transform.up * segmentSpacing;
            targetToFollow = transform;
        }
        else
        {
            // Additional segments follow the previous segment
            SnakeSegment lastSegment = bodySegments[bodySegments.Count - 1];
            spawnPosition = lastSegment.transform.position;
            targetToFollow = lastSegment.transform;
        }

        GameObject newSegmentObj = Instantiate(segmentPrefab, spawnPosition, Quaternion.identity);
        SnakeSegment newSegment = newSegmentObj.GetComponent<SnakeSegment>();

        if (newSegment != null)
        {
            // Tell this segment what to follow
            newSegment.SetTarget(targetToFollow, segmentSpacing);

            // Copy head color to segment
            SpriteRenderer headSprite = GetComponent<SpriteRenderer>();
            SpriteRenderer segmentSprite = newSegmentObj.GetComponent<SpriteRenderer>();
            if (headSprite != null && segmentSprite != null)
            {
                segmentSprite.color = headSprite.color;
            }

            // Tag and collider so AI can hit our body
            newSegmentObj.tag = "Player";
            CircleCollider2D collider = newSegmentObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            collider.isTrigger = true;

            // List.Add() - Adds item to end of list
            bodySegments.Add(newSegment);
        }
    }

    // Called by FoodPellet when snake eats food
    public void Grow(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddSegment();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AISnake"))
        {
            Die();
        }
    }

    void CheckBoundaries()
    {
        if (!BoundsHelper.IsInBounds(transform.position))
        {
            Vector3 oldPosition = transform.position;
            transform.position = BoundsHelper.WrapPosition(transform.position);
            Vector3 teleportOffset = transform.position - oldPosition;

            // FOREACH: Teleport all segments when head wraps
            foreach (SnakeSegment segment in bodySegments)
            {
                if (segment != null)
                {
                    segment.Teleport(teleportOffset);
                }
            }
        }
    }

    void Die()
    {
        isAlive = false;
        Debug.Log("Player died! Segments: " + bodySegments.Count);

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }

    // FOREACH LOOP: Clean up all segments when destroyed
    void OnDestroy()
    {
        foreach (SnakeSegment segment in bodySegments)
        {
            if (segment != null)
            {
                Destroy(segment.gameObject);
            }
        }
    }
}
