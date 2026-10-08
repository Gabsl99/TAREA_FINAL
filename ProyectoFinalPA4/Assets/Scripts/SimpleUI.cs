using UnityEngine;

public class SimpleUI : MonoBehaviour
{
    void OnGUI()
    {
        // Coin counter at top-left
        GUI.Label(new Rect(10, 10, 200, 30), "Coins: " + (GameManager.Instance != null ? GameManager.Instance.coinCount : 0));

        // Game Over or Victory messages centered
        if (GameManager.Instance != null && GameManager.Instance.gameOver)
        {
            string message = GameManager.Instance.victory ? "VICTORY!" : "GAME OVER";
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 20, 100, 40), message);

            // Show restart hint
            GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height / 2 + 20, 120, 30), "Press R to Restart");
        }
    }
}