/*
 * GAME 220: Slither Template -- FoodPellet
 * One food item. When a snake touches it, the snake grows, the player scores, and the
 * spawner replaces it.
 *
 * Read alongside: LOOPS_AND_LISTS_CONCEPTS.md section 4
 * Session 4: read OnTriggerEnter2D, then try the growthAmount experiment (section 9)
 * Summative: new food types (like one worth 50 points) start here.
 */

using UnityEngine;

public class FoodPellet : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // SETTINGS - Tweak these on the FoodPellet prefab
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Pellet Settings")]
    // TRY IT: set growthAmount to 5, then 0, and watch what happens when you eat (see section 9)
    public int growthAmount = 1;                 // Segments a snake gains from this food

    // ─────────────────────────────────────────────────────────────────────────
    // REFERENCES - Set by code, not in the Inspector
    // ─────────────────────────────────────────────────────────────────────────

    private FoodSpawner spawner;                 // The spawner that made us, so we can report back

    // Called by FoodSpawner right after Instantiate() so we can call it back later
    public void SetSpawner(FoodSpawner foodSpawner)
    {
        spawner = foodSpawner;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COLLISION
    // ─────────────────────────────────────────────────────────────────────────
    // TRIGGERS: OnTriggerEnter2D runs once, on the frame two colliders start to overlap.
    //   It needs a Collider2D on both objects, Is Trigger on at least one,
    //   and a Rigidbody2D on at least one. (The snake heads have the Rigidbody2D.)
    // GetComponent<T>() looks for a script on the object we touched. It returns null
    // if that object doesn't have one, so check before you use it.

    void OnTriggerEnter2D(Collider2D other)
    {
        // Player snake: grow, score, and tell the spawner
        if (other.CompareTag("Player"))          // CompareTag is faster than tag == "Player"
        {
            PlayerSnakeController player = other.GetComponent<PlayerSnakeController>(); // null if the script isn't there

            if (player != null)
            {
                player.Grow(growthAmount);

                // Spawner removes us from its list, destroys us, and spawns a replacement.
                // Why hand that to the spawner instead of calling Destroy(gameObject) here?
                if (spawner != null)
                {
                    spawner.OnFoodCollected(this);
                }
                else
                {
                    Destroy(gameObject);         // No spawner (placed by hand): clean up ourselves
                }

                // Every pellet is worth 10 here. Where would a food worth 50 need to change?
                ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>(); // Slower than a saved reference, but simpler to read
                if (scoreManager != null)
                {
                    scoreManager.AddScore(10);
                }
            }
        }

        // AI snake: same pattern, but no score
        if (other.CompareTag("AISnake"))
        {
            AISnakeController ai = other.GetComponent<AISnakeController>();

            if (ai != null)
            {
                ai.Grow(growthAmount);

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

    // ─────────────────────────────────────────────────────────────────────────
    // VISUAL EFFECT
    // ─────────────────────────────────────────────────────────────────────────

    // Bobbing is visual only, so it runs in Update for the smoothest motion
    void Update()
    {
        float bobAmount = 0.1f;    // How far up/down to bob
        float bobSpeed = 2f;       // How fast to bob

        // Mathf.Sin swings between -1 and 1 over time. Why multiply by Time.deltaTime too?
        float newY = transform.position.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z); // Change Y only
    }
}
