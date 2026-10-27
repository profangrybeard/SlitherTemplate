/*
 * GAME 220: Slither Template -- GameManager
 * Spawns the AI snakes, tracks them in a List, and handles game over, victory, and restart.
 * Compare it with FoodSpawner: a for loop fills a List, List.Remove() takes one out,
 * and at game over a foreach loop plus Clear() empties it.
 *
 * Read alongside: LOOPS_AND_LISTS_CONCEPTS.md section 7
 * Session 5: read GameOver() and RestartGame()   Session 6: read SpawnAISnakes()
 */

using System.Collections.Generic;  // Required for List<T>
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // INPUT - Same InputAction pattern as PlayerSnakeController's steerAction
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Input")]
    [SerializeField] private InputAction restartAction =
        new InputAction("Restart", InputActionType.Button, "<Keyboard>/r");

    // ─────────────────────────────────────────────────────────────────────────
    // SETTINGS - Tweak these in the Inspector
    // ─────────────────────────────────────────────────────────────────────────

    [Header("AI Settings")]
    public GameObject aiSnakePrefab;             // Drag AISnake.prefab here

    // TRY IT: set to 0, 10, then 20 (see section 9)
    public int numberOfAISnakes = 3;

    // These only draw the cyan box below. Which method really picks where an AI spawns?
    [Header("Spawn Settings")]
    public Vector2 spawnAreaMin = new Vector2(-20, -15);
    public Vector2 spawnAreaMax = new Vector2(20, 15);

    // ─────────────────────────────────────────────────────────────────────────
    // STATE - Tracked while playing
    // ─────────────────────────────────────────────────────────────────────────

    private List<AISnakeController> aiSnakes = new List<AISnakeController>(); // Every live AI snake

    private bool gameOver = false;               // Once true, only restart input is accepted

    // ─────────────────────────────────────────────────────────────────────────
    // UNITY MESSAGES - Unity calls these automatically
    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        Debug.Log("Game started!");
        SpawnAISnakes();
    }

    // Input actions must be enabled before they report anything
    void OnEnable()
    {
        restartAction.Enable();
    }

    void OnDisable()
    {
        restartAction.Disable();
    }

    void Update()
    {
        // Why does R only work after game over? What would change without the gameOver part?
        if (gameOver && restartAction.WasPressedThisFrame())
        {
            RestartGame();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SPAWNING - A for loop fills the List (Session 6)
    // ─────────────────────────────────────────────────────────────────────────

    // Runs SpawnSingleAISnake() numberOfAISnakes times
    void SpawnAISnakes()
    {
        for (int i = 0; i < numberOfAISnakes; i++)                    // [LOOP]
        {
            SpawnSingleAISnake();
        }

        Debug.Log($"Spawned {aiSnakes.Count} AI snakes");
    }

    // Creates one AI snake and adds it to the List
    void SpawnSingleAISnake()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject aiObj = Instantiate(aiSnakePrefab, spawnPosition, Quaternion.identity);
        AISnakeController ai = aiObj.GetComponent<AISnakeController>();

        if (ai != null)
        {
            aiSnakes.Add(ai);                    // List.Add(): same pattern as FoodSpawner
        }
    }

    // BoundsHelper keeps the spawn inside the play area
    Vector3 GetRandomSpawnPosition()
    {
        return BoundsHelper.GetRandomPosition();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GAME OVER - foreach + Clear() empties the List (Session 5)
    // ─────────────────────────────────────────────────────────────────────────
    // DESTROY vs CLEAR: they do different jobs, and you need both.
    //   Destroy(ai.gameObject)   removes the snake from the game world
    //   aiSnakes.Clear()         removes every entry from the List
    // Destroy() doesn't touch the List, and Clear() doesn't touch the game world.

    // Called by PlayerSnakeController.Die()
    public void GameOver()
    {
        // Why stop here if gameOver is already true? (Hint: what if the player hits two things at once?)
        if (gameOver) return;

        gameOver = true;
        Debug.Log("GAME OVER! Press R to restart");

        foreach (AISnakeController ai in aiSnakes)                    // [LOOP]
        {
            if (ai != null)                      // Some may already be destroyed
            {
                Destroy(ai.gameObject);
            }
        }

        // Comment out Clear() and log aiSnakes.Count after it. Why isn't the List empty?
        aiSnakes.Clear();

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.ShowGameOver();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RESTART
    // ─────────────────────────────────────────────────────────────────────────

    // Reloading the scene resets the player, score, and every List. The high score survives because PlayerPrefs saves it to disk.
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AI TRACKING - AI snakes report back here
    // ─────────────────────────────────────────────────────────────────────────

    public int GetAISnakeCount()
    {
        return aiSnakes.Count;
    }

    // Called by AISnakeController.Die(). Keeps the List in sync and checks for a win.
    public void OnAISnakeDied(AISnakeController ai)
    {
        if (gameOver) return;                    // The game already ended, so don't count it

        aiSnakes.Remove(ai);                     // List.Remove(): removes just this one

        Debug.Log($"AI snake eliminated! Remaining: {aiSnakes.Count}");

        // Why check for the win right after Remove(), instead of every frame in Update()?
        if (aiSnakes.Count == 0)
        {
            Victory();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // VICTORY
    // ─────────────────────────────────────────────────────────────────────────

    void Victory()
    {
        gameOver = true;
        Debug.Log("VICTORY! All AI snakes eliminated!");

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.ShowVictory();
        }
    }

    // Older name, kept so existing calls still work
    public void RemoveAISnake(AISnakeController ai)
    {
        OnAISnakeDied(ai);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DEBUGGING - Draws in the Scene view only
    // ─────────────────────────────────────────────────────────────────────────

    // Cyan box = spawnAreaMin/Max. Compare it with FoodSpawner's green box.
    void OnDrawGizmos()
    {
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
