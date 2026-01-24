/*
 * SnakeSegment.cs
 *
 * LEARNING FOCUS: Following another object
 *
 * Key Concepts:
 * - Transform references to track another object
 * - Vector3.MoveTowards for smooth, constant-speed movement
 *
 * Each segment stores a reference to what it should follow.
 * The first segment follows the head, the second follows the first, etc.
 */

using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [Header("Following")]
    public float followDistance = 0.3f;
    public float moveSpeed = 20f;

    // The Transform of whatever is in front of us
    private Transform target;

    // Called once when the segment is created
    public void SetTarget(Transform newTarget, float distance)
    {
        target = newTarget;
        followDistance = distance;
    }

    // Called when head wraps around screen boundary
    public void Teleport(Vector3 offset)
    {
        transform.position += offset;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Calculate direction to target
        Vector3 direction = (target.position - transform.position).normalized;

        // Calculate ideal position (followDistance behind target)
        Vector3 targetPosition = target.position - direction * followDistance;

        // Move toward that position at constant speed
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );
    }
}
