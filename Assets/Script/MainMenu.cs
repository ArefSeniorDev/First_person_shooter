using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

   public void QuitGame()
    {
       // Application.Quit();
       Debug.Log("خروج از بازی...");

        // این خط فقط در نسخه‌ی نهایی (Build شده) کار می‌کند
        Application.Quit();

        // این خط برای تست در محیط Unity Editor است
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
