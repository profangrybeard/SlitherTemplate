/*
 * ╔════════════════════════════════════════════════════════════════════════════════╗
 * ║                                                                                ║
 * ║                              GAME MANAGER                                      ║
 * ║                    List Cleanup & Game Coordination                            ║
 * ║                                                                                ║
 * ╚════════════════════════════════════════════════════════════════════════════════╝
 *
 * LEARNING OBJECTIVES:
 * After studying this file, you will understand:
 * - Managing multiple objects with a List
 * - for loops for spawning multiple entities
 * - foreach loops for cleanup operations
 * - List.Clear() to empty a list completely
 * - The difference between Destroy() and Clear()
 *
 * DATA STRUCTURES USED:
 * - List<AISnakeController> - Tracks all AI opponents
 *
 * LOOPS USED:
 * - for loop - Spawning multiple AI snakes
 * - foreach loop - Cleaning up all AI on game over
 *
 * ═══════════════════════════════════════════════════════════════════════════════════
 *
 *                          CLEANUP PATTERN DIAGRAM
 *
 * ═══════════════════════════════════════════════════════════════════════════════════
 *
 * When the game ends, we need to clean up ALL the AI snakes properly.
 * This requires TWO steps: Destroy the GameObjects AND clear the List.
 *
 * ┌───────────────────────────────────────────────────────────────────────────────────┐
 * │                                                                                   │
 * │  STEP 1: foreach loop - Destroy all AI GameObjects                                │
 * │  ──────────────────────────────────────────────────                               │
 * │                                                                                   │
 * │  Before cleanup:                                                                  │
 * │                                                                                   │
 * │  aiSnakes List:     [AI1]  [AI2]  [AI3]  [AI4]  [AI5]                            │
 * │                       │      │      │      │      │                               │
 * │                       ▼      ▼      ▼      ▼      ▼                               │
 * │  Game World:        🐍     🐍     🐍     🐍     🐍     (5 snakes in game)         │
 * │                                                                                   │
 * │  foreach (ai in aiSnakes) { Destroy(ai.gameObject); }                             │
 * │                                                                                   │
 * │  After Destroy:                                                                   │
 * │                                                                                   │
 * │  aiSnakes List:     [AI1]  [AI2]  [AI3]  [AI4]  [AI5]   (still has 5 items!)     │
 * │                       │      │      │      │      │                               │
 * │                       ▼      ▼      ▼      ▼      ▼                               │
 * │  Game World:        💀     💀     💀     💀     💀     (objects destroyed)        │
 * │                     null   null   null   null   null   (references are now null)  │
 * │                                                                                   │
 * │  ⚠️  PROBLEM: The List still has 5 items, but they point to nothing!              │
 * │  If we try to use these later, we'll get NullReferenceException!                  │
 * │                                                                                   │
 * └───────────────────────────────────────────────────────────────────────────────────┘
 *
 * ┌───────────────────────────────────────────────────────────────────────────────────┐
 * │                                                                                   │
 * │  STEP 2: List.Clear() - Empty the list                                            │
 * │  ─────────────────────────────────────                                            │
 * │                                                                                   │
 * │  Before Clear:                                                                    │
 * │                                                                                   │
 * │  aiSnakes List:     [null][null][null][null][null]    Count = 5 (BAD!)           │
 * │                                                                                   │
 * │  aiSnakes.Clear();                                                                │
 * │                                                                                   │
 * │  After Clear:                                                                     │
 * │                                                                                   │
 * │  aiSnakes List:     [ ]                                Count = 0 (GOOD!)          │
 * │                                                                                   │
 * │  ✓ Now the list is truly empty and safe to use again!                             │
 * │                                                                                   │
 * └───────────────────────────────────────────────────────────────────────────────────┘
 *
 * ═══════════════════════════════════════════════════════════════════════════════════
 *
 *                    WHY BOTH DESTROY AND CLEAR?
 *
 * ═══════════════════════════════════════════════════════════════════════════════════
 *
 *    Destroy(gameObject)          List.Clear()
 *    ──────────────────           ──────────────
 *         │                            │
 *         ▼                            ▼
 *    Removes object from           Removes references
 *    the GAME WORLD                from the LIST
 *    (visible snake disappears)    (list becomes empty)
 *
 *    If you ONLY Destroy:          If you ONLY Clear:
 *    ────────────────────          ────────────────────
 *    - Snakes disappear            - List is empty
 *    - But list still has items    - But snakes still exist in game!
 *    - Those items are null        - Memory leak, orphaned objects
 *    - NullReferenceException!     - Ghost snakes!
 *
 *    You need BOTH for proper cleanup!
 *
 * ═══════════════════════════════════════════════════════════════════════════════════
 */

using System.Collections.Generic;  // Required for List<T>
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  INSPECTOR SETTINGS                                                           ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    [Header("AI Settings")]
    public GameObject aiSnakePrefab;

    /*
     * ╔═══════════════════════════════════════════════════════════════════════════╗
     * ║  TRY IT: Change the number of AI snakes                                   ║
     * ╠═══════════════════════════════════════════════════════════════════════════╣
     * ║  1. Set numberOfAISnakes = 0  → No opponents, peaceful mode               ║
     * ║  2. Set numberOfAISnakes = 10 → Crowded arena, high challenge             ║
     * ║  3. Set numberOfAISnakes = 20 → Chaos mode!                               ║
     * ║                                                                           ║
     * ║  Watch the Console to see how many AI snakes spawn.                       ║
     * ╚═══════════════════════════════════════════════════════════════════════════╝
     */
    public int numberOfAISnakes = 3;

    [Header("Spawn Settings")]
    public Vector2 spawnAreaMin = new Vector2(-20, -15);
    public Vector2 spawnAreaMax = new Vector2(20, 15);

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  LIST DECLARATION - Tracking all AI snakes                                    ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    /*
     * This List tracks all AI snakes currently in the game.
     *
     * WHY TRACK AI SNAKES?
     * ────────────────────
     * 1. We need to clean them up when the game ends
     * 2. We can count how many exist: aiSnakes.Count
     * 3. Other scripts might need to know about all AI snakes
     *
     * Without this list, we'd have to use FindObjectsOfType<AISnakeController>()
     * every time we need to check all AI snakes - that's slow!
     */
    private List<AISnakeController> aiSnakes = new List<AISnakeController>();

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  GAME STATE                                                                   ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    /*
     * gameOver tracks whether the game has ended.
     * Once true, we accept restart input and don't run game logic.
     */
    private bool gameOver = false;

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  START - Initialize the game                                                  ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    void Start()
    {
        Debug.Log("Game started!");

        // Spawn all AI snakes using a FOR loop
        SpawnAISnakes();
    }

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  UPDATE - Check for restart input                                             ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    void Update()
    {
        // Only check for restart if game is over
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║                                                                               ║
    // ║                    FOR LOOP - SPAWNING AI SNAKES                              ║
    // ║                                                                               ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    /*
     * SpawnAISnakes uses a FOR loop to create multiple AI opponents.
     * This is the same pattern as spawning food or body segments!
     */
    void SpawnAISnakes()
    {
        /*
         * FOR LOOP - SPAWN MULTIPLE AI:
         * ─────────────────────────────
         *
         * for (int i = 0; i < numberOfAISnakes; i++)
         *          ─────   ─────────────────   ───
         *            │              │            │
         *            │              │            └── Add 1 each time
         *            │              │
         *            │              └── Keep going while i < 3
         *            │
         *            └── Start at 0
         *
         * EXECUTION TRACE (numberOfAISnakes = 3):
         * ────────────────────────────────────────
         *
         *   i=0: SpawnSingleAISnake() → aiSnakes = [AI1]          Count = 1
         *   i=1: SpawnSingleAISnake() → aiSnakes = [AI1][AI2]     Count = 2
         *   i=2: SpawnSingleAISnake() → aiSnakes = [AI1][AI2][AI3] Count = 3
         *   i=3: 3 < 3? FALSE → EXIT LOOP
         */
        for (int i = 0; i < numberOfAISnakes; i++)
        {
            SpawnSingleAISnake();
        }

        Debug.Log($"Spawned {aiSnakes.Count} AI snakes");
    }

    /*
     * SpawnSingleAISnake creates ONE AI snake and adds it to the list.
     * This is called by the for loop above.
     */
    void SpawnSingleAISnake()
    {
        // Get a random position for this AI
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Create the AI snake
        GameObject aiObj = Instantiate(aiSnakePrefab, spawnPosition, Quaternion.identity);

        // Get the AISnakeController component
        AISnakeController ai = aiObj.GetComponent<AISnakeController>();

        if (ai != null)
        {
            /*
             * LIST.ADD() - Track this AI snake
             * ────────────────────────────────
             * Same pattern as FoodSpawner and PlayerSnakeController!
             *
             * Before: [AI1][AI2]     Count = 2
             * Add(ai)
             * After:  [AI1][AI2][AI3] Count = 3
             */
            aiSnakes.Add(ai);
        }
    }

    /*
     * GetRandomSpawnPosition returns a random position within the game bounds.
     * Uses BoundsHelper to ensure AI always spawn inside the play area.
     */
    Vector3 GetRandomSpawnPosition()
    {
        // Use BoundsHelper to guarantee spawn is inside game bounds
        return BoundsHelper.GetRandomPosition();
    }

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║                                                                               ║
    // ║              FOREACH LOOP + CLEAR - GAME OVER CLEANUP                         ║
    // ║                                                                               ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    /*
     * GameOver is called when the player dies.
     * It handles cleaning up all AI snakes properly.
     */
    public void GameOver()
    {
        // Prevent multiple calls (e.g., if player hits two things at once)
        if (gameOver) return;

        gameOver = true;
        Debug.Log("GAME OVER! Press R to restart");

        /*
         * STEP 1: FOREACH LOOP - Destroy all AI GameObjects
         * ──────────────────────────────────────────────────
         *
         * foreach (AISnakeController ai in aiSnakes)
         * {
         *     Destroy(ai.gameObject);
         * }
         *
         * This loops through every AI in our list and destroys it.
         *
         * WHY FOREACH HERE?
         * - We need to process EVERY item in the list
         * - We don't need the index number
         * - We're just doing the same thing to each item
         *
         * IMPORTANT: We check for null because Unity might have
         * already destroyed some objects (e.g., if AI hit player
         * and died at the same time).
         */
        foreach (AISnakeController ai in aiSnakes)
        {
            if (ai != null)
            {
                // Destroy removes the GameObject from the game world
                // The snake disappears from the screen
                Destroy(ai.gameObject);
            }
        }

        /*
         * STEP 2: LIST.CLEAR() - Empty the list
         * ─────────────────────────────────────
         *
         * aiSnakes.Clear();
         *
         * After Destroy(), the list still has 3 items, but they're all null!
         * Clear() removes ALL items from the list at once.
         *
         * Before: [null][null][null]  Count = 3
         * After:  []                   Count = 0
         *
         * This is like Remove() but for EVERYTHING at once.
         * Much faster than calling Remove() for each item.
         */
        aiSnakes.Clear();

        // Show game over UI
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.ShowGameOver();
        }
    }

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  RESTART - Reload the scene                                                   ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    void RestartGame()
    {
        /*
         * SCENE RELOADING:
         * ────────────────
         * SceneManager.LoadScene() reloads the current scene.
         * This resets everything to its starting state:
         * - Player respawns at starting position
         * - All Lists are empty (new script instances)
         * - Score resets
         * - All GameObjects recreated
         *
         * It's like pressing "restart" on the game!
         */
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  HELPER METHODS - Public access to AI information                             ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    /*
     * GetAISnakeCount returns how many AI snakes are currently active.
     * Other scripts might use this for UI or difficulty scaling.
     */
    public int GetAISnakeCount()
    {
        return aiSnakes.Count;
    }

    /*
     * OnAISnakeDied is called when an individual AI snake dies.
     * This keeps our list in sync with reality and checks for win condition.
     */
    public void OnAISnakeDied(AISnakeController ai)
    {
        // Don't process if game is already over
        if (gameOver) return;

        /*
         * LIST.REMOVE() - Remove a specific item
         * ──────────────────────────────────────
         * Unlike Clear(), this removes just ONE item.
         *
         * Before: [AI1][AI2][AI3]  Count = 3
         * Remove(AI2)
         * After:  [AI1][AI3]       Count = 2
         *
         * The other items shift to fill the gap.
         */
        aiSnakes.Remove(ai);

        Debug.Log($"AI snake eliminated! Remaining: {aiSnakes.Count}");

        // WIN CONDITION: Check if all AI snakes are dead
        if (aiSnakes.Count == 0)
        {
            Victory();
        }
    }

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  VICTORY - Player wins!                                                       ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    /*
     * Victory is called when all AI snakes have been eliminated.
     * The player has won the game!
     */
    void Victory()
    {
        gameOver = true;
        Debug.Log("VICTORY! All AI snakes eliminated!");

        // Show victory UI
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.ShowVictory();
        }
    }

    /*
     * RemoveAISnake is kept for backwards compatibility.
     */
    public void RemoveAISnake(AISnakeController ai)
    {
        OnAISnakeDied(ai);
    }

    // ╔═══════════════════════════════════════════════════════════════════════════════╗
    // ║  DEBUGGING - Visualize spawn area in Editor                                   ║
    // ╚═══════════════════════════════════════════════════════════════════════════════╝

    void OnDrawGizmos()
    {
        // Draw the spawn area as a cyan rectangle
        Gizmos.color = Color.cyan;

        Vector3 center = new Vector3(
            (spawnAreaMin.x + spawnAreaMax.x) / 2,
            (spawnAreaMin.y + spawnAreaMax.y) / 2,
            0
        );

        Vector3 size = new Vector3(
            spawnAreaMax.x - spawnAreaMin.x,
            spawnAreaMax.y - spawnAreaMin.y,
            0
        );

        Gizmos.DrawWireCube(center, size);
    }
}

/*
 * ╔═══════════════════════════════════════════════════════════════════════════════════╗
 * ║  SUMMARY - What You Learned in This File                                          ║
 * ╠═══════════════════════════════════════════════════════════════════════════════════╣
 * ║                                                                                   ║
 * ║  LIST CLEANUP PATTERN:                                                            ║
 * ║  ─────────────────────                                                            ║
 * ║  1. foreach loop to Destroy() each GameObject                                     ║
 * ║  2. List.Clear() to empty the list                                                ║
 * ║  BOTH steps are needed for proper cleanup!                                        ║
 * ║                                                                                   ║
 * ║  LIST OPERATIONS USED:                                                            ║
 * ║  ─────────────────────                                                            ║
 * ║  • List.Add(item) - Add one item                                                  ║
 * ║  • List.Remove(item) - Remove one specific item                                   ║
 * ║  • List.Clear() - Remove ALL items at once                                        ║
 * ║  • List.Count - How many items currently                                          ║
 * ║                                                                                   ║
 * ║  LOOPS USED:                                                                      ║
 * ║  ───────────                                                                      ║
 * ║  • for loop - Spawning known number of AI snakes                                  ║
 * ║  • foreach loop - Processing all AI for cleanup                                   ║
 * ║                                                                                   ║
 * ║  KEY CONCEPTS:                                                                    ║
 * ║  ─────────────                                                                    ║
 * ║  • Destroy() affects the game world (visible objects)                             ║
 * ║  • Clear() affects the List (our tracking data)                                   ║
 * ║  • Always check for null when iterating (things might already be destroyed)       ║
 * ║                                                                                   ║
 * ║  NEXT FILE: AISnakeController.cs - See code reuse patterns!                       ║
 * ║                                                                                   ║
 * ╚═══════════════════════════════════════════════════════════════════════════════════╝
 */
