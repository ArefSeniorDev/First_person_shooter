using UnityEngine;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour
{
    public Text highScoreText;

    void Start()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        int highScore = PlayerPrefs.GetInt("HighKillCount", 0);
        highScoreText.text = "High Score: " + highScore;
    }
}