/*
 * ╔════════════════════════════════════════════════════════════════════════════╗
 * ║  GAME 220: Slither.io Template                                             ║
 * ║  FoodPellet.cs - Individual Food Item                                      ║
 * ╚════════════════════════════════════════════════════════════════════════════╝
 *
 * LEARNING OBJECTIVES:
 * After studying this file, you will understand:
 * - How collision detection works with OnTriggerEnter2D
 * - The difference between Triggers and Colliders
 * - How to use CompareTag for efficient object identification
 * - Component communication patterns (notifying other scripts)
 *
 * DATA STRUCTURES USED: None
 * LOOPS USED: None
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * WHAT DOES THIS SCRIPT DO?
 * ══════════════════════════
 * Each food pellet in the game has this script attached.
 * When a snake (player or AI) touches the food:
 * 1. The snake grows longer
 * 2. The score increases (if player)
 * 3. This food is destroyed and a new one spawns elsewhere
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * COLLISION DETECTION FLOW:
 * ══════════════════════════
 *
 *    [Snake Head]  ───touches───>  [Food Pellet]
 *         │                             │
 *         │                             ▼
 *         │                   OnTriggerEnter2D fires!
 *         │                             │
 *         │                             ▼
 *         │                   Is it a "Player" or "AISnake"?
 *         │                        /           \
 *         ▼                      YES            NO
 *    player.Grow(1)              │              │
 *         │                      ▼              ▼
 *         │              Get snake component   Do nothing
 *         │                      │
 *         ▼                      ▼
 *    Score +10            snake.Grow(1)
 *         │                      │
 *         │                      ▼
 *         └─────────> spawner.OnFoodCollected(this)
 *                               │
 *                               ▼
 *                     Pellet destroyed, new one spawns
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * TRIGGER vs COLLIDER - What's the difference?
 * ═════════════════════════════════════════════
 *
 * COLLIDER (Is Trigger = OFF):
 * ────────────────────────────
 * - Objects physically BOUNCE off each other
 * - Uses OnCollisionEnter2D
 * - Good for: walls, platforms, physical objects
 *
 *    [Ball]  ──bounce!──>  |Wall|
 *                          |    |
 *
 * TRIGGER (Is Trigger = ON):
 * ──────────────────────────
 * - Objects PASS THROUGH each other
 * - Uses OnTriggerEnter2D
 * - Good for: pickups, detection zones, power-ups
 *
 *    [Snake] ──passes through──> (Food)  --> Food collected!
 *
 * For food collection, we use TRIGGERS because:
 * - We don't want the snake to bounce off food
 * - We just want to DETECT when they overlap
 * - The food disappears, it doesn't push the snake
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 */

using UnityEngine;

public class FoodPellet : MonoBehaviour
{
    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  SETTINGS - Adjustable in Unity Inspector                                 ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    [Header("Pellet Settings")]
    /*
     * How many segments the snake gains when eating this food.
     *
     * ╔═══════════════════════════════════════════════════════════════════════════╗
     * ║  TRY IT: Experiment with Growth Amount                                    ║
     * ╠═══════════════════════════════════════════════════════════════════════════╣
     * ║  1. Set growthAmount = 5                                                  ║
     * ║     Eating one food adds 5 segments - snake grows FAST!                   ║
     * ║                                                                           ║
     * ║  2. Set growthAmount = 0                                                  ║
     * ║     Food gives score but no growth - "diet mode"                          ║
     * ║                                                                           ║
     * ║  CHALLENGE: Create "super food" with growthAmount = 10                    ║
     * ║  How would you make it spawn rarely? (Hint: modify FoodSpawner)           ║
     * ╚═══════════════════════════════════════════════════════════════════════════╝
     */
    public int growthAmount = 1;

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  PRIVATE VARIABLES - References to other scripts                          ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * Reference to the FoodSpawner that created this pellet.
     *
     * WHY DO WE NEED THIS?
     * When this food is collected, we need to tell the spawner:
     * 1. "Remove me from your tracking list"
     * 2. "Spawn a new food to replace me"
     *
     * This is called "component communication" - scripts talking to each other.
     */
    private FoodSpawner spawner;

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  PUBLIC METHOD - Called by FoodSpawner when creating this pellet          ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * SetSpawner is called right after this pellet is created.
     * The spawner passes a reference to itself so we can call back later.
     *
     * COMMUNICATION PATTERN:
     * 1. FoodSpawner creates FoodPellet using Instantiate()
     * 2. FoodSpawner calls pellet.SetSpawner(this) to give us a reference
     * 3. Later, when collected, we call spawner.OnFoodCollected(this)
     *
     * This creates a two-way communication link:
     *
     *    FoodSpawner <──────────────> FoodPellet
     *         │                           │
     *         │  "Here's my reference"    │
     *         │  (SetSpawner)             │
     *         │ ─────────────────────────>│
     *         │                           │
     *         │  "I was collected!"       │
     *         │  (OnFoodCollected)        │
     *         │<───────────────────────── │
     */
    public void SetSpawner(FoodSpawner foodSpawner)
    {
        spawner = foodSpawner;
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  COLLISION DETECTION - The heart of food collection                       ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * OnTriggerEnter2D is called AUTOMATICALLY by Unity's physics system
     * when another object's collider overlaps with this object's trigger.
     *
     * ════════════════════════════════════════════════════════════════════════════
     * REQUIREMENTS FOR OnTriggerEnter2D TO WORK:
     * ════════════════════════════════════════════════════════════════════════════
     *
     * 1. BOTH objects must have a Collider2D component
     *    - Snake head has CircleCollider2D
     *    - Food pellet has CircleCollider2D
     *
     * 2. At least ONE must have "Is Trigger" checked
     *    - Food pellet has Is Trigger = ON
     *
     * 3. At least ONE must have a Rigidbody2D component
     *    - Snake head has Rigidbody2D (for physics)
     *
     * If ANY of these are missing, OnTriggerEnter2D will NOT be called!
     * This is a common bug source - always check these requirements.
     * ════════════════════════════════════════════════════════════════════════════
     *
     * The "other" parameter is the Collider2D that entered our trigger.
     * We use it to find out WHAT touched us and get its components.
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        // ════════════════════════════════════════════════════════════════════════
        // CHECK #1: Did a PLAYER snake touch us?
        // ════════════════════════════════════════════════════════════════════════
        /*
         * CompareTag("Player") checks if the other object's tag is "Player".
         *
         * WHY USE CompareTag INSTEAD OF == ?
         * ───────────────────────────────────
         * BAD:  if (other.gameObject.tag == "Player")
         *       - Creates garbage (memory allocation)
         *       - Slightly slower
         *
         * GOOD: if (other.CompareTag("Player"))
         *       - No garbage created
         *       - Faster comparison
         *
         * For something called every collision, this optimization matters!
         */
        if (other.CompareTag("Player"))
        {
            /*
             * GetComponent<T>() retrieves a specific script from a GameObject.
             *
             * "other" is a Collider2D (the component that touched us)
             * "other.gameObject" would give us the GameObject
             * GetComponent<PlayerSnakeController>() finds the snake script on it
             *
             * This returns null if the component doesn't exist, so we check.
             */
            PlayerSnakeController player = other.GetComponent<PlayerSnakeController>();

            if (player != null)
            {
                // Tell the player snake to grow
                // This calls PlayerSnakeController.Grow() which adds segments
                player.Grow(growthAmount);

                // Notify the spawner so it can:
                // 1. Remove us from its tracking list
                // 2. Destroy our GameObject
                // 3. Spawn a new food to replace us
                if (spawner != null)
                {
                    spawner.OnFoodCollected(this);
                }
                else
                {
                    // Fallback: if no spawner reference, just destroy ourselves
                    // This might happen if food was created without going through spawner
                    Destroy(gameObject);
                }

                // Update the score
                // FindAnyObjectByType searches the scene for a ScoreManager
                // This is slower than caching a reference, but simpler for teaching
                ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
                if (scoreManager != null)
                {
                    scoreManager.AddScore(10);
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // CHECK #2: Did an AI snake touch us?
        // ════════════════════════════════════════════════════════════════════════
        /*
         * AI snakes can also eat food - they just don't add to the player's score.
         * This uses the exact same pattern as above.
         */
        if (other.CompareTag("AISnake"))
        {
            AISnakeController ai = other.GetComponent<AISnakeController>();

            if (ai != null)
            {
                // AI snakes grow too
                ai.Grow(growthAmount);

                // Notify spawner (same cleanup process)
                if (spawner != null)
                {
                    spawner.OnFoodCollected(this);
                }
                else
                {
                    Destroy(gameObject);
                }

                // Note: We don't add score for AI eating food
                // Only the player's food collection counts
            }
        }
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  VISUAL EFFECTS - Optional bobbing animation                              ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * Update is used here for a simple visual effect - making food "bob" up and down.
     * This is purely cosmetic and doesn't affect gameplay.
     *
     * WHY UPDATE INSTEAD OF FIXEDUPDATE?
     * ───────────────────────────────────
     * This is just a visual effect, not physics.
     * Update runs every frame, giving the smoothest visual.
     * FixedUpdate would make the bobbing look choppy at high frame rates.
     */
    void Update()
    {
        /*
         * SINE WAVE BOBBING EFFECT:
         * ─────────────────────────
         * Mathf.Sin() returns values between -1 and 1 in a wave pattern.
         * We use Time.time to make it continuous (time keeps increasing).
         *
         * Mathf.Sin(Time.time * bobSpeed):
         * - Time.time increases each frame
         * - Multiplied by bobSpeed to control how fast it oscillates
         * - Returns a value that smoothly goes from -1 to 1 and back
         *
         * bobAmount * Time.deltaTime:
         * - bobAmount controls how big the movement is
         * - Time.deltaTime makes it frame-rate independent
         */
        float bobAmount = 0.1f;    // How far up/down to bob
        float bobSpeed = 2f;       // How fast to bob

        // Calculate new Y position using sine wave
        float newY = transform.position.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount * Time.deltaTime;

        // Update only the Y position, keep X and Z the same
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

/*
 * ╔═══════════════════════════════════════════════════════════════════════════════╗
 * ║  SUMMARY - What You Learned                                                   ║
 * ╠═══════════════════════════════════════════════════════════════════════════════╣
 * ║                                                                               ║
 * ║  1. OnTriggerEnter2D - Detect when objects overlap                            ║
 * ║     - Requires: Collider2D on both, "Is Trigger" on at least one              ║
 * ║     - Requires: Rigidbody2D on at least one object                            ║
 * ║                                                                               ║
 * ║  2. Trigger vs Collider                                                       ║
 * ║     - Trigger: Objects pass through, just detects overlap                     ║
 * ║     - Collider: Objects bounce off each other physically                      ║
 * ║                                                                               ║
 * ║  3. CompareTag - Fast way to check object type                                ║
 * ║     - Faster and cleaner than string comparison                               ║
 * ║     - Tags are set in Unity Inspector                                         ║
 * ║                                                                               ║
 * ║  4. Component Communication                                                   ║
 * ║     - Scripts can call methods on other scripts                               ║
 * ║     - Pass references to enable two-way communication                         ║
 * ║                                                                               ║
 * ║  NEXT FILE: ScoreManager.cs - Learn about UI and event-driven updates!        ║
 * ║                                                                               ║
 * ╚═══════════════════════════════════════════════════════════════════════════════╝
 */
