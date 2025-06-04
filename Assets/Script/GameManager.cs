using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int enemiesToKill = 10;
    private int enemiesKilled = 0;

    public void EnemyKilled()
    {
        enemiesKilled++;
        if (enemiesKilled >= enemiesToKill)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("You Win!");
        SceneManager.LoadScene("WinScene"); 
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }
}
