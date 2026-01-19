/*
 * GAME 220: Slither.io Template
 * Session 4: Score Manager
 *
 * TEACHING FOCUS:
 * - UI integration with Text components
 * - Event-driven updates (score changes when food is collected)
 * - FindObjectOfType to locate other scripts
 *
 * This script manages the score display:
 * - Tracks current score
 * - Updates UI text
 * - Shows game over message
 */

using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    // TEACHING: We drag the Text component from the scene into this field
    public Text scoreText;
    public Text gameOverText;

    [Header("Score Settings")]
    private int currentScore = 0;
    private int highScore = 0;

    void Start()
    {
        // TEACHING: Load high score from PlayerPrefs (persistent storage)
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Initialize display
        UpdateScoreDisplay();

        // Hide game over text at start
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }

    // ============================================
    // SCORE MANAGEMENT
    // ============================================

    // TEACHING: Public method so other scripts can add to the score
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();

        // Check for new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            // TEACHING: PlayerPrefs saves data between game sessions
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            // TEACHING: ToString() converts a number to text
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }

    // ============================================
    // GAME OVER
    // ============================================

    public void ShowGameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = $"GAME OVER!\nScore: {currentScore}\nHigh Score: {highScore}\n\nPress R to Restart";
        }
    }

    // ============================================
    // HELPER METHODS
    // ============================================

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    // TEACHING: Reset score (useful for restart)
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }
}
