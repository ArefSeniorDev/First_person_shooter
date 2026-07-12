using UnityEngine;

public class HelpManager : MonoBehaviour
{
    public GameObject helpPanel;

    void Start()
    {
        helpPanel.SetActive(false); 
    }

    public void ShowHelp()
    {
        helpPanel.SetActive(true);
    }

    public void HideHelp()
    {
        helpPanel.SetActive(false);
    }
}
