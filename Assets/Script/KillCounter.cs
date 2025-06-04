using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    public static int killCount = 0;
    public Text killText;

    void Start()
    {
        killCount = 0;
        UpdateUI();
    }

    public void AddKill()
    {
        killCount++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (killText != null)
            killText.text = "Kills: " + killCount;
    }
    public void CheckHighScore()
    {
        int currentHigh = PlayerPrefs.GetInt("HighKillCount", 0);

        if (killCount > currentHigh)
        {
            PlayerPrefs.SetInt("HighKillCount", killCount);
            PlayerPrefs.Save();
            Debug.Log("New High Score: " + killCount);
        }
        else
        {
            Debug.Log("No new high score. Current: " + killCount + ", High: " + currentHigh);
        }

        FindObjectOfType<HighScoreDisplay>().UpdateScore();

    }
}
