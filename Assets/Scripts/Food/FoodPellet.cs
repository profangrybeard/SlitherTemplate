/*
 * GAME 220: Slither.io Template
 * Session 4: Food Pellet
 *
 * TEACHING FOCUS:
 * - OnTriggerEnter2D for collision detection
 * - CompareTag for identifying objects
 * - Component communication (notifying spawner)
 *
 * This script handles individual food pellets:
 * - Detects when a snake collides with it
 * - Tells the snake to grow
 * - Notifies the spawner that it was collected
 */

using UnityEngine;

public class FoodPellet : MonoBehaviour
{
    [Header("Pellet Settings")]
    public int growthAmount = 1;

    // Reference to the spawner that created this pellet
    private FoodSpawner spawner;

    // TEACHING: This is called by the spawner when the pellet is created
    public void SetSpawner(FoodSpawner foodSpawner)
    {
        spawner = foodSpawner;
    }

    // TEACHING: OnTriggerEnter2D is called automatically by Unity's physics system
    // This happens when another object's collider overlaps with this object's collider
    //
    // Requirements for this to work:
    // 1. Both objects must have a Collider2D component
    // 2. At least one must have "Is Trigger" checked
    // 3. At least one must have a Rigidbody2D component
    void OnTriggerEnter2D(Collider2D other)
    {
        // TEACHING: CompareTag checks what type of object collided with us
        // This is faster than comparing other.gameObject.tag == "Player"
        if (other.CompareTag("Player"))
        {
            // TEACHING: GetComponent<T>() retrieves a script from the GameObject
            PlayerSnakeController player = other.GetComponent<PlayerSnakeController>();

            if (player != null)
            {
                // Tell the player snake to grow
                player.Grow(growthAmount);

                // Notify the spawner so it can respawn food and update the list
                if (spawner != null)
                {
                    spawner.OnFoodCollected(this);
                }
                else
                {
                    // If no spawner reference, just destroy ourselves
                    Destroy(gameObject);
                }

                // Update score
                ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
                if (scoreManager != null)
                {
                    scoreManager.AddScore(10);
                }
            }
        }

        // SESSION 6: AI snakes can also eat food
        if (other.CompareTag("AISnake"))
        {
            AISnakeController ai = other.GetComponent<AISnakeController>();

            if (ai != null)
            {
                // AI snakes can grow too
                ai.Grow(growthAmount);

                // Notify spawner
                if (spawner != null)
                {
                    spawner.OnFoodCollected(this);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    // ============================================
    // OPTIONAL: Bobbing Animation
    // ============================================

    // TEACHING: Update is good for visual effects that don't need physics
    void Update()
    {
        // Optional: Make food bob up and down slightly
        // This uses a sine wave for smooth oscillation
        float bobAmount = 0.1f;
        float bobSpeed = 2f;
        float newY = transform.position.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount * Time.deltaTime;

        // Update only the Y position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
