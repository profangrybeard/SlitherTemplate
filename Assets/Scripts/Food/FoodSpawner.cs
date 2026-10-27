/*
 * GAME 220: Slither Template -- FoodSpawner
 * Keeps the arena stocked: List.Add() on spawn, List.Remove() on collect, and a while
 * loop to find a spot that isn't right on top of a snake.
 *
 * Read alongside: LOOPS_AND_LISTS_CONCEPTS.md section 3
 * Session 4: read Start(), OnFoodCollected(), and GetRandomValidPosition()
 */

using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // SETTINGS - Tweak these in the Inspector
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Spawn Settings")]
    public GameObject foodPrefab;                // Drag FoodPellet.prefab here
    public int initialFoodCount = 20;

    [Header("Validation")]
    public float minDistanceFromSnakes = 2f;     // Too close to a snake head = try again
    public int maxSpawnAttempts = 10;            // The while loop gives up after this many tries

    // ─────────────────────────────────────────────────────────────────────────
    // STATE - Tracked while playing
    // ─────────────────────────────────────────────────────────────────────────
    // THE LIST LIFECYCLE: every pellet goes through the same three steps.
    //   SpawnFood()          Instantiate it, then activeFoodPellets.Add(pellet)
    //   OnFoodCollected()    activeFoodPellets.Remove(pellet), then Destroy it
    //   then SpawnFood() again, so activeFoodPellets.Count stays the same

    private List<FoodPellet> activeFoodPellets = new List<FoodPellet>(); // Every pellet in the arena

    // ─────────────────────────────────────────────────────────────────────────
    // UNITY MESSAGES - Unity calls these automatically
    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        for (int i = 0; i < initialFoodCount; i++)                    // [LOOP]
        {
            SpawnFood();
        }
        Debug.Log($"Spawned {activeFoodPellets.Count} food pellets");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PUBLIC METHODS - Other scripts call these
    // ─────────────────────────────────────────────────────────────────────────

    public void SpawnFood()
    {
        Vector3 spawnPosition = GetRandomValidPosition();
        GameObject foodObj = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        FoodPellet pellet = foodObj.GetComponent<FoodPellet>();

        if (pellet != null)
        {
            activeFoodPellets.Add(pellet);                            // Start tracking this food
            pellet.SetSpawner(this);
        }
    }

    // Called by FoodPellet when a snake eats it
    public void OnFoodCollected(FoodPellet pellet)
    {
        // Why remove it from the List before destroying it?
        activeFoodPellets.Remove(pellet);
        Destroy(pellet.gameObject);

        SpawnFood();                             // Replace it, so the count stays the same
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PRIVATE METHODS - Picking a spot
    // ─────────────────────────────────────────────────────────────────────────

    // WHILE LOOPS: repeat as long as a condition is true, when you DON'T know how many
    // passes it will take. Here: keep picking random spots until one is valid.
    //   while (attempts < maxSpawnAttempts) { ...try a spot...  attempts++; }
    // `return` inside the loop ends the whole method, so the loop stops early on success.

    // Start() uses a for loop. Why is a while loop the better fit here?
    Vector3 GetRandomValidPosition()
    {
        Vector3 position = Vector3.zero;
        int attempts = 0;

        // Why cap the loop at maxSpawnAttempts? What could happen if no spot were ever valid?
        while (attempts < maxSpawnAttempts)                           // [LOOP]
        {
            position = BoundsHelper.GetRandomPosition();

            if (IsPositionValid(position))
            {
                return position;
            }
            attempts++;
        }

        Debug.LogWarning($"Could not find valid spawn position after {maxSpawnAttempts} attempts");
        return position;                         // Use the last try anyway
    }

    // ARRAYS vs LISTS: FindObjectsByType hands back an ARRAY (fixed size, AISnakeController[]).
    // Here we only read it, never add to it, so an array is fine. foreach works on both.
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

        AISnakeController[] aiSnakes = FindObjectsByType<AISnakeController>();
        foreach (AISnakeController ai in aiSnakes)                    // [LOOP]
        {
            if (Vector3.Distance(position, ai.transform.position) < minDistanceFromSnakes)
            {
                return false;
            }
        }

        return true;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DEBUGGING - Draws in the Scene view only
    // ─────────────────────────────────────────────────────────────────────────

    // Green box = the play bounds from BoundsHelper, where food and AI snakes spawn
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = BoundsHelper.GetBoundsCenter();
        Vector2 size = BoundsHelper.GetBoundsSize();
        Gizmos.DrawWireCube(center, new Vector3(size.x, size.y, 0));
    }
}
