using UnityEngine;

public class RuinedCityBuilder : MonoBehaviour
{
    public Vector3 cityCenter = new Vector3(285f, 0f, 150f);
    public float cityRadius = 165f;
    public float largeGroundSize = 620f;
    public int roadHalfGrid = 5;
    public float blockSpacing = 30f;
    public Material wallMaterial;
    public Material roadMaterial;
    public Transform playerSpawnPoint;
    public bool spawnAmbientAnimalsInForest = true;

    private GameObject houseOnePrefab;
    private GameObject houseTwoPrefab;
    private GameObject roadPrefab;
    private GameObject carPrefab;
    private GameObject trashPrefab;
    private GameObject lightPrefab;
    private GameObject deerPrefab;
    private GameObject rabbitPrefab;
    private GameObject chickenPrefab;

    private Material forestMaterial;
    private Material asphaltMaterial;
    private Material sidewalkMaterial;
    private Material laneMaterial;
    private Material trunkMaterial;
    private Material leavesMaterial;
    private Material towerMaterial;
    private Material windowMaterial;
    private Material rubbleMaterial;
    private Material damagedWallMaterial;
    private Material dirtMaterial;

    void Start()
    {
        BuildCity();
    }

    public void BuildCity()
    {
        // Runtime rebuild keeps the playable view correct even when the binary scene cannot be edited safely.
        if (transform.childCount > 0)
        {
            return;
        }

        Random.InitState(250705);
        LoadResourcePrefabs();
        CreateRuntimeMaterials();
        CreateLargeGround();
        CreateForestZone();
        CreateCityRoadGrid();
        CreateCityBlocks();
        CreateCrashScenes();
        CreateStreetDebris();
        CreateAmbientAnimals();
        CreatePlayerSpawn();
    }

    private void LoadResourcePrefabs()
    {
        // These prefabs are copied into Assets/Resources so Unity 5 can load them at runtime.
        houseOnePrefab = Resources.Load("RuntimeCity/House_01") as GameObject;
        houseTwoPrefab = Resources.Load("RuntimeCity/House_16") as GameObject;
        roadPrefab = Resources.Load("RuntimeCity/Road_10") as GameObject;
        carPrefab = Resources.Load("RuntimeCity/Car_03") as GameObject;
        trashPrefab = Resources.Load("RuntimeCity/Trash_01") as GameObject;
        lightPrefab = Resources.Load("RuntimeCity/Light_01") as GameObject;
        deerPrefab = Resources.Load("RuntimeAnimals/Deer_prefab") as GameObject;
        rabbitPrefab = Resources.Load("RuntimeAnimals/Wild_rabbit_prefab") as GameObject;
        chickenPrefab = Resources.Load("RuntimeAnimals/Chicken_prefab") as GameObject;
    }

    private void CreateRuntimeMaterials()
    {
        forestMaterial = CreateMaterial(new Color(0.15f, 0.24f, 0.13f));
        asphaltMaterial = roadMaterial != null ? roadMaterial : CreateMaterial(new Color(0.055f, 0.055f, 0.055f));
        sidewalkMaterial = CreateMaterial(new Color(0.34f, 0.34f, 0.32f));
        laneMaterial = CreateMaterial(new Color(0.86f, 0.82f, 0.62f));
        trunkMaterial = CreateMaterial(new Color(0.27f, 0.18f, 0.11f));
        leavesMaterial = CreateMaterial(new Color(0.09f, 0.29f, 0.11f));
        towerMaterial = CreateMaterial(new Color(0.23f, 0.24f, 0.24f));
        windowMaterial = CreateMaterial(new Color(0.035f, 0.06f, 0.08f));
        rubbleMaterial = CreateMaterial(new Color(0.42f, 0.39f, 0.35f));
        damagedWallMaterial = wallMaterial != null ? wallMaterial : CreateMaterial(new Color(0.19f, 0.19f, 0.18f));
        dirtMaterial = CreateMaterial(new Color(0.22f, 0.19f, 0.13f));
    }

    private Material CreateMaterial(Color color)
    {
        Material material = new Material(Shader.Find("Diffuse"));
        material.color = color;
        return material;
    }

    private void CreateLargeGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Expanded_Forest_And_City_Ground";
        ground.transform.parent = transform;
        ground.transform.localPosition = new Vector3(165f, -0.08f, 115f);
        ground.transform.localScale = new Vector3(largeGroundSize, 0.16f, largeGroundSize * 0.86f);
        ApplyMaterial(ground, forestMaterial);

        GameObject cityBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cityBase.name = "City_Block_Base";
        cityBase.transform.parent = transform;
        cityBase.transform.localPosition = cityCenter + new Vector3(0f, -0.02f, 0f);
        cityBase.transform.localScale = new Vector3(cityRadius * 2.05f, 0.08f, cityRadius * 2.05f);
        ApplyMaterial(cityBase, CreateMaterial(new Color(0.12f, 0.13f, 0.12f)));

        GameObject entranceRoad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        entranceRoad.name = "Forest_To_City_Main_Road";
        entranceRoad.transform.parent = transform;
        entranceRoad.transform.localPosition = new Vector3(132f, 0.04f, 35f);
        entranceRoad.transform.localScale = new Vector3(270f, 0.08f, 8.5f);
        ApplyMaterial(entranceRoad, asphaltMaterial);
    }

    private void CreateForestZone()
    {
        for (int i = 0; i < 230; i++)
        {
            Vector3 position = new Vector3(Random.Range(-135f, 235f), 0f, Random.Range(-120f, 330f));
            bool insideCity = Mathf.Abs(position.x - cityCenter.x) < cityRadius + 18f && Mathf.Abs(position.z - cityCenter.z) < cityRadius + 18f;
            bool onEntranceRoad = Mathf.Abs(position.z - 35f) < 13f && position.x > -10f && position.x < 255f;
            if (insideCity || onEntranceRoad)
            {
                continue;
            }

            CreateTree(position, Random.Range(0.75f, 1.85f));
        }

        for (int i = 0; i < 38; i++)
        {
            Vector3 edgePosition = new Vector3(218f + Random.Range(-8f, 10f), 0f, -95f + i * 9.4f);
            CreateTree(edgePosition, Random.Range(0.85f, 1.55f));
        }
    }

    private void CreateTree(Vector3 localPosition, float scale)
    {
        GameObject tree = new GameObject("Forest_Tree");
        tree.transform.parent = transform;
        tree.transform.localPosition = localPosition;

        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.parent = tree.transform;
        trunk.transform.localPosition = new Vector3(0f, 1.2f * scale, 0f);
        trunk.transform.localScale = new Vector3(0.32f * scale, 1.5f * scale, 0.32f * scale);
        ApplyMaterial(trunk, trunkMaterial);

        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.name = "Leaves";
        leaves.transform.parent = tree.transform;
        leaves.transform.localPosition = new Vector3(0f, 3.05f * scale, 0f);
        leaves.transform.localScale = new Vector3(2.0f * scale, 1.45f * scale, 2.0f * scale);
        ApplyMaterial(leaves, leavesMaterial);
    }

    private void CreateCityRoadGrid()
    {
        float totalLength = blockSpacing * (roadHalfGrid * 2 + 1);
        GameObject roadRoot = new GameObject("Ordered_Road_Network");
        roadRoot.transform.parent = transform;

        for (int i = -roadHalfGrid; i <= roadHalfGrid; i++)
        {
            float offset = i * blockSpacing;
            CreateRoadCube(roadRoot.transform, cityCenter + new Vector3(offset, 0.06f, 0f), new Vector3(8f, 0.09f, totalLength), asphaltMaterial, "NorthSouth_Road");
            CreateRoadCube(roadRoot.transform, cityCenter + new Vector3(0f, 0.07f, offset), new Vector3(totalLength, 0.09f, 8f), asphaltMaterial, "EastWest_Road");

            CreateRoadCube(roadRoot.transform, cityCenter + new Vector3(offset + 5.4f, 0.12f, 0f), new Vector3(1.7f, 0.08f, totalLength), sidewalkMaterial, "NorthSouth_Sidewalk");
            CreateRoadCube(roadRoot.transform, cityCenter + new Vector3(offset - 5.4f, 0.12f, 0f), new Vector3(1.7f, 0.08f, totalLength), sidewalkMaterial, "NorthSouth_Sidewalk");
            CreateRoadCube(roadRoot.transform, cityCenter + new Vector3(0f, 0.13f, offset + 5.4f), new Vector3(totalLength, 0.08f, 1.7f), sidewalkMaterial, "EastWest_Sidewalk");
            CreateRoadCube(roadRoot.transform, cityCenter + new Vector3(0f, 0.13f, offset - 5.4f), new Vector3(totalLength, 0.08f, 1.7f), sidewalkMaterial, "EastWest_Sidewalk");
        }

        CreateLaneMarkings(roadRoot.transform, totalLength);
    }

    private void CreateLaneMarkings(Transform parent, float totalLength)
    {
        for (int i = -roadHalfGrid; i <= roadHalfGrid; i++)
        {
            float offset = i * blockSpacing;
            for (int segment = -5; segment <= 5; segment++)
            {
                CreateRoadCube(parent, cityCenter + new Vector3(offset, 0.16f, segment * 27f), new Vector3(0.28f, 0.025f, 9f), laneMaterial, "Lane_Marking_NS");
                CreateRoadCube(parent, cityCenter + new Vector3(segment * 27f, 0.17f, offset), new Vector3(9f, 0.025f, 0.28f), laneMaterial, "Lane_Marking_EW");
            }

            CreateRoadCube(parent, cityCenter + new Vector3(offset + 4.05f, 0.16f, 0f), new Vector3(0.16f, 0.025f, totalLength), laneMaterial, "Road_Edge_NS");
            CreateRoadCube(parent, cityCenter + new Vector3(offset - 4.05f, 0.16f, 0f), new Vector3(0.16f, 0.025f, totalLength), laneMaterial, "Road_Edge_NS");
            CreateRoadCube(parent, cityCenter + new Vector3(0f, 0.17f, offset + 4.05f), new Vector3(totalLength, 0.025f, 0.16f), laneMaterial, "Road_Edge_EW");
            CreateRoadCube(parent, cityCenter + new Vector3(0f, 0.17f, offset - 4.05f), new Vector3(totalLength, 0.025f, 0.16f), laneMaterial, "Road_Edge_EW");
        }
    }

    private void CreateCityBlocks()
    {
        GameObject buildingRoot = new GameObject("Ordered_Ruined_City_Blocks");
        buildingRoot.transform.parent = transform;

        int index = 0;
        for (int x = -roadHalfGrid; x < roadHalfGrid; x++)
        {
            for (int z = -roadHalfGrid; z < roadHalfGrid; z++)
            {
                Vector3 blockCenter = cityCenter + new Vector3(x * blockSpacing + blockSpacing * 0.5f, 0f, z * blockSpacing + blockSpacing * 0.5f);
                bool downtown = Mathf.Abs(x) <= 2 && Mathf.Abs(z) <= 2;
                bool towerBlock = downtown || (x + z) % 4 == 0;
                if (towerBlock)
                {
                    CreateSkyscraperBlock(buildingRoot.transform, blockCenter, index);
                }
                else
                {
                    CreateHouseBlock(buildingRoot.transform, blockCenter, index);
                }

                index++;
            }
        }
    }

    private void CreateSkyscraperBlock(Transform parent, Vector3 center, int index)
    {
        int towerCount = index % 5 == 0 ? 2 : 1;
        for (int i = 0; i < towerCount; i++)
        {
            float width = Random.Range(7.5f, 11f);
            float depth = Random.Range(7.5f, 11f);
            float height = Random.Range(34f, 64f);
            Vector3 offset = new Vector3(Random.Range(-5.5f, 5.5f), 0f, Random.Range(-5.5f, 5.5f));

            GameObject tower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tower.name = "Abandoned_Skyscraper";
            tower.transform.parent = parent;
            tower.transform.localPosition = center + offset + new Vector3(0f, height * 0.5f, 0f);
            tower.transform.localScale = new Vector3(width, height, depth);
            ApplyMaterial(tower, towerMaterial);

            CreateWindowRows(tower.transform, width, height);
            if ((index + i) % 3 == 0)
            {
                CreateBrokenFacade(tower.transform);
            }

            AddRubble(parent, center + offset, 6);
        }
    }

    private void CreateWindowRows(Transform tower, float width, float height)
    {
        int floors = Mathf.Clamp(Mathf.RoundToInt(height / 5f), 6, 12);
        int columns = Mathf.Clamp(Mathf.RoundToInt(width / 2.3f), 3, 5);
        for (int floor = 1; floor < floors; floor++)
        {
            for (int column = 0; column < columns; column++)
            {
                float x = -0.38f + column * (0.76f / Mathf.Max(1, columns - 1));
                float y = -0.45f + floor * (0.86f / floors);
                CreateWindow(tower, new Vector3(x, y, -0.505f), new Vector3(0.075f, 0.03f, 0.01f));
                CreateWindow(tower, new Vector3(x, y, 0.505f), new Vector3(0.075f, 0.03f, 0.01f));
            }
        }
    }

    private void CreateWindow(Transform parent, Vector3 localPosition, Vector3 localScale)
    {
        GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
        window.name = "Dark_Broken_Window";
        window.transform.parent = parent;
        window.transform.localPosition = localPosition;
        window.transform.localScale = localScale;
        ApplyMaterial(window, windowMaterial);
    }

    private void CreateBrokenFacade(Transform parent)
    {
        GameObject wound = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wound.name = "Broken_Facade";
        wound.transform.parent = parent;
        wound.transform.localPosition = new Vector3(0f, Random.Range(-0.18f, 0.2f), -0.512f);
        wound.transform.localScale = new Vector3(0.68f, 0.22f, 0.045f);
        ApplyMaterial(wound, damagedWallMaterial);
    }

    private void CreateHouseBlock(Transform parent, Vector3 center, int index)
    {
        GameObject prefab = index % 2 == 0 ? houseOnePrefab : houseTwoPrefab;
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-6.6f, 0f, -5.8f),
            new Vector3(6.4f, 0f, -5.8f),
            new Vector3(-6.4f, 0f, 6.2f),
            new Vector3(6.2f, 0f, 6.1f)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            GameObject house = SpawnPrefab(prefab, center + offsets[i], 90f * i, Random.Range(1.05f, 1.45f), "Ordered_Abandoned_House");
            house.transform.parent = parent;
            TintRenderers(house, new Color(0.66f, 0.63f, 0.56f));
            CreateBrokenWalls(house.transform);
        }
    }

    private void CreateBrokenWalls(Transform parent)
    {
        int count = Random.Range(1, 4);
        for (int i = 0; i < count; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "BrokenWall_Detail";
            wall.transform.parent = parent;
            wall.transform.localPosition = new Vector3(Random.Range(-3.2f, 3.2f), Random.Range(1.0f, 2.4f), Random.Range(-3.2f, 3.2f));
            wall.transform.localRotation = Quaternion.Euler(Random.Range(-8f, 8f), Random.Range(0f, 180f), Random.Range(-8f, 8f));
            wall.transform.localScale = new Vector3(Random.Range(1.5f, 3.8f), Random.Range(1.0f, 2.7f), Random.Range(0.2f, 0.45f));
            ApplyMaterial(wall, damagedWallMaterial);
        }
    }

    private void CreateCrashScenes()
    {
        GameObject crashRoot = new GameObject("Crashed_Abandoned_Cars");
        crashRoot.transform.parent = transform;

        Vector3[] crashCenters = new Vector3[]
        {
            cityCenter + new Vector3(-95f, 0.2f, -55f),
            cityCenter + new Vector3(-30f, 0.2f, -112f),
            cityCenter + new Vector3(62f, 0.2f, -80f),
            cityCenter + new Vector3(104f, 0.2f, 34f),
            cityCenter + new Vector3(-84f, 0.2f, 88f),
            cityCenter + new Vector3(18f, 0.2f, 118f)
        };

        for (int i = 0; i < crashCenters.Length; i++)
        {
            GameObject carA = SpawnPrefab(carPrefab, crashCenters[i], 22f + i * 37f, 1.18f, i % 2 == 0 ? "Flipped_Abandoned_Car" : "Crashed_Abandoned_Car");
            carA.transform.parent = crashRoot.transform;
            if (i % 2 == 0)
            {
                carA.transform.rotation *= Quaternion.Euler(0f, 0f, 88f);
            }
            else
            {
                carA.transform.rotation *= Quaternion.Euler(-8f, 0f, 10f);
            }

            GameObject carB = SpawnPrefab(carPrefab, crashCenters[i] + new Vector3(4.5f, 0f, Random.Range(-3.5f, 3.5f)), 170f + i * 18f, 1.08f, "Collided_Abandoned_Car");
            carB.transform.parent = crashRoot.transform;
            carB.transform.rotation *= Quaternion.Euler(Random.Range(-5f, 5f), 0f, Random.Range(-7f, 7f));

            CreateCrashWall(crashRoot.transform, crashCenters[i] + new Vector3(-3.5f, 0.8f, 4f));
            AddRubble(crashRoot.transform, crashCenters[i], 10);
        }
    }

    private void CreateCrashWall(Transform parent, Vector3 position)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Car_Crashed_Into_Wall";
        wall.transform.parent = parent;
        wall.transform.localPosition = position;
        wall.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), Random.Range(-10f, 10f));
        wall.transform.localScale = new Vector3(7.2f, 1.8f, 0.55f);
        ApplyMaterial(wall, damagedWallMaterial);
    }

    private void CreateStreetDebris()
    {
        GameObject propsRoot = new GameObject("Street_Debris_And_Dim_Lights");
        propsRoot.transform.parent = transform;

        for (int i = 0; i < 55; i++)
        {
            Vector3 position = cityCenter + new Vector3(Random.Range(-150f, 150f), 0.08f, Random.Range(-150f, 150f));
            GameObject prop = SpawnPrefab(trashPrefab, position, Random.Range(0f, 360f), Random.Range(0.9f, 1.45f), "Street_Debris");
            prop.transform.parent = propsRoot.transform;
        }

        for (int i = 0; i < 28; i++)
        {
            float x = -roadHalfGrid + Random.Range(0, roadHalfGrid * 2 + 1);
            float z = -roadHalfGrid + Random.Range(0, roadHalfGrid * 2 + 1);
            Vector3 position = cityCenter + new Vector3(x * blockSpacing + Random.Range(-5.5f, 5.5f), 0f, z * blockSpacing + Random.Range(-5.5f, 5.5f));
            GameObject lamp = SpawnPrefab(lightPrefab, position, Random.Range(0f, 360f), 1.15f, "Dim_Street_Lamp");
            lamp.transform.parent = propsRoot.transform;
            LimitLampLight(lamp);
        }
    }

    private void AddRubble(Transform parent, Vector3 center, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject rubble = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rubble.name = "Rubble";
            rubble.transform.parent = parent;
            rubble.transform.localPosition = center + new Vector3(Random.Range(-6f, 6f), Random.Range(0.14f, 0.65f), Random.Range(-6f, 6f));
            rubble.transform.localRotation = Random.rotation;
            rubble.transform.localScale = new Vector3(Random.Range(0.25f, 1f), Random.Range(0.18f, 0.65f), Random.Range(0.25f, 1f));
            ApplyMaterial(rubble, rubbleMaterial);
        }
    }

    private void CreateAmbientAnimals()
    {
        if (!spawnAmbientAnimalsInForest)
        {
            return;
        }

        GameObject[] animalPrefabs = new GameObject[] { deerPrefab, rabbitPrefab, chickenPrefab };
        for (int i = 0; i < animalPrefabs.Length; i++)
        {
            if (animalPrefabs[i] == null)
            {
                continue;
            }

            for (int j = 0; j < 2; j++)
            {
                Vector3 position = new Vector3(Random.Range(-100f, 120f), 0.1f, Random.Range(-90f, 180f));
                GameObject animal = SpawnPrefab(animalPrefabs[i], position, Random.Range(0f, 360f), 0.8f, "Forest_Animal");
                if (animal.GetComponent("SimpleAnimalWander") == null)
                {
                    animal.AddComponent<SimpleAnimalWander>();
                }
            }
        }
    }

    private void CreatePlayerSpawn()
    {
        GameObject spawn = new GameObject("RuinedCity_PlayerSpawn");
        spawn.transform.parent = transform;
        spawn.transform.position = transform.TransformPoint(cityCenter + new Vector3(0f, 2f, -cityRadius * 0.78f));
        spawn.transform.rotation = Quaternion.LookRotation(transform.TransformPoint(cityCenter) - spawn.transform.position);
        playerSpawnPoint = spawn.transform;
    }

    private void CreateRoadCube(Transform parent, Vector3 localPosition, Vector3 scale, Material material, string name)
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = name;
        road.transform.parent = parent;
        road.transform.localPosition = localPosition;
        road.transform.localScale = scale;
        ApplyMaterial(road, material);

        if (roadPrefab != null && name == "EastWest_Road" && Mathf.Abs(localPosition.z - cityCenter.z) < 0.1f)
        {
            GameObject roadModel = Instantiate(roadPrefab, transform.TransformPoint(localPosition + new Vector3(0f, 0.08f, 0f)), Quaternion.identity) as GameObject;
            roadModel.name = "CityPack_Road_Detail";
            roadModel.transform.parent = parent;
            roadModel.transform.localScale = Vector3.one * 2.2f;
        }
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector3 localPosition, float yRotation, float scale, string objectName)
    {
        GameObject obj;
        if (prefab != null)
        {
            obj = Instantiate(prefab, transform.TransformPoint(localPosition), Quaternion.Euler(0f, yRotation, 0f)) as GameObject;
        }
        else
        {
            obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = transform.TransformPoint(localPosition + Vector3.up * 0.5f);
            obj.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            obj.transform.localScale = objectName.ToLower().Contains("car") ? new Vector3(2.8f, 0.8f, 1.35f) : Vector3.one;
        }

        obj.name = objectName;
        obj.transform.localScale = obj.transform.localScale * scale;
        return obj;
    }

    private void ApplyMaterial(GameObject obj, Material material)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.material = material;
        }
    }

    private void TintRenderers(GameObject obj, Color color)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                renderers[i].material.color = Color.Lerp(renderers[i].material.color, color, 0.35f);
            }
        }
    }

    private void LimitLampLight(GameObject lamp)
    {
        if (lamp == null)
        {
            return;
        }

        Light[] lights = lamp.GetComponentsInChildren<Light>();
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = Mathf.Min(lights[i].intensity, 0.45f);
            lights[i].range = Mathf.Min(lights[i].range, 7.5f);
        }
    }
}
