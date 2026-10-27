/*
 * GAME 220: Slither Template -- BoundsHelper (STATIC CLASS)
 * Shared play-area edges plus helpers to check, wrap, clamp, and pick random positions.
 * The snakes, FoodSpawner, and GameManager all call it, so the edges live in one place.
 *
 * Read alongside: LOOPS_AND_LISTS_CONCEPTS.md section 6
 * Session 5: read WrapPosition(), then try the wrap/clamp experiment (section 9)
 */

using UnityEngine;

// STATIC: a static class is never attached to a GameObject and never created with `new`.
//   You call its methods on the class name itself:
//       transform.position = BoundsHelper.WrapPosition(transform.position);
//   That's why it isn't a MonoBehaviour, and why it has no Start() or Update().
public static class BoundsHelper
{
    // ─────────────────────────────────────────────────────────────────────────
    // PLAY AREA EDGES - static, so every script shares the same values
    // ─────────────────────────────────────────────────────────────────────────

    // Sized to match a camera with orthographic size 10 (16:9 aspect), so all the action stays on screen
    public static Vector2 minBounds = new Vector2(-17, -10);
    public static Vector2 maxBounds = new Vector2(17, 10);

    // ─────────────────────────────────────────────────────────────────────────
    // BOUNDS CHECK
    // ─────────────────────────────────────────────────────────────────────────

    // true if the position is inside on all four sides. Why && and not ||?
    public static bool IsInBounds(Vector3 position)
    {
        return position.x >= minBounds.x && position.x <= maxBounds.x &&
               position.y >= minBounds.y && position.y <= maxBounds.y;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // WRAPPING - Pac-Man style
    // ─────────────────────────────────────────────────────────────────────────

    // TRY IT: steer off each edge, then swap in ClampPosition (see section 9)
    public static Vector3 WrapPosition(Vector3 position)
    {
        Vector3 wrappedPosition = position;      // Start with the current position

        // Too far LEFT? Teleport to the RIGHT edge. Too far RIGHT? Teleport to the LEFT edge.
        if (position.x < minBounds.x)
        {
            wrappedPosition.x = maxBounds.x;
        }
        else if (position.x > maxBounds.x)
        {
            wrappedPosition.x = minBounds.x;
        }

        // Too far DOWN? Teleport to the TOP. Too far UP? Teleport to the BOTTOM.
        // Why is this a new `if`, not one more `else if` on the chain above? Think about a corner.
        if (position.y < minBounds.y)
        {
            wrappedPosition.y = maxBounds.y;
        }
        else if (position.y > maxBounds.y)
        {
            wrappedPosition.y = minBounds.y;
        }

        return wrappedPosition;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CLAMPING - Stop at the wall
    // ─────────────────────────────────────────────────────────────────────────

    // Mathf.Clamp(value, min, max) forces each axis into the range
    public static Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(position.y, minBounds.y, maxBounds.y),
            0  // Keep Z at 0 for 2D
        );
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RANDOM POSITIONS
    // ─────────────────────────────────────────────────────────────────────────

    // Random point anywhere in the play area (float Random.Range includes both ends)
    public static Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            0
        );
    }

    // Random point in a smaller area, trimmed so it stays inside the play area
    public static Vector3 GetRandomPositionInArea(Vector2 areaMin, Vector2 areaMax)
    {
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

    // ─────────────────────────────────────────────────────────────────────────
    // DISTANCE AND SIZE
    // ─────────────────────────────────────────────────────────────────────────

    // Distance to the nearest edge (useful for AI edge avoidance). Nothing calls it yet.
    public static float GetDistanceToBounds(Vector3 position)
    {
        float distanceToLeft = position.x - minBounds.x;
        float distanceToRight = maxBounds.x - position.x;
        float distanceToBottom = position.y - minBounds.y;
        float distanceToTop = maxBounds.y - position.y;

        return Mathf.Min(                        // The smallest of the four
            distanceToLeft,
            distanceToRight,
            distanceToBottom,
            distanceToTop
        );
    }

    public static Vector2 GetBoundsSize()
    {
        return new Vector2(
            maxBounds.x - minBounds.x,  // Width: 17 - (-17) = 34
            maxBounds.y - minBounds.y   // Height: 10 - (-10) = 20
        );
    }

    public static Vector3 GetBoundsCenter()
    {
        return new Vector3(
            (minBounds.x + maxBounds.x) / 2,  // (-17 + 17) / 2 = 0
            (minBounds.y + maxBounds.y) / 2,  // (-10 + 10) / 2 = 0
            0
        );
    }
}
