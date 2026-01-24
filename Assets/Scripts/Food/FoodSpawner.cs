/*
 * FoodSpawner.cs
 *
 * LEARNING FOCUS: List Add/Remove lifecycle
 *
 * Key Concepts:
 * - List.Add() when spawning
 * - List.Remove() when collected
 * - for loop for batch spawning
 * - while loop for validation with retry
 */

using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject foodPrefab;
    public int initialFoodCount = 20;

    [Header("Validation")]
    public float minDistanceFromSnakes = 2f;
    public int maxSpawnAttempts = 10;

    // Tracks all active food in the game
    private List<FoodPellet> activeFoodPellets = new List<FoodPellet>();

    void Start()
    {
        // FOR LOOP: Spawn initial batch of food
        for (int i = 0; i < initialFoodCount; i++)
        {
            SpawnFood();
        }
        Debug.Log($"Spawned {activeFoodPellets.Count} food pellets");
    }

    public void SpawnFood()
    {
        Vector3 spawnPosition = GetRandomValidPosition();
        GameObject foodObj = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        FoodPellet pellet = foodObj.GetComponent<FoodPellet>();

        if (pellet != null)
        {
            // List.Add() - track this food
            activeFoodPellets.Add(pellet);
            pellet.SetSpawner(this);
        }
    }

    // Called by FoodPellet when collected
    public void OnFoodCollected(FoodPellet pellet)
    {
        // List.Remove() - stop tracking this food
        activeFoodPellets.Remove(pellet);
        Destroy(pellet.gameObject);

        // Spawn replacement to keep count constant
        SpawnFood();
    }

    // WHILE LOOP: Keep trying until we find a valid position
    Vector3 GetRandomValidPosition()
    {
        Vector3 position = Vector3.zero;
        int attempts = 0;

        while (attempts < maxSpawnAttempts)
        {
            position = BoundsHelper.GetRandomPosition();

            if (IsPositionValid(position))
            {
                return position;
            }
            attempts++;
        }

        Debug.LogWarning($"Could not find valid spawn position after {maxSpawnAttempts} attempts");
        return position;
    }

    // FOREACH: Check distance from all snakes
    bool IsPositionValid(Vector3 position)
    {
        PlayerSnakeController player = FindAnyObjectByType<PlayerSnakeController>();
        if (player != null)
        {
            if (Vector3.Distance(position, player.transform.position) < minDistanceFromSnakes)
            {
                return false;
            }
        }

        AISnakeController[] aiSnakes = FindObjectsByType<AISnakeController>(FindObjectsSortMode.None);
        foreach (AISnakeController ai in aiSnakes)
        {
            if (Vector3.Distance(position, ai.transform.position) < minDistanceFromSnakes)
            {
                return false;
            }
        }

        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = BoundsHelper.GetBoundsCenter();
        Vector2 size = BoundsHelper.GetBoundsSize();
        Gizmos.DrawWireCube(center, new Vector3(size.x, size.y, 0));
    }
}
