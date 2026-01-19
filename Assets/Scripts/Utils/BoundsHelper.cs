/*
 * GAME 220: Slither.io Template
 * Session 5: Bounds Helper
 *
 * TEACHING FOCUS:
 * - Static methods (can be called without an instance)
 * - Helper/utility pattern
 * - Boundary checking logic
 * - Position wrapping (like Pac-Man)
 *
 * This script provides utility methods for managing play area boundaries:
 * - Check if a position is inside bounds
 * - Wrap position to opposite side
 * - Get random position within bounds
 */

using UnityEngine;

public static class BoundsHelper
{
    // TEACHING: Static fields are shared across all uses of this class
    // These define the rectangular play area
    public static Vector2 minBounds = new Vector2(-25, -15);
    public static Vector2 maxBounds = new Vector2(25, 15);

    // ============================================
    // BOUNDARY CHECKING
    // ============================================

    // TEACHING: Static methods can be called without creating an instance
    // Usage: BoundsHelper.IsInBounds(position)
    public static bool IsInBounds(Vector3 position)
    {
        // TEACHING: && means "AND" - all conditions must be true
        return position.x >= minBounds.x && position.x <= maxBounds.x &&
               position.y >= minBounds.y && position.y <= maxBounds.y;
    }

    // ============================================
    // POSITION WRAPPING
    // ============================================

    // TEACHING: Wrap position to opposite side (like Pac-Man)
    // If you go off the right edge, you appear on the left edge
    public static Vector3 WrapPosition(Vector3 position)
    {
        Vector3 wrappedPosition = position;

        // TEACHING: Check each edge and wrap if needed
        // Left edge
        if (position.x < minBounds.x)
        {
            wrappedPosition.x = maxBounds.x;
        }
        // Right edge
        else if (position.x > maxBounds.x)
        {
            wrappedPosition.x = minBounds.x;
        }

        // Bottom edge
        if (position.y < minBounds.y)
        {
            wrappedPosition.y = maxBounds.y;
        }
        // Top edge
        else if (position.y > maxBounds.y)
        {
            wrappedPosition.y = minBounds.y;
        }

        return wrappedPosition;
    }

    // ============================================
    // POSITION CLAMPING (alternative to wrapping)
    // ============================================

    // TEACHING: Clamp keeps position inside bounds (doesn't wrap)
    // If you hit an edge, you stop at that edge
    public static Vector3 ClampPosition(Vector3 position)
    {
        // TEACHING: Mathf.Clamp forces a value between min and max
        return new Vector3(
            Mathf.Clamp(position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(position.y, minBounds.y, maxBounds.y),
            0
        );
    }

    // ============================================
    // RANDOM POSITIONS
    // ============================================

    // TEACHING: Get a random position anywhere within bounds
    public static Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            0
        );
    }

    // TEACHING: Get a random position within a specific area of bounds
    public static Vector3 GetRandomPositionInArea(Vector2 areaMin, Vector2 areaMax)
    {
        // Make sure the area is within bounds
        areaMin.x = Mathf.Max(areaMin.x, minBounds.x);
        areaMin.y = Mathf.Max(areaMin.y, minBounds.y);
        areaMax.x = Mathf.Min(areaMax.x, maxBounds.x);
        areaMax.y = Mathf.Min(areaMax.y, maxBounds.y);

        return new Vector3(
            Random.Range(areaMin.x, areaMax.x),
            Random.Range(areaMin.y, areaMax.y),
            0
        );
    }

    // ============================================
    // DISTANCE CALCULATIONS
    // ============================================

    // TEACHING: Get distance to nearest boundary edge
    public static float GetDistanceToBounds(Vector3 position)
    {
        // Calculate distance to each edge
        float distanceToLeft = position.x - minBounds.x;
        float distanceToRight = maxBounds.x - position.x;
        float distanceToBottom = position.y - minBounds.y;
        float distanceToTop = maxBounds.y - position.y;

        // TEACHING: Mathf.Min finds the smallest value
        // Return distance to nearest edge
        return Mathf.Min(
            distanceToLeft,
            distanceToRight,
            distanceToBottom,
            distanceToTop
        );
    }

    // ============================================
    // SIZE CALCULATIONS
    // ============================================

    // TEACHING: Get the width and height of the play area
    public static Vector2 GetBoundsSize()
    {
        return new Vector2(
            maxBounds.x - minBounds.x,
            maxBounds.y - minBounds.y
        );
    }

    public static Vector3 GetBoundsCenter()
    {
        return new Vector3(
            (minBounds.x + maxBounds.x) / 2,
            (minBounds.y + maxBounds.y) / 2,
            0
        );
    }
}
