using UnityEngine;
using UnityEngine.UI;

public class RuinedCityPortalUI : MonoBehaviour
{
    public RuinedCityBuilder ruinedCity;
    public Transform player;
    public Vector3 cityCenter = new Vector3(120f, 0f, 120f);
    public Vector2 buttonSize = new Vector2(190f, 42f);
    public string buttonText = "Ruined City";

    void Start()
    {
        EnsureCityExists();
        CreateTravelButton();
    }

    public void TravelToRuinedCity()
    {
        EnsureCityExists();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player == null || ruinedCity == null || ruinedCity.playerSpawnPoint == null)
        {
            Debug.LogWarning("Ruined city travel is missing player or spawn point.");
            return;
        }

        // CharacterController must be disabled while teleporting or it can snap the player back.
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        player.position = ruinedCity.playerSpawnPoint.position;
        player.rotation = ruinedCity.playerSpawnPoint.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }
    }

    private void EnsureCityExists()
    {
        if (ruinedCity != null)
        {
            return;
        }

        ruinedCity = FindObjectOfType<RuinedCityBuilder>();
        if (ruinedCity != null)
        {
            return;
        }

        GameObject cityObject = new GameObject("Runtime_RuinedCity");
        cityObject.transform.position = cityCenter;
        ruinedCity = cityObject.AddComponent<RuinedCityBuilder>();
    }

    private void CreateTravelButton()
    {
        if (GameObject.Find("RuinedCityButton") != null)
        {
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("No Canvas found for ruined city button.");
            return;
        }

        GameObject buttonObject = new GameObject("RuinedCityButton");
        buttonObject.transform.SetParent(canvas.transform, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = buttonSize;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.16f, 0.16f, 0.16f, 0.82f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(TravelToRuinedCity);

        GameObject labelObject = new GameObject("Text");
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text label = labelObject.AddComponent<Text>();
        label.text = buttonText;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.fontSize = 18;
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}
