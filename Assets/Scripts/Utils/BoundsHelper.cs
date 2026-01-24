/*
 * ╔════════════════════════════════════════════════════════════════════════════╗
 * ║  GAME 220: Slither.io Template                                             ║
 * ║  BoundsHelper.cs - Static Utility Class for Boundaries                     ║
 * ╚════════════════════════════════════════════════════════════════════════════╝
 *
 * LEARNING OBJECTIVES:
 * After studying this file, you will understand:
 * - What "static" means and when to use it
 * - Utility/helper class pattern
 * - Boundary checking and position wrapping
 * - Mathf.Clamp and other math helpers
 *
 * DATA STRUCTURES USED: None
 * LOOPS USED: None
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * WHAT IS A STATIC CLASS?
 * ═══════════════════════
 *
 * A "static" class is a class that:
 * - You DON'T create instances of (no "new BoundsHelper()")
 * - You DON'T attach to GameObjects in Unity
 * - You call methods directly on the class name
 *
 * REGULAR CLASS (like PlayerSnakeController):
 * ─────────────────────────────────────────────
 *     // You need an instance (a specific object)
 *     PlayerSnakeController player = FindObjectOfType<PlayerSnakeController>();
 *     player.Grow(1);  // Call method on the instance
 *
 * STATIC CLASS (like BoundsHelper):
 * ─────────────────────────────────
 *     // No instance needed - call directly on the class name
 *     bool isInside = BoundsHelper.IsInBounds(position);
 *     Vector3 wrapped = BoundsHelper.WrapPosition(position);
 *
 * WHEN TO USE STATIC:
 * ───────────────────
 * - Utility functions that don't need to remember anything
 * - Math helpers
 * - Constants and shared data
 * - Functions that work the same way every time
 *
 * BoundsHelper is perfect for static because:
 * - The bounds are the same for everything in the game
 * - These are just calculations, no state to remember
 * - Every script needs to use the same boundary rules
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * POSITION WRAPPING vs CLAMPING:
 * ═══════════════════════════════
 *
 * WRAPPING (Like Pac-Man):
 * ────────────────────────
 * When you exit one side, you appear on the opposite side.
 * Used in this game for snakes.
 *
 *    ┌─────────────────────────────────────────────────┐
 *    │                                                 │
 *    │   Play Area                                     │
 *    │                                                 │
 *    │         ●────────────────────────────────────>  │  (Snake exits right)
 *    │                                                 │
 *  > ●                                                 │  (Snake appears left!)
 *    │                                                 │
 *    └─────────────────────────────────────────────────┘
 *
 *
 * CLAMPING (Like hitting a wall):
 * ─────────────────────────────────
 * When you reach the edge, you stop. You can't go further.
 *
 *    ┌─────────────────────────────────────────────────┐
 *    │                                                 │
 *    │   Play Area                                     │
 *    │                                                 │
 *    │         ●────────────────────────────────────>│ │  (Snake reaches edge)
 *    │                                               │ │
 *    │                                               ●─│  (Snake stops at edge)
 *    │                                                 │
 *    └─────────────────────────────────────────────────┘
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

/*
 * "public static class" means:
 * - public: Any script can use it
 * - static: No instances, call methods directly on the class name
 * - class: It's a class (contains methods and data)
 */
public static class BoundsHelper
{
    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  BOUNDARY DEFINITIONS - The edges of the play area                        ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * STATIC FIELDS:
     * ──────────────
     * These are "shared" values - the same for everyone who uses this class.
     * When you change minBounds, EVERY script sees the new value.
     *
     * Think of it like a shared whiteboard:
     * - Regular variable: Each person has their own notepad
     * - Static variable: Everyone looks at the same whiteboard
     *
     * The play area is a rectangle:
     *
     *          minBounds.y = -15 (bottom edge)
     *                │
     *    ┌───────────┴───────────┐
     *    │                       │
     *    │      PLAY AREA        │  minBounds.x = -25 (left edge)
     *    │                       │  maxBounds.x = 25 (right edge)
     *    │                       │
     *    └───────────┬───────────┘
     *                │
     *          maxBounds.y = 15 (top edge)
     *
     * Total size: 50 units wide x 30 units tall
     */
    // Bounds sized to match a camera with orthographic size 10 (16:9 aspect)
    // This keeps all game action visible on screen
    public static Vector2 minBounds = new Vector2(-17, -10);
    public static Vector2 maxBounds = new Vector2(17, 10);

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  BOUNDARY CHECKING - Is a position inside the play area?                  ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * IsInBounds checks if a position is inside the play area.
     *
     * USAGE:
     *     if (BoundsHelper.IsInBounds(transform.position))
     *     {
     *         // Position is inside - do nothing
     *     }
     *     else
     *     {
     *         // Position is outside - wrap or clamp!
     *     }
     *
     * RETURNS:
     *     true  = position is INSIDE the bounds
     *     false = position is OUTSIDE the bounds
     */
    public static bool IsInBounds(Vector3 position)
    {
        /*
         * THE && OPERATOR (AND):
         * ──────────────────────
         * All four conditions must be true for the whole thing to be true.
         *
         * position.x >= minBounds.x   →  Not too far LEFT
         *              AND
         * position.x <= maxBounds.x   →  Not too far RIGHT
         *              AND
         * position.y >= minBounds.y   →  Not too far DOWN
         *              AND
         * position.y <= maxBounds.y   →  Not too far UP
         *
         * If ANY of these is false, the position is out of bounds.
         */
        return position.x >= minBounds.x && position.x <= maxBounds.x &&
               position.y >= minBounds.y && position.y <= maxBounds.y;
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  POSITION WRAPPING - Pac-Man style teleportation                          ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * WrapPosition moves a position to the opposite edge if it goes out of bounds.
     *
     * USAGE:
     *     transform.position = BoundsHelper.WrapPosition(transform.position);
     *
     * This is the classic "Pac-Man" behavior where going off one edge
     * makes you appear on the other side.
     *
     * ╔═══════════════════════════════════════════════════════════════════════════╗
     * ║  TRY IT: Test the wrapping                                                ║
     * ╠═══════════════════════════════════════════════════════════════════════════╣
     * ║  1. Move your snake to the right edge of the screen                       ║
     * ║  2. Watch what happens when you cross the edge                            ║
     * ║  3. Try the same with top/bottom/left edges                               ║
     * ║                                                                           ║
     * ║  CHALLENGE: Change PlayerSnakeController to use ClampPosition instead     ║
     * ║  What happens when you hit an edge now?                                   ║
     * ╚═══════════════════════════════════════════════════════════════════════════╝
     */
    public static Vector3 WrapPosition(Vector3 position)
    {
        // Start with the current position
        Vector3 wrappedPosition = position;

        // ════════════════════════════════════════════════════════════════════════
        // CHECK HORIZONTAL EDGES (LEFT and RIGHT)
        // ════════════════════════════════════════════════════════════════════════

        // Too far LEFT? Teleport to RIGHT edge
        if (position.x < minBounds.x)
        {
            wrappedPosition.x = maxBounds.x;
        }
        // Too far RIGHT? Teleport to LEFT edge
        else if (position.x > maxBounds.x)
        {
            wrappedPosition.x = minBounds.x;
        }

        // ════════════════════════════════════════════════════════════════════════
        // CHECK VERTICAL EDGES (TOP and BOTTOM)
        // ════════════════════════════════════════════════════════════════════════

        // Too far DOWN? Teleport to TOP edge
        if (position.y < minBounds.y)
        {
            wrappedPosition.y = maxBounds.y;
        }
        // Too far UP? Teleport to BOTTOM edge
        else if (position.y > maxBounds.y)
        {
            wrappedPosition.y = minBounds.y;
        }

        return wrappedPosition;
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  POSITION CLAMPING - Stop at the edge (alternative to wrapping)           ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * ClampPosition keeps a position INSIDE the bounds by stopping at edges.
     *
     * USAGE:
     *     transform.position = BoundsHelper.ClampPosition(transform.position);
     *
     * Unlike wrapping, this doesn't teleport - it just prevents going further.
     * Like hitting an invisible wall.
     */
    public static Vector3 ClampPosition(Vector3 position)
    {
        /*
         * MATHF.CLAMP - Force a value into a range:
         * ─────────────────────────────────────────
         * Mathf.Clamp(value, min, max)
         *
         * - If value < min, returns min
         * - If value > max, returns max
         * - Otherwise, returns value unchanged
         *
         * Example:
         *     Mathf.Clamp(50, 0, 100)  →  50 (within range)
         *     Mathf.Clamp(-10, 0, 100) →  0  (below min, return min)
         *     Mathf.Clamp(200, 0, 100) →  100 (above max, return max)
         */
        return new Vector3(
            Mathf.Clamp(position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(position.y, minBounds.y, maxBounds.y),
            0  // Keep Z at 0 for 2D
        );
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  RANDOM POSITIONS - Spawn things at random locations                      ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * GetRandomPosition returns a random position inside the play area.
     *
     * USAGE:
     *     Vector3 spawnPoint = BoundsHelper.GetRandomPosition();
     *     Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
     *
     * This is useful for spawning food, enemies, or anything that
     * needs to appear at a random location.
     */
    public static Vector3 GetRandomPosition()
    {
        /*
         * RANDOM.RANGE - Get a random number in a range:
         * ──────────────────────────────────────────────
         * Random.Range(min, max)
         *
         * For floats: Returns any value between min and max (inclusive)
         * For ints: Returns min to max-1 (max is exclusive)
         *
         * Example:
         *     Random.Range(0f, 10f)  →  Could be 0.0, 3.7, 9.99, etc.
         *     Random.Range(0, 10)    →  Could be 0, 1, 2... up to 9 (not 10!)
         */
        return new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            0
        );
    }

    /*
     * GetRandomPositionInArea is like GetRandomPosition but within a smaller area.
     *
     * USAGE:
     *     // Spawn in the left half of the screen
     *     Vector3 pos = BoundsHelper.GetRandomPositionInArea(
     *         new Vector2(-25, -15),  // Left corner
     *         new Vector2(0, 15)      // Middle-right corner
     *     );
     *
     * This also ensures the area stays within the overall bounds.
     */
    public static Vector3 GetRandomPositionInArea(Vector2 areaMin, Vector2 areaMax)
    {
        /*
         * MATHF.MAX and MATHF.MIN - Constrain the area to game bounds:
         * ─────────────────────────────────────────────────────────────
         * Mathf.Max(a, b) returns the LARGER value
         * Mathf.Min(a, b) returns the SMALLER value
         *
         * We use these to make sure the spawn area doesn't exceed game bounds:
         * - If areaMin is below minBounds, use minBounds instead
         * - If areaMax is above maxBounds, use maxBounds instead
         */
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

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  DISTANCE CALCULATIONS - How far from the edge?                           ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * GetDistanceToBounds returns how far a position is from the NEAREST edge.
     *
     * USAGE:
     *     float distance = BoundsHelper.GetDistanceToBounds(transform.position);
     *     if (distance < 3)
     *     {
     *         // We're getting close to an edge!
     *         // Maybe show a warning or turn around
     *     }
     *
     * This is useful for:
     * - AI that should avoid edges
     * - Warning indicators when near boundaries
     * - Gradual effects as you approach edges
     */
    public static float GetDistanceToBounds(Vector3 position)
    {
        // Calculate distance to each edge
        float distanceToLeft = position.x - minBounds.x;
        float distanceToRight = maxBounds.x - position.x;
        float distanceToBottom = position.y - minBounds.y;
        float distanceToTop = maxBounds.y - position.y;

        /*
         * MATHF.MIN WITH MULTIPLE VALUES:
         * ───────────────────────────────
         * Find the smallest of all four distances.
         * That's the distance to the nearest edge.
         */
        return Mathf.Min(
            distanceToLeft,
            distanceToRight,
            distanceToBottom,
            distanceToTop
        );
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  SIZE CALCULATIONS - Information about the play area                      ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * GetBoundsSize returns the width and height of the play area.
     *
     * USAGE:
     *     Vector2 size = BoundsHelper.GetBoundsSize();
     *     Debug.Log($"Play area is {size.x} wide and {size.y} tall");
     *     // Output: "Play area is 50 wide and 30 tall"
     */
    public static Vector2 GetBoundsSize()
    {
        return new Vector2(
            maxBounds.x - minBounds.x,  // Width: 25 - (-25) = 50
            maxBounds.y - minBounds.y   // Height: 15 - (-15) = 30
        );
    }

    /*
     * GetBoundsCenter returns the center point of the play area.
     *
     * USAGE:
     *     Vector3 center = BoundsHelper.GetBoundsCenter();
     *     // Good for spawning player at the center of the arena
     */
    public static Vector3 GetBoundsCenter()
    {
        return new Vector3(
            (minBounds.x + maxBounds.x) / 2,  // (-25 + 25) / 2 = 0
            (minBounds.y + maxBounds.y) / 2,  // (-15 + 15) / 2 = 0
            0
        );
    }
}

/*
 * ╔═══════════════════════════════════════════════════════════════════════════════╗
 * ║  SUMMARY - What You Learned                                                   ║
 * ╠═══════════════════════════════════════════════════════════════════════════════╣
 * ║                                                                               ║
 * ║  1. Static Classes                                                            ║
 * ║     - No instances needed - call methods directly on class name               ║
 * ║     - Perfect for utility functions and shared data                           ║
 * ║     - Usage: BoundsHelper.IsInBounds(position)                                ║
 * ║                                                                               ║
 * ║  2. Position Wrapping vs Clamping                                             ║
 * ║     - Wrapping: Teleport to opposite side (Pac-Man style)                     ║
 * ║     - Clamping: Stop at the edge (wall style)                                 ║
 * ║                                                                               ║
 * ║  3. Math Helpers                                                              ║
 * ║     - Mathf.Clamp: Force value into a range                                   ║
 * ║     - Mathf.Min/Max: Find smallest/largest value                              ║
 * ║     - Random.Range: Get random number in a range                              ║
 * ║                                                                               ║
 * ║  4. Boolean Logic                                                             ║
 * ║     - && (AND): All conditions must be true                                   ║
 * ║     - Useful for checking multiple bounds at once                             ║
 * ║                                                                               ║
 * ║  NEXT FILE: PlayerSnakeController.cs - The main event!                        ║
 * ║  Now you'll learn about Lists, Queues, and all types of Loops!                ║
 * ║                                                                               ║
 * ╚═══════════════════════════════════════════════════════════════════════════════╝
 */
