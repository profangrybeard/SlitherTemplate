/*
 * AISnakeController.cs
 *
 * LEARNING FOCUS: Code Reuse
 *
 * This script uses the SAME patterns as PlayerSnakeController:
 * - Same List<SnakeSegment> for body tracking
 * - Same for loop for creating body
 * - Same foreach loop for cleanup
 *
 * The ONLY difference: direction comes from a timer, not mouse input.
 * This demonstrates how patterns are reusable across different contexts.
 */

using System.Collections.Generic;
using UnityEngine;

public class AISnakeController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 150f;

    [Header("Body")]
    public int startingSegments = 3;
    public GameObject segmentPrefab;
    public float segmentSpacing = 0.5f;

    [Header("AI Behavior")]
    public float directionChangeInterval = 2f;

    private List<SnakeSegment> bodySegments = new List<SnakeSegment>();
    private Vector3 moveDirection;
    private float directionChangeTimer;
    private bool isAlive = true;

    void Start()
    {
        ChangeDirection();
        directionChangeTimer = directionChangeInterval;
        CreateStartingBody();
    }

    void Update()
    {
        if (!isAlive) return;

        // AI decides direction via timer instead of mouse input
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
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

    void ChangeDirection()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        moveDirection = new Vector3(randomDir.x, randomDir.y, 0);
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

    // Same pattern as PlayerSnakeController
    void CreateStartingBody()
    {
        for (int i = 0; i < startingSegments; i++)
        {
            AddSegment();
        }
    }

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

    public void Grow(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddSegment();
        }
    }

    void CheckBoundaries()
    {
        if (!BoundsHelper.IsInBounds(transform.position))
        {
            Vector3 oldPosition = transform.position;
            transform.position = BoundsHelper.WrapPosition(transform.position);
            Vector3 teleportOffset = transform.position - oldPosition;

            foreach (SnakeSegment segment in bodySegments)
            {
                if (segment != null)
                {
                    segment.Teleport(teleportOffset);
                }
            }

            ChangeDirection();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }

    void Die()
    {
        if (!isAlive) return;
        isAlive = false;

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnAISnakeDied(this);
        }

        foreach (SnakeSegment segment in bodySegments)
        {
            if (segment != null)
            {
                Destroy(segment.gameObject);
            }
        }

        Destroy(gameObject);
    }

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
