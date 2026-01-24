/*
 * ╔════════════════════════════════════════════════════════════════════════════╗
 * ║  GAME 220: Slither.io Template                                             ║
 * ║  ScoreManager.cs - Score & UI Management                                   ║
 * ╚════════════════════════════════════════════════════════════════════════════╝
 *
 * LEARNING OBJECTIVES:
 * After studying this file, you will understand:
 * - How to connect scripts to Unity UI elements
 * - Event-driven updates (score changes when food is collected)
 * - PlayerPrefs for saving data between game sessions
 * - Separation of concerns (why score logic is separate from player logic)
 *
 * DATA STRUCTURES USED: None
 * LOOPS USED: None
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * WHAT DOES THIS SCRIPT DO?
 * ══════════════════════════
 * The ScoreManager is the single source of truth for the player's score.
 * It handles:
 * - Tracking the current score
 * - Updating the UI display
 * - Saving/loading high scores
 * - Showing game over message
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * SEPARATION OF CONCERNS - Why is score separate from the player?
 * ═══════════════════════════════════════════════════════════════
 *
 * We COULD put score logic in PlayerSnakeController, but that would:
 * - Make PlayerSnakeController even longer
 * - Mix movement code with UI code
 * - Make it harder to find and change score logic
 *
 * Instead, each script has ONE JOB:
 *
 *    ┌────────────────────────┐
 *    │  PlayerSnakeController │  Job: Control the snake's movement
 *    └───────────┬────────────┘
 *                │
 *                │ "I collected food!"
 *                ▼
 *    ┌────────────────────────┐
 *    │      FoodPellet        │  Job: Detect collection, notify score
 *    └───────────┬────────────┘
 *                │
 *                │ scoreManager.AddScore(10)
 *                ▼
 *    ┌────────────────────────┐
 *    │     ScoreManager       │  Job: Track score, update UI
 *    └───────────┬────────────┘
 *                │
 *                │ Updates text on screen
 *                ▼
 *    ┌────────────────────────┐
 *    │      UI Text           │  Job: Display text to player
 *    └────────────────────────┘
 *
 * This is called "Single Responsibility Principle" - each part does one thing well.
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * EVENT-DRIVEN UPDATES:
 * ══════════════════════
 *
 * The score doesn't update every frame. It only updates WHEN something happens:
 *
 *    Event: Food collected
 *         │
 *         └──> AddScore(10) called
 *                   │
 *                   └──> UpdateScoreDisplay() updates UI
 *
 * This is more efficient than checking "did score change?" every frame.
 * It's also cleaner - the UI updates BECAUSE something happened.
 *
 * ═══════════════════════════════════════════════════════════════════════════════
 */

using UnityEngine;
using TMPro; // Required for TextMeshPro UI components

public class ScoreManager : MonoBehaviour
{
    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  UI REFERENCES - Drag these from the scene in Unity Inspector             ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    [Header("UI References")]
    /*
     * HOW TO CONNECT UI IN UNITY:
     * ───────────────────────────
     * 1. Create a UI > Text - TextMeshPro element in the scene (called "ScoreText")
     * 2. Select the GameObject with this ScoreManager script
     * 3. In Inspector, find the "Score Text" field
     * 4. Drag the ScoreText from Hierarchy into the field
     *
     * Now this script can change what that text displays!
     *
     * WHY TEXTMESHPRO?
     * TextMeshPro (TMP) is Unity's modern text system with better:
     * - Text rendering quality
     * - Font styling options
     * - Performance
     * Legacy UI.Text is outdated and should be avoided in new projects.
     */
    public TMP_Text scoreText;      // Displays current score during gameplay
    public TMP_Text gameOverText;   // Displays game over message

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  SCORE TRACKING - The actual score data                                   ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    [Header("Score Settings")]
    /*
     * currentScore: The score for THIS game session
     * highScore: The best score EVER (saved between sessions)
     *
     * WHY ARE THESE PRIVATE?
     * We don't want other scripts directly changing the score.
     * They should call AddScore() which properly updates the UI.
     * This is called "encapsulation" - controlling access to data.
     */
    private int currentScore = 0;
    private int highScore = 0;

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  START - Initialize score system                                          ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    void Start()
    {
        /*
         * PLAYERPREFS - Unity's simple save system
         * ─────────────────────────────────────────
         * PlayerPrefs stores key-value pairs that persist between game sessions.
         * Even if you close the game and reopen it, the data is still there!
         *
         * PlayerPrefs.GetInt("HighScore", 0):
         * - "HighScore" is the key (like a variable name)
         * - 0 is the default value if the key doesn't exist yet
         *
         * This is perfect for:
         * - High scores
         * - Settings (volume, controls)
         * - Simple save data
         *
         * For complex save data, you'd use files or a database instead.
         */
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Set initial display
        UpdateScoreDisplay();

        // Hide game over text at start
        if (gameOverText != null)
        {
            // SetActive(false) makes the GameObject invisible and inactive
            gameOverText.gameObject.SetActive(false);
        }
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  SCORE MANAGEMENT - Adding points and updating display                    ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * AddScore is PUBLIC so other scripts can call it.
     * This is the ONLY way to change the score from outside.
     *
     * WHY NOT JUST MAKE currentScore PUBLIC?
     * ──────────────────────────────────────
     * If currentScore was public, any script could do:
     *     scoreManager.currentScore = 9999999;  // Cheating!
     *     scoreManager.currentScore = -100;     // Bugs!
     *
     * By using AddScore(), we control HOW the score changes:
     * - We always update the UI after changing
     * - We can add validation (no negative points)
     * - We can check for high score
     *
     * This is ENCAPSULATION - hiding data and providing controlled access.
     */
    public void AddScore(int points)
    {
        // Add points to current score
        currentScore += points;

        // Update the UI to show new score
        UpdateScoreDisplay();

        // Check if this is a new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;

            /*
             * SAVING THE HIGH SCORE:
             * ──────────────────────
             * PlayerPrefs.SetInt("HighScore", highScore) saves the value
             * PlayerPrefs.Save() writes it to disk immediately
             *
             * Without Save(), the data might be lost if the game crashes.
             * For important data, always call Save()!
             */
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    /*
     * UpdateScoreDisplay is PRIVATE - only this script calls it.
     * It's an internal helper that keeps the UI in sync with the score.
     */
    void UpdateScoreDisplay()
    {
        // Only update if we have a reference to the text
        if (scoreText != null)
        {
            /*
             * TEXT + NUMBER CONCATENATION:
             * ────────────────────────────
             * "Score: " + currentScore.ToString()
             *
             * ToString() converts a number to text.
             * 100 becomes "100"
             * Result: "Score: 100"
             *
             * You can also use string interpolation (fancier):
             * scoreText.text = $"Score: {currentScore}";
             *
             * Both work the same way!
             */
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  GAME OVER - Display final results                                        ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * Called by GameManager when the player dies.
     * Shows the game over message with final score and high score.
     */
    public void ShowGameOver()
    {
        if (gameOverText != null)
        {
            // Make the text visible
            gameOverText.gameObject.SetActive(true);

            /*
             * STRING INTERPOLATION WITH $"":
             * ──────────────────────────────
             * The $ before the string enables interpolation.
             * {variableName} gets replaced with the variable's value.
             * \n creates a new line.
             *
             * Example:
             * currentScore = 150, highScore = 200
             * Result:
             * "GAME OVER!
             *  Score: 150
             *  High Score: 200
             *
             *  Press R to Restart"
             */
            gameOverText.text = $"GAME OVER!\nScore: {currentScore}\nHigh Score: {highScore}\n\nPress R to Restart";
        }
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  VICTORY - Display win message                                            ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * Called by GameManager when all AI snakes are eliminated.
     * Shows the victory message with final score and high score.
     */
    public void ShowVictory()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = $"VICTORY!\nAll enemies eliminated!\n\nScore: {currentScore}\nHigh Score: {highScore}\n\nPress R to Play Again";
        }
    }

    // ╔═══════════════════════════════════════════════════════════════════════════╗
    // ║  HELPER METHODS - Getting score values                                    ║
    // ╚═══════════════════════════════════════════════════════════════════════════╝

    /*
     * GETTER METHODS:
     * ───────────────
     * These let other scripts READ the score without being able to CHANGE it.
     * This is part of encapsulation - controlled access to data.
     *
     * Other scripts can do:
     *     int score = scoreManager.GetCurrentScore();  // OK!
     *
     * But they can't do:
     *     scoreManager.currentScore = 999;  // Won't compile! currentScore is private
     */
    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    /*
     * ResetScore could be useful for:
     * - Restarting the game without reloading the scene
     * - Challenge modes that reset score
     * - Testing
     */
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }
}

/*
 * ╔═══════════════════════════════════════════════════════════════════════════════╗
 * ║  SUMMARY - What You Learned                                                   ║
 * ╠═══════════════════════════════════════════════════════════════════════════════╣
 * ║                                                                               ║
 * ║  1. UI Connection                                                             ║
 * ║     - Public variables can be connected in Unity Inspector                    ║
 * ║     - Drag UI elements from scene to script fields                            ║
 * ║                                                                               ║
 * ║  2. Event-Driven Updates                                                      ║
 * ║     - Don't update every frame if you don't need to                           ║
 * ║     - Update when something happens (score changes)                           ║
 * ║                                                                               ║
 * ║  3. PlayerPrefs                                                               ║
 * ║     - Simple key-value storage that persists between sessions                 ║
 * ║     - Perfect for high scores and settings                                    ║
 * ║                                                                               ║
 * ║  4. Encapsulation                                                             ║
 * ║     - Private variables, public methods                                       ║
 * ║     - Control how data can be accessed and modified                           ║
 * ║                                                                               ║
 * ║  5. Separation of Concerns                                                    ║
 * ║     - Each script has one job                                                 ║
 * ║     - Makes code easier to find, read, and modify                             ║
 * ║                                                                               ║
 * ║  NEXT FILE: BoundsHelper.cs - Learn about static utility classes!             ║
 * ║                                                                               ║
 * ╚═══════════════════════════════════════════════════════════════════════════════╝
 */
