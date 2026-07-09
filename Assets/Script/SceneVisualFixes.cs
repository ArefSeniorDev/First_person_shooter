using UnityEngine;
using UnityEngine.UI;

public class SceneVisualFixes : MonoBehaviour
{
    public bool hideGameOverAtStart = true;
    public bool disableStartupLightArtifacts = true;
    public bool hideMobileControlUi = true;
    public int startupCleanupFrames = 5;

    private int cleanupFramesLeft;

    void Start()
    {
        Time.timeScale = 1f;
        cleanupFramesLeft = startupCleanupFrames;

        if (hideGameOverAtStart)
        {
            HideGameOverPanels();
        }

        if (disableStartupLightArtifacts)
        {
            DisableStartupLightArtifacts();
        }

        if (hideMobileControlUi)
        {
            HideMobileControls();
        }
    }

    void LateUpdate()
    {
        if (cleanupFramesLeft <= 0)
        {
            return;
        }

        cleanupFramesLeft--;
        if (hideGameOverAtStart)
        {
            HideGameOverPanels();
        }

        if (hideMobileControlUi)
        {
            HideMobileControls();
        }
    }

    private void HideGameOverPanels()
    {
        // Some UI panels were visible as thin lines at startup; names/text decide what is hidden.
        Transform[] transforms = FindObjectsOfType<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            string lowerName = transforms[i].name.ToLower();
            if (lowerName.Contains("gameover") || lowerName.Contains("game over"))
            {
                transforms[i].gameObject.SetActive(false);
            }
        }

        Text[] texts = FindObjectsOfType<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].text != null && texts[i].text.ToLower().Contains("game over"))
            {
                GameObject panel = texts[i].transform.parent != null ? texts[i].transform.parent.gameObject : texts[i].gameObject;
                panel.SetActive(false);
            }
        }
    }

    private void DisableStartupLightArtifacts()
    {
        // Lens flares and already-playing particles can appear as icons/white dots in the Game view.
        LensFlare[] flares = FindObjectsOfType<LensFlare>();
        for (int i = 0; i < flares.Length; i++)
        {
            flares[i].enabled = false;
        }

        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.MainModule main = particles[i].main;
            main.playOnAwake = false;
            main.loop = false;
            particles[i].Stop();
            particles[i].Clear();
        }
    }

    private void HideMobileControls()
    {
        // The project is a desktop FPS; imported mobile control sprites can show as white icons over the gun.
        Transform[] transforms = FindObjectsOfType<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            string lowerName = transforms[i].name.ToLower();
            if (lowerName.Contains("mobile") || lowerName.Contains("touchpad") || lowerName.Contains("joystick") || lowerName.Contains("thumbstick"))
            {
                transforms[i].gameObject.SetActive(false);
            }
        }
    }
}
