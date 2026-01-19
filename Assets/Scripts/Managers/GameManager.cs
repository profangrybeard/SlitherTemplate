/*
 * GAME 220: Slither.io Template
 * Session 1, 5-6: Game Manager
 *
 * TEACHING FOCUS:
 * - Managing multiple Lists (AI snakes)
 * - for loops for spawning multiple objects
 * - foreach loops for cleanup
 * - List.Clear() to empty a list
 * - Coordinating game systems
 *
 * This script manages the overall game:
 * - Initializes all systems
 * - Spawns AI snakes
 * - Handles game over state
 * - Manages restarts
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("AI Settings")]
    public GameObject aiSnakePrefab;

    // TEACHING: This controls how many AI opponents are in the game
    public int numberOfAISnakes = 3;

    [Header("Spawn Settings")]
    public Vector2 spawnAreaMin = new Vector2(-20, -15);
    public Vector2 spawnAreaMax = new Vector2(20, 15);

    // TEACHING: List manages all AI snakes in the game
    private List<AISnakeController> aiSnakes = new List<AISnakeController>();

    // Game state
    private bool gameOver = false;

    void Start()
    {
        // SESSION 1: Basic initialization
        Debug.Log("Game started!");

        // SESSION 6: Spawn AI snakes
        SpawnAISnakes();
    }

    void Update()
    {
        // TEACHING: Check for restart input
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    // ============================================
    // SESSION 6: AI Snake Management
    // ============================================

    void SpawnAISnakes()
    {
        // TEACHING: for loop spawns multiple AI snakes
        // Notice how we use the loop variable 'i' to count up to numberOfAISnakes
        for (int i = 0; i < numberOfAISnakes; i++)
        {
            SpawnSingleAISnake();
        }

        Debug.Log($"Spawned {aiSnakes.Count} AI snakes");
    }

    void SpawnSingleAISnake()
    {
        // Get a random spawn position
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // TEACHING: Instantiate creates a new GameObject from a prefab
        GameObject aiObj = Instantiate(aiSnakePrefab, spawnPosition, Quaternion.identity);

        // Get the AISnakeController component
        AISnakeController ai = aiObj.GetComponent<AISnakeController>();

        if (ai != null)
        {
            // TEACHING: List.Add() adds the AI snake to our list
            aiSnakes.Add(ai);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // TEACHING: Random.Range for random X and Y coordinates
        return new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            0
        );
    }

    // ============================================
    // SESSION 5: Game Over
    // ============================================

    public void GameOver()
    {
        if (gameOver) return; // Prevent multiple calls

        gameOver = true;
        Debug.Log("GAME OVER! Press R to restart");

        // TEACHING: foreach loop goes through all AI snakes
        // This is cleaner than a for loop when we just need to access each item
        foreach (AISnakeController ai in aiSnakes)
        {
            if (ai != null)
            {
                // Could add death animation or effects here
                Destroy(ai.gameObject);
            }
        }

        // TEACHING: List.Clear() removes all items from the list
        // After this, aiSnakes.Count will be 0
        aiSnakes.Clear();

        // Show game over UI
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.ShowGameOver();
        }
    }

    void RestartGame()
    {
        // TEACHING: SceneManager.LoadScene reloads the current scene
        // This resets everything to starting state
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ============================================
    // HELPER METHODS
    // ============================================

    // TEACHING: Public method other scripts can call
    public int GetAISnakeCount()
    {
        return aiSnakes.Count;
    }

    // TEACHING: Remove a specific AI snake from the list (if it dies)
    public void RemoveAISnake(AISnakeController ai)
    {
        // TEACHING: List.Remove() finds and removes the specific item
        aiSnakes.Remove(ai);

        // Could spawn a new AI to maintain population
        // SpawnSingleAISnake();
    }

    // ============================================
    // VISUALIZATION (for debugging)
    // ============================================

    void OnDrawGizmos()
    {
        // Draw the spawn area in editor
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
