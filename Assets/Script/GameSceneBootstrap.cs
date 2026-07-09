using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneBootstrap : MonoBehaviour
{
    private const string BootstrapName = "Runtime_GameScene_Bootstrap";
    private const string RuntimeMapName = "Runtime_ForestCity_Redesign";

    [RuntimeInitializeOnLoadMethod]
    private static void CreateBootstrap()
    {
        // Only the playable scene gets the runtime world builder and visual cleanup.
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            return;
        }

        if (GameObject.Find(BootstrapName) != null)
        {
            return;
        }

        GameObject bootstrap = new GameObject(BootstrapName);
        bootstrap.AddComponent<GameSceneBootstrap>();
        bootstrap.AddComponent<SceneVisualFixes>();

        GameObject oldGeneratedMap = GameObject.Find("Generated_ForestCityExtension");
        if (oldGeneratedMap != null)
        {
            oldGeneratedMap.SetActive(false);
        }

        GameObject city = GameObject.Find(RuntimeMapName);
        if (city == null)
        {
            city = new GameObject(RuntimeMapName);
            city.transform.position = Vector3.zero;
        }

        RuinedCityBuilder cityBuilder = city.GetComponent<RuinedCityBuilder>();
        if (cityBuilder == null)
        {
            cityBuilder = city.AddComponent<RuinedCityBuilder>();
        }

        RuinedCityPortalUI portal = bootstrap.AddComponent<RuinedCityPortalUI>();
        portal.ruinedCity = cityBuilder;
        portal.cityCenter = cityBuilder.cityCenter;
    }
}
