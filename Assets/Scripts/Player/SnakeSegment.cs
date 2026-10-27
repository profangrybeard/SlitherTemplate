/*
 * GAME 220: Slither Template -- SnakeSegment
 * One body piece. It follows the Transform in front of it (the head, or the segment ahead)
 * with MoveTowards. PlayerSnakeController and AISnakeController keep these in a List.
 *
 * Read alongside: LOOPS_AND_LISTS_CONCEPTS.md section 2
 * Session 2: read SetTarget() and FixedUpdate()
 */

using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // SETTINGS
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Following")]
    public float followDistance = 0.3f;          // SetTarget() replaces this with the snake's segmentSpacing
    public float moveSpeed = 20f;                // World units per second

    // ─────────────────────────────────────────────────────────────────────────
    // STATE
    // ─────────────────────────────────────────────────────────────────────────

    private Transform target;                    // Whatever is in front of us: the head or the segment ahead

    // ─────────────────────────────────────────────────────────────────────────
    // PUBLIC METHODS - The snake controller calls these
    // ─────────────────────────────────────────────────────────────────────────

    // Called once, right after the segment is created
    public void SetTarget(Transform newTarget, float distance)
    {
        target = newTarget;
        followDistance = distance;
    }

    // Called when the head wraps around an edge, so the whole body jumps with it
    public void Teleport(Vector3 offset)
    {
        transform.position += offset;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // UNITY MESSAGES - Unity calls these automatically
    // ─────────────────────────────────────────────────────────────────────────

    void FixedUpdate()
    {
        // Why check for null before using target?
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;   // Toward the target
        Vector3 targetPosition = target.position - direction * followDistance;   // followDistance short of it

        // Why does a segment need a higher moveSpeed than the head? Set it below PlayerSnake's moveSpeed and press Play.
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );
    }
}
