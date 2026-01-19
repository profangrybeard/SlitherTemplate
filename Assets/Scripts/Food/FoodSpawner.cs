/*
 * GAME 220: Slither.io Template
 * Session 4: Food Spawner
 *
 * TEACHING FOCUS:
 * - List<T> for tracking spawned objects
 * - for loops for batch spawning
 * - while loops for validation/retry logic
 * - Random.Range for random positions
 * - List.Add() and List.Remove()
 *
 * This script manages all food pellets in the game:
 * - Spawns initial batch of food
 * - Tracks all active food in a List
 * - Respawns food when collected
 * - Ensures food spawns in valid positions
 */

using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject foodPrefab;

    // TEACHING: This controls how much food is in the game at start
    public int initialFoodCount = 20;

    [Header("Spawn Area")]
    // TEACHING: These define the rectangular area where food can spawn
    public Vector2 spawnAreaMin = new Vector2(-20, -15);
    public Vector2 spawnAreaMax = new Vector2(20, 15);

    [Header("Validation")]
    // TEACHING: Minimum distance from snakes when spawning
    public float minDistanceFromSnakes = 2f;
    public int maxSpawnAttempts = 10;

    // TEACHING: List stores all food pellets currently in the game
    // This lets us track them, count them, and remove them when collected
    private List<FoodPellet> activeFoodPellets = new List<FoodPellet>();

    void Start()
    {
        // TEACHING: for loop spawns initial batch of food
        // This runs once at the start of the game
        for (int i = 0; i < initialFoodCount; i++)
        {
            SpawnFood();
        }

        Debug.Log($"Spawned {activeFoodPellets.Count} food pellets");
    }

    // TEACHING: This method is public so other scripts can call it
    // FoodPellet will call this when it gets collected
    public void SpawnFood()
    {
        // Get a valid position
        Vector3 spawnPosition = GetRandomValidPosition();

        // TEACHING: Instantiate creates a new GameObject from a prefab
        // Quaternion.identity means "no rotation" (facing default direction)
        GameObject foodObj = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);

        // Get the FoodPellet component
        FoodPellet pellet = foodObj.GetComponent<FoodPellet>();

        if (pellet != null)
        {
            // TEACHING: List.Add() puts the pellet at the end of the list
            activeFoodPellets.Add(pellet);

            // Tell the pellet who spawned it (so it can notify us when collected)
            pellet.SetSpawner(this);
        }
    }

    // TEACHING: Called by FoodPellet when it gets collected
    public void OnFoodCollected(FoodPellet pellet)
    {
        // TEACHING: List.Remove() finds and removes the specific pellet
        // It searches through the list and removes the first match
        activeFoodPellets.Remove(pellet);

        // Destroy the GameObject
        Destroy(pellet.gameObject);

        // Spawn a new food to replace it
        SpawnFood();
    }

    // ============================================
    // POSITION GENERATION
    // ============================================

    Vector3 GetRandomValidPosition()
    {
        Vector3 position = Vector3.zero;
        int attempts = 0;

        // TEACHING: while loop keeps trying until we find a valid position
        // This prevents food from spawning inside snakes
        while (attempts < maxSpawnAttempts)
        {
            // TEACHING: Random.Range generates random number between min and max
            // For floats, it can be any value in the range (not just integers)
            position = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                0
            );

            // Check if this position is valid (not too close to snakes)
            if (IsPositionValid(position))
            {
                // TEACHING: return exits the method immediately with this value
                return position;
            }

            attempts++;
        }

        // TEACHING: If we couldn't find a valid position after max attempts,
        // just return the last attempt (better than nothing)
        Debug.LogWarning("Could not find valid food spawn position after " + maxSpawnAttempts + " attempts");
        return position;
    }

    bool IsPositionValid(Vector3 position)
    {
        // TEACHING: Check distance from player snake
        PlayerSnakeController player = FindObjectOfType<PlayerSnakeController>();
        if (player != null)
        {
            // Check distance from player head
            if (Vector3.Distance(position, player.transform.position) < minDistanceFromSnakes)
            {
                return false;
            }
        }

        // TEACHING: Check distance from AI snakes
        AISnakeController[] aiSnakes = FindObjectsOfType<AISnakeController>();
        // TEACHING: foreach loop checks all AI snakes
        foreach (AISnakeController ai in aiSnakes)
        {
            if (Vector3.Distance(position, ai.transform.position) < minDistanceFromSnakes)
            {
                return false;
            }
        }

        // Position is valid!
        return true;
    }

    // ============================================
    // VISUALIZATION (for debugging)
    // ============================================

    void OnDrawGizmos()
    {
        // TEACHING: Draw the spawn area in the editor
        Gizmos.color = Color.green;

        // Draw rectangle showing spawn area
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
