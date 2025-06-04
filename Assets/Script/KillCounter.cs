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
}
