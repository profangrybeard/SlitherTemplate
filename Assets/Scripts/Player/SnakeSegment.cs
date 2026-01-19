/*
 * GAME 220: Slither.io Template
 * Session 2-3: Snake Segment
 *
 * TEACHING FOCUS:
 * - Component communication (controller tells segment where to go)
 * - Vector3.Lerp for smooth movement
 * - FixedUpdate for consistent physics
 *
 * This script makes a segment smoothly follow an assigned position.
 * The PlayerSnakeController (or AISnakeController) tells each segment
 * where it should be, and the segment smoothly moves there.
 */

using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    [Header("Following Settings")]
    // TEACHING: Higher value = faster following (more responsive but less smooth)
    // Lower value = slower following (more smooth but more laggy)
    public float followSpeed = 10f;

    // The position this segment is trying to reach
    private Vector3 targetPosition;

    void Start()
    {
        // TEACHING: Initialize target to current position
        // This prevents the segment from "jumping" on first frame
        targetPosition = transform.position;
    }

    // TEACHING: This method is called by the snake controller each FixedUpdate
    // It tells this segment where it should be positioned
    public void FollowPosition(Vector3 position)
    {
        targetPosition = position;
    }

    // TEACHING: FixedUpdate for consistent movement timing
    // This runs at the same time as the controller's FixedUpdate
    void FixedUpdate()
    {
        // TEACHING: Vector3.Lerp creates smooth interpolation between two positions
        // Lerp stands for "Linear Interpolation"
        // Formula: Lerp(start, end, t) = start + (end - start) * t
        // - t = 0 means stay at start
        // - t = 1 means move to end
        // - t = 0.5 means move halfway between

        // We use followSpeed * Time.fixedDeltaTime to make movement frame-rate independent
        float lerpAmount = followSpeed * Time.fixedDeltaTime;

        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpAmount);

        // TEACHING: Alternative approach without Lerp (more direct):
        // Vector3 direction = targetPosition - transform.position;
        // transform.position += direction * followSpeed * Time.fixedDeltaTime;
    }

    // ============================================
    // VISUALIZATION (for debugging)
    // ============================================

    // TEACHING: OnDrawGizmos draws debug visualization in Unity Editor
    // This helps us see what the segment is doing
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Draw a line from current position to target position
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPosition);

            // Draw a small sphere at target position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.1f);
        }
    }
}
