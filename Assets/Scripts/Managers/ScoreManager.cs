/*
 * GAME 220: Slither Template -- ScoreManager
 * The one place the score lives: tracks it, updates the UI, saves the high score, shows end messages.
 * Other scripts can ask for the score or add to it, but they can't set it directly.
 *
 * Read alongside: LOOPS_AND_LISTS_CONCEPTS.md section 5
 * Session 4: read AddScore() and connect the UI text fields in the Inspector
 */

using UnityEngine;
using TMPro; // Required for TextMeshPro UI components

public class ScoreManager : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // REFERENCES - Drag these in from the Hierarchy
    // ─────────────────────────────────────────────────────────────────────────

    [Header("UI References")]
    public TMP_Text scoreText;      // Displays current score during gameplay
    public TMP_Text gameOverText;   // Displays game over message

    // ─────────────────────────────────────────────────────────────────────────
    // STATE - The score itself
    // ─────────────────────────────────────────────────────────────────────────
    // ENCAPSULATION: the score is private, so the only way to change it from outside is
    // AddScore(). That method also updates the UI and the high score, so they can't be forgotten.
    //   scoreManager.AddScore(10);        works from any script
    //   scoreManager.currentScore = 999;  compile error: currentScore is private

    // This Header never shows up in the Inspector. Why not?
    [Header("Score Settings")]
    private int currentScore = 0;                // This game only
    private int highScore = 0;                   // Best ever, saved with PlayerPrefs

    // ─────────────────────────────────────────────────────────────────────────
    // UNITY MESSAGES - Unity calls these automatically
    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0); // 0 if nothing is saved yet

        UpdateScoreDisplay();

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);   // Hidden until the game ends
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SCORE
    // ─────────────────────────────────────────────────────────────────────────

    // The only way to change the score from outside, so the UI and high score always update
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();

        // Why save highScore with PlayerPrefs, but never currentScore?
        if (currentScore > highScore)
        {
            highScore = currentScore;

            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();                  // Write to disk now in case the game crashes
        }
    }

    // Private helper: keeps the UI in sync with the score
    void UpdateScoreDisplay()
    {
        // Why check for null? Clear Score Text in the Inspector and press Play. What happens?
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString(); // Same as $"Score: {currentScore}"
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // END-OF-GAME MESSAGES
    // ─────────────────────────────────────────────────────────────────────────

    // Called by GameManager.GameOver() when the player dies
    public void ShowGameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);

            // $"" fills in {variables}; \n starts a new line
            gameOverText.text = $"GAME OVER!\nScore: {currentScore}\nHigh Score: {highScore}\n\nPress R to Restart";
        }
    }

    // Called by GameManager.Victory() when all AI snakes are eliminated
    public void ShowVictory()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = $"VICTORY!\nAll enemies eliminated!\n\nScore: {currentScore}\nHigh Score: {highScore}\n\nPress R to Play Again";
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GETTERS - Read the score, but can't change it
    // ─────────────────────────────────────────────────────────────────────────
    // RETURN VALUES: these methods hand a value BACK to whoever called them.
    //   The type before the name (int) is what comes back.
    //   `return` sends it back and ends the method.
    //       int score = scoreManager.GetCurrentScore();    // score now holds the current score
    //   A `void` method (like AddScore) does something but hands nothing back.

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    // For restarting without reloading the scene, or for testing
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }
}
