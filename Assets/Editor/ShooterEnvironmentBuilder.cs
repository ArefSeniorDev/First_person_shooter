using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ShooterEnvironmentBuilder
{
    private const string ScenePath = "Assets/Scenes/GameScene.unity";
    private const string RootName = "Generated_ForestCityExtension";

    [MenuItem("Tools/Shooter/Build Forest City Extension")]
    public static void BuildFromMenu()
    {
        BuildForestCityExtension();
    }

    public static void BuildForBatchMode()
    {
        BuildForestCityExtension();
    }

    private static void BuildForestCityExtension()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Object oldRoot = GameObject.Find(RootName);
        if (oldRoot != null)
        {
            Object.DestroyImmediate(oldRoot);
        }

        Random.InitState(250705);

        Material forestMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_ForestGround.mat", new Color(0.16f, 0.24f, 0.14f));
        Material roadMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_Asphalt.mat", new Color(0.055f, 0.055f, 0.055f));
        Material sidewalkMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_Sidewalk.mat", new Color(0.34f, 0.34f, 0.32f));
        Material trunkMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_Trunk.mat", new Color(0.27f, 0.18f, 0.11f));
        Material leavesMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_Leaves.mat", new Color(0.11f, 0.30f, 0.11f));
        Material rubbleMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_Rubble.mat", new Color(0.42f, 0.39f, 0.35f));
        Material laneMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_LaneMarking.mat", new Color(0.85f, 0.82f, 0.68f));
        Material towerMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_TowerConcrete.mat", new Color(0.23f, 0.24f, 0.24f));
        Material windowMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_WindowDark.mat", new Color(0.04f, 0.07f, 0.09f));
        Material damagedMaterial = GetOrCreateMaterial("Assets/Material/Generated/Generated_DamagedWall.mat", new Color(0.18f, 0.18f, 0.17f));

        GameObject root = new GameObject(RootName);
        GameObject forestRoot = CreateChild(root.transform, "Expanded_Forest");
        GameObject cityRoot = CreateChild(root.transform, "Abandoned_Grid_City");

        CreateExpandedGround(forestRoot.transform, forestMaterial);
        CreateForestExtension(forestRoot.transform, trunkMaterial, leavesMaterial);
        CreateCity(cityRoot.transform, roadMaterial, sidewalkMaterial, laneMaterial, rubbleMaterial, towerMaterial, windowMaterial, damagedMaterial);
        CreateCitySpawn(cityRoot.transform);
        CleanSceneVisualArtifacts();
        DisableEditorAnnotations();

        SetStaticRecursive(root);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        TryBuildNavMesh();

        Debug.Log("Shooter environment generated and saved into " + ScenePath);
    }

    private static Material GetOrCreateMaterial(string path, Color color)
    {
        EnsureFolder("Assets/Material/Generated");
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Diffuse"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        return material;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        if (!AssetDatabase.IsValidFolder("Assets/Material"))
        {
            AssetDatabase.CreateFolder("Assets", "Material");
        }

        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/Material", "Generated");
        }
    }

    private static GameObject CreateChild(Transform parent, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        return obj;
    }

    private static void CreateExpandedGround(Transform parent, Material material)
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Large_Forest_And_City_Ground";
        ground.transform.parent = parent;
        ground.transform.position = new Vector3(175f, -0.09f, 145f);
        ground.transform.localScale = new Vector3(620f, 0.18f, 520f);
        ground.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void CreateForestExtension(Transform parent, Material trunkMaterial, Material leavesMaterial)
    {
        Vector3 cityCenter = new Vector3(280f, 0f, 150f);
        for (int i = 0; i < 185; i++)
        {
            Vector3 position = new Vector3(Random.Range(-120f, 255f), 0f, Random.Range(-95f, 325f));
            if (Vector3.Distance(position, cityCenter) < 105f || position.x > 210f && position.z > 25f && position.z < 275f)
            {
                continue;
            }

            CreateTree(parent, position, trunkMaterial, leavesMaterial);
        }

        for (int i = 0; i < 24; i++)
        {
            Vector3 linePosition = new Vector3(205f + Random.Range(-8f, 8f), 0f, -75f + i * 14f);
            CreateTree(parent, linePosition, trunkMaterial, leavesMaterial);
        }
    }

    private static void CreateTree(Transform parent, Vector3 position, Material trunkMaterial, Material leavesMaterial)
    {
        GameObject tree = new GameObject("Generated_Tree");
        tree.transform.parent = parent;
        tree.transform.position = position;
        float scale = Random.Range(0.75f, 1.75f);

        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.parent = tree.transform;
        trunk.transform.localPosition = new Vector3(0f, 1.15f * scale, 0f);
        trunk.transform.localScale = new Vector3(0.34f * scale, 1.55f * scale, 0.34f * scale);
        trunk.GetComponent<Renderer>().sharedMaterial = trunkMaterial;

        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.name = "Leaves";
        leaves.transform.parent = tree.transform;
        leaves.transform.localPosition = new Vector3(0f, 3.05f * scale, 0f);
        leaves.transform.localScale = new Vector3(2.1f * scale, 1.55f * scale, 2.1f * scale);
        leaves.GetComponent<Renderer>().sharedMaterial = leavesMaterial;
    }

    private static void CreateCity(
        Transform parent,
        Material roadMaterial,
        Material sidewalkMaterial,
        Material laneMaterial,
        Material rubbleMaterial,
        Material towerMaterial,
        Material windowMaterial,
        Material damagedMaterial)
    {
        GameObject houseA = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/CartoonLowPolyCityLite/Prefabs/House_01.prefab");
        GameObject houseB = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/CartoonLowPolyCityLite/Prefabs/House_16.prefab");
        GameObject carPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/CartoonLowPolyCityLite/Prefabs/Car_03.prefab");
        GameObject trashPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/CartoonLowPolyCityLite/Prefabs/Trash_01.prefab");
        GameObject lightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/CartoonLowPolyCityLite/Prefabs/Light_01.prefab");

        Vector3 cityCenter = new Vector3(285f, 0f, 150f);
        float spacing = 30f;
        int halfGrid = 4;

        GameObject roads = CreateChild(parent, "Ordered_Road_Network");
        GameObject buildings = CreateChild(parent, "Ordered_Building_Blocks");
        GameObject props = CreateChild(parent, "Crash_And_Debris_Props");

        for (int i = -halfGrid; i <= halfGrid; i++)
        {
            float offset = i * spacing;
            CreateRoadCube(roads.transform, cityCenter + new Vector3(offset, 0.02f, 0f), new Vector3(7.2f, 0.09f, spacing * (halfGrid * 2 + 1)), roadMaterial, "NorthSouth_Road");
            CreateRoadCube(roads.transform, cityCenter + new Vector3(0f, 0.03f, offset), new Vector3(spacing * (halfGrid * 2 + 1), 0.09f, 7.2f), roadMaterial, "EastWest_Road");
            CreateSidewalk(roads.transform, cityCenter + new Vector3(offset + 5.3f, 0.09f, 0f), new Vector3(1.7f, 0.08f, spacing * (halfGrid * 2 + 1)), sidewalkMaterial);
            CreateSidewalk(roads.transform, cityCenter + new Vector3(offset - 5.3f, 0.09f, 0f), new Vector3(1.7f, 0.08f, spacing * (halfGrid * 2 + 1)), sidewalkMaterial);
            CreateSidewalk(roads.transform, cityCenter + new Vector3(0f, 0.10f, offset + 5.3f), new Vector3(spacing * (halfGrid * 2 + 1), 0.08f, 1.7f), sidewalkMaterial);
            CreateSidewalk(roads.transform, cityCenter + new Vector3(0f, 0.10f, offset - 5.3f), new Vector3(spacing * (halfGrid * 2 + 1), 0.08f, 1.7f), sidewalkMaterial);
        }

        CreateRoadMarkings(roads.transform, cityCenter, halfGrid, spacing, laneMaterial);

        int index = 0;
        for (int x = -halfGrid; x < halfGrid; x++)
        {
            for (int z = -halfGrid; z < halfGrid; z++)
            {
                Vector3 blockCenter = cityCenter + new Vector3(x * spacing + spacing * 0.5f, 0f, z * spacing + spacing * 0.5f);
                bool towerBlock = (x + z) % 3 == 0 || Mathf.Abs(x) <= 1 && Mathf.Abs(z) <= 1;
                if (towerBlock)
                {
                    CreateSkyscraperBlock(buildings.transform, blockCenter, towerMaterial, windowMaterial, damagedMaterial, rubbleMaterial, index);
                }
                else
                {
                    CreateHouseBlock(buildings.transform, blockCenter, houseA, houseB, rubbleMaterial, index);
                }

                index++;
            }
        }

        CreateCrashScenes(props.transform, cityCenter, spacing, carPrefab, rubbleMaterial, damagedMaterial);
        CreateStreetDebris(props.transform, cityCenter, trashPrefab, lightPrefab);
    }

    private static void CreateRoadMarkings(Transform parent, Vector3 cityCenter, int halfGrid, float spacing, Material laneMaterial)
    {
        float total = spacing * (halfGrid * 2 + 1);
        for (int i = -halfGrid; i <= halfGrid; i++)
        {
            float offset = i * spacing;
            for (int segment = -4; segment <= 4; segment++)
            {
                CreateRoadCube(parent, cityCenter + new Vector3(offset, 0.13f, segment * 28f), new Vector3(0.28f, 0.025f, 10f), laneMaterial, "Lane_Marking_NS");
                CreateRoadCube(parent, cityCenter + new Vector3(segment * 28f, 0.14f, offset), new Vector3(10f, 0.025f, 0.28f), laneMaterial, "Lane_Marking_EW");
            }

            CreateRoadCube(parent, cityCenter + new Vector3(offset + 3.7f, 0.13f, 0f), new Vector3(0.16f, 0.025f, total), laneMaterial, "Road_Edge_NS");
            CreateRoadCube(parent, cityCenter + new Vector3(offset - 3.7f, 0.13f, 0f), new Vector3(0.16f, 0.025f, total), laneMaterial, "Road_Edge_NS");
            CreateRoadCube(parent, cityCenter + new Vector3(0f, 0.14f, offset + 3.7f), new Vector3(total, 0.025f, 0.16f), laneMaterial, "Road_Edge_EW");
            CreateRoadCube(parent, cityCenter + new Vector3(0f, 0.14f, offset - 3.7f), new Vector3(total, 0.025f, 0.16f), laneMaterial, "Road_Edge_EW");
        }
    }

    private static void CreateSkyscraperBlock(
        Transform parent,
        Vector3 center,
        Material towerMaterial,
        Material windowMaterial,
        Material damagedMaterial,
        Material rubbleMaterial,
        int index)
    {
        int towers = Random.Range(1, 3);
        for (int i = 0; i < towers; i++)
        {
            Vector3 localOffset = new Vector3(Random.Range(-6f, 6f), 0f, Random.Range(-6f, 6f));
            float width = Random.Range(7f, 11f);
            float depth = Random.Range(7f, 11f);
            float height = Random.Range(30f, 58f);

            GameObject tower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tower.name = "Abandoned_Skyscraper";
            tower.transform.parent = parent;
            tower.transform.position = center + localOffset + new Vector3(0f, height * 0.5f, 0f);
            tower.transform.localScale = new Vector3(width, height, depth);
            tower.GetComponent<Renderer>().sharedMaterial = towerMaterial;

            CreateWindowGrid(tower.transform, width, height, depth, windowMaterial);

            if (index % 4 == 0)
            {
                GameObject wound = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wound.name = "Broken_Facade";
                wound.transform.parent = tower.transform;
                wound.transform.localPosition = new Vector3(0f, Random.Range(-0.15f, 0.25f), -0.51f);
                wound.transform.localScale = new Vector3(0.65f, 0.24f, 0.04f);
                wound.GetComponent<Renderer>().sharedMaterial = damagedMaterial;
            }

            AddRubble(tower.transform, rubbleMaterial);
        }
    }

    private static void CreateWindowGrid(Transform tower, float width, float height, float depth, Material windowMaterial)
    {
        int floors = Mathf.Clamp(Mathf.RoundToInt(height / 5f), 5, 11);
        int columns = Mathf.Clamp(Mathf.RoundToInt(width / 2.5f), 3, 5);
        for (int floor = 1; floor < floors; floor++)
        {
            for (int column = 0; column < columns; column++)
            {
                float x = -0.38f + column * (0.76f / Mathf.Max(1, columns - 1));
                float y = -0.48f + floor * (0.9f / floors);
                CreateWindow(tower, new Vector3(x, y, -0.505f), new Vector3(0.08f, 0.035f, 0.01f), windowMaterial, "Front_Window");
                CreateWindow(tower, new Vector3(x, y, 0.505f), new Vector3(0.08f, 0.035f, 0.01f), windowMaterial, "Back_Window");
            }
        }
    }

    private static void CreateWindow(Transform parent, Vector3 localPosition, Vector3 localScale, Material material, string name)
    {
        GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
        window.name = name;
        window.transform.parent = parent;
        window.transform.localPosition = localPosition;
        window.transform.localScale = localScale;
        window.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void CreateHouseBlock(Transform parent, Vector3 center, GameObject houseA, GameObject houseB, Material rubbleMaterial, int index)
    {
        GameObject housePrefab = index % 2 == 0 ? houseA : houseB;
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-6f, 0f, -5f),
            new Vector3(6f, 0f, -5f),
            new Vector3(-6f, 0f, 6f),
            new Vector3(6f, 0f, 6f)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            GameObject house = InstantiatePrefab(housePrefab, parent, center + offsets[i], Quaternion.Euler(0f, 90f * i, 0f), Random.Range(1.1f, 1.55f), "Ordered_Ruined_House");
            AddRubble(house.transform, rubbleMaterial);
        }
    }

    private static void CreateCrashScenes(Transform parent, Vector3 cityCenter, float spacing, GameObject carPrefab, Material rubbleMaterial, Material damagedMaterial)
    {
        Vector3[] crashCenters = new Vector3[]
        {
            cityCenter + new Vector3(-spacing * 2.5f, 0.08f, -spacing * 1.5f),
            cityCenter + new Vector3(spacing * 1.5f, 0.08f, -spacing * 2.5f),
            cityCenter + new Vector3(spacing * 2.5f, 0.08f, spacing * 1.5f),
            cityCenter + new Vector3(-spacing * 1.5f, 0.08f, spacing * 2.5f),
            cityCenter + new Vector3(0f, 0.08f, -spacing * 3.5f)
        };

        for (int i = 0; i < crashCenters.Length; i++)
        {
            GameObject carA = InstantiatePrefab(carPrefab, parent, crashCenters[i], Quaternion.Euler(0f, i * 35f, i % 2 == 0 ? 88f : -8f), 1.25f, i % 2 == 0 ? "Flipped_Abandoned_Car" : "Crashed_Abandoned_Car");
            GameObject carB = InstantiatePrefab(carPrefab, parent, crashCenters[i] + new Vector3(4f, 0f, Random.Range(-3f, 3f)), Quaternion.Euler(0f, 170f + i * 20f, 0f), 1.15f, "Collided_Abandoned_Car");
            TiltObject(carA, i % 2 == 0 ? 0f : -6f, i % 2 == 0 ? 0f : 10f);
            TiltObject(carB, Random.Range(-5f, 5f), Random.Range(-7f, 7f));
            CreateCrashBarrier(parent, crashCenters[i] + new Vector3(-3f, 0.8f, 4f), damagedMaterial);
            AddLooseRubble(parent, crashCenters[i], rubbleMaterial);
        }
    }

    private static void CreateCrashBarrier(Transform parent, Vector3 position, Material material)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Crashed_Into_Wall";
        wall.transform.parent = parent;
        wall.transform.position = position;
        wall.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), Random.Range(-12f, 12f));
        wall.transform.localScale = new Vector3(7f, 1.8f, 0.5f);
        wall.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void AddLooseRubble(Transform parent, Vector3 center, Material material)
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject rubble = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rubble.name = "Crash_Rubble";
            rubble.transform.parent = parent;
            rubble.transform.position = center + new Vector3(Random.Range(-6f, 6f), Random.Range(0.12f, 0.5f), Random.Range(-6f, 6f));
            rubble.transform.rotation = Random.rotation;
            rubble.transform.localScale = Vector3.one * Random.Range(0.25f, 0.8f);
            rubble.GetComponent<Renderer>().sharedMaterial = material;
        }
    }

    private static void CreateStreetDebris(Transform parent, Vector3 cityCenter, GameObject trashPrefab, GameObject lightPrefab)
    {
        for (int i = 0; i < 40; i++)
        {
            Vector3 trashPosition = cityCenter + new Vector3(Random.Range(-128f, 128f), 0.05f, Random.Range(-128f, 128f));
            InstantiatePrefab(trashPrefab, parent, trashPosition, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Random.Range(0.9f, 1.4f), "Street_Debris");
        }

        for (int i = 0; i < 18; i++)
        {
            Vector3 lightPosition = cityCenter + new Vector3(Random.Range(-126f, 126f), 0f, Random.Range(-126f, 126f));
            GameObject lamp = InstantiatePrefab(lightPrefab, parent, lightPosition, Quaternion.identity, 1.2f, "Dim_Street_Lamp");
            LimitLights(lamp);
        }
    }

    private static void CreateRoadCube(Transform parent, Vector3 position, Vector3 scale, Material material, string name)
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = name;
        road.transform.parent = parent;
        road.transform.position = position;
        road.transform.localScale = scale;
        road.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void CreateSidewalk(Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sidewalk.name = "Sidewalk";
        sidewalk.transform.parent = parent;
        sidewalk.transform.position = position;
        sidewalk.transform.localScale = scale;
        sidewalk.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static GameObject InstantiatePrefab(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, float scale, string name)
    {
        GameObject obj;
        if (prefab != null)
        {
            obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (obj == null)
            {
                obj = Object.Instantiate(prefab) as GameObject;
            }

            if (obj == null)
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        else
        {
            obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = position + Vector3.up;
            obj.transform.rotation = rotation;
        }

        obj.name = name;
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one * scale;
        return obj;
    }

    private static void AddRubble(Transform parent, Material material)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject rubble = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rubble.name = "Rubble";
            rubble.transform.parent = parent;
            rubble.transform.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(0.15f, 1.1f), Random.Range(-4f, 4f));
            rubble.transform.localRotation = Random.rotation;
            rubble.transform.localScale = new Vector3(Random.Range(0.35f, 1.1f), Random.Range(0.2f, 0.7f), Random.Range(0.35f, 1.1f));
            rubble.GetComponent<Renderer>().sharedMaterial = material;
        }
    }

    private static void TiltObject(GameObject obj, float xAngle, float zAngle)
    {
        if (obj == null)
        {
            return;
        }

        obj.transform.rotation *= Quaternion.Euler(xAngle, 0f, zAngle);
    }

    private static void LimitLights(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        Light[] lights = obj.GetComponentsInChildren<Light>();
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = Mathf.Min(lights[i].intensity, 0.42f);
            lights[i].range = Mathf.Min(lights[i].range, 7.5f);
        }
    }

    private static void CreateCitySpawn(Transform parent)
    {
        GameObject spawn = new GameObject("RuinedCity_PlayerSpawn");
        spawn.transform.parent = parent;
        spawn.transform.position = new Vector3(285f, 2f, 20f);
        spawn.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private static void CleanSceneVisualArtifacts()
    {
        PlayerHealth[] healthComponents = Resources.FindObjectsOfTypeAll<PlayerHealth>();
        for (int i = 0; i < healthComponents.Length; i++)
        {
            if (!IsSceneObject(healthComponents[i]))
            {
                continue;
            }

            if (healthComponents[i].gameOverPanel != null)
            {
                healthComponents[i].gameOverPanel.SetActive(false);
            }
        }

        Text[] texts = Resources.FindObjectsOfTypeAll<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            if (!IsSceneObject(texts[i]) || texts[i].text == null)
            {
                continue;
            }

            if (texts[i].text.ToLower().Contains("game over"))
            {
                GameObject panel = texts[i].transform.parent != null ? texts[i].transform.parent.gameObject : texts[i].gameObject;
                panel.SetActive(false);
            }
        }

        Image[] images = Resources.FindObjectsOfTypeAll<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            if (!IsSceneObject(images[i]))
            {
                continue;
            }

            RectTransform rect = images[i].GetComponent<RectTransform>();
            string lowerName = images[i].name.ToLower();
            bool largePanel = rect != null && rect.rect.width > 300f && rect.rect.height > 200f && images[i].color.a > 0.05f;
            if (lowerName.Contains("gameover") || lowerName.Contains("game over") || largePanel && lowerName.Contains("panel"))
            {
                images[i].gameObject.SetActive(false);
            }
        }

        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            if (!IsSceneObject(transforms[i]))
            {
                continue;
            }

            string lowerName = transforms[i].name.ToLower();
            if (lowerName.Contains("mobile") || lowerName.Contains("touchpad") || lowerName.Contains("joystick") || lowerName.Contains("thumbstick"))
            {
                transforms[i].gameObject.SetActive(false);
            }
        }

        ParticleSystem[] particles = Resources.FindObjectsOfTypeAll<ParticleSystem>();
        for (int i = 0; i < particles.Length; i++)
        {
            if (!IsSceneObject(particles[i]))
            {
                continue;
            }

            ParticleSystem.MainModule main = particles[i].main;
            main.playOnAwake = false;
            main.loop = false;
            particles[i].Stop();
            particles[i].Clear();
        }
    }

    private static bool IsSceneObject(Component component)
    {
        return component != null && component.gameObject != null && !EditorUtility.IsPersistent(component.gameObject);
    }

    private static void DisableEditorAnnotations()
    {
        System.Type annotationUtility = System.Type.GetType("UnityEditor.AnnotationUtility,UnityEditor");
        if (annotationUtility == null)
        {
            return;
        }

        MethodInfo getAnnotations = annotationUtility.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        MethodInfo setIconEnabled = annotationUtility.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (getAnnotations == null || setIconEnabled == null)
        {
            return;
        }

        System.Array annotations = getAnnotations.Invoke(null, null) as System.Array;
        if (annotations == null)
        {
            return;
        }

        for (int i = 0; i < annotations.Length; i++)
        {
            object annotation = annotations.GetValue(i);
            if (annotation == null)
            {
                continue;
            }

            FieldInfo classIdField = annotation.GetType().GetField("classID");
            FieldInfo scriptClassField = annotation.GetType().GetField("scriptClass");
            if (classIdField == null || scriptClassField == null)
            {
                continue;
            }

            int classId = (int)classIdField.GetValue(annotation);
            string scriptClass = scriptClassField.GetValue(annotation) as string;
            if (scriptClass == "AudioSource" || scriptClass == "ParticleSystem" || scriptClass == "Canvas" || scriptClass == "RectTransform")
            {
                setIconEnabled.Invoke(null, new object[] { classId, scriptClass, 0 });
            }
        }
    }

    private static void SetStaticRecursive(GameObject obj)
    {
        obj.isStatic = true;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetStaticRecursive(obj.transform.GetChild(i).gameObject);
        }
    }

    private static void TryBuildNavMesh()
    {
        System.Type builderType = System.Type.GetType("UnityEditor.AI.NavMeshBuilder,UnityEditor");
        if (builderType == null)
        {
            return;
        }

        MethodInfo method = builderType.GetMethod("BuildNavMesh", BindingFlags.Public | BindingFlags.Static);
        if (method != null)
        {
            method.Invoke(null, null);
        }
    }
}
