using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class LevelBuilder : MonoBehaviour
{
    [MenuItem("PA4/Generar Nivel Completo")]
    public static void GenerateLevel()
    {
        // Create a new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Create floor (36x36)
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.position = new Vector3(0, 0, 0);
        floor.transform.localScale = new Vector3(36, 1, 36);

        // Create perimeter walls
        float wallHeight = 5f;
        float wallThickness = 1f;
        float floorSize = 36f;
        float halfSize = floorSize / 2f;

        // North wall
        GameObject wallNorth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallNorth.name = "WallNorth";
        wallNorth.transform.position = new Vector3(0, wallHeight/2, halfSize);
        wallNorth.transform.localScale = new Vector3(floorSize + wallThickness*2, wallHeight, wallThickness);
        // South wall
        GameObject wallSouth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallSouth.name = "WallSouth";
        wallSouth.transform.position = new Vector3(0, wallHeight/2, -halfSize);
        wallSouth.transform.localScale = new Vector3(floorSize + wallThickness*2, wallHeight, wallThickness);
        // East wall
        GameObject wallEast = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallEast.name = "WallEast";
        wallEast.transform.position = new Vector3(halfSize, wallHeight/2, 0);
        wallEast.transform.localScale = new Vector3(wallThickness, wallHeight, floorSize + wallThickness*2);
        // West wall
        GameObject wallWest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallWest.name = "WallWest";
        wallWest.transform.position = new Vector3(-halfSize, wallHeight/2, 0);
        wallWest.transform.localScale = new Vector3(wallThickness, wallHeight, floorSize + wallThickness*2);

        // Create 4 open obstacles (columns)
        float obstacleSize = 3f;
        float obstacleHeight = 4f;
        Vector3[] obstaclePositions = {
            new Vector3(-10, obstacleHeight/2, -10),
            new Vector3(10, obstacleHeight/2, -10),
            new Vector3(-10, obstacleHeight/2, 10),
            new Vector3(10, obstacleHeight/2, 10)
        };
        foreach (var pos in obstaclePositions)
        {
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = "Obstacle_" + pos;
            obstacle.transform.position = pos;
            obstacle.transform.localScale = new Vector3(obstacleSize, obstacleHeight, obstacleSize);
        }

        // Instantiate Player at (-14, 1.2, -14) without native CapsuleCollider and with CharacterController + tag "Player"
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(-14f, 1.2f, -14f);
        // Remove the CapsuleCollider component
        DestroyImmediate(player.GetComponent<CapsuleCollider>());
        // Add CharacterController
        player.AddComponent<CharacterController>();
        // Add our PlayerMovement script
        player.AddComponent<PlayerMovement>();
        // Tag as Player
        player.tag = "Player";

        // Instantiate Enemy at (14, 1.2, 14) with EnemyAI and 3 Waypoints lejanos
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        enemy.name = "Enemy";
        enemy.transform.position = new Vector3(14f, 1.2f, 14f);
        // Remove any NavMeshAgent if present (should not be, but just in case)
        var navMeshAgent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent != null)
        {
            DestroyImmediate(navMeshAgent);
        }
        // Add EnemyAI (our updated version without NavMesh)
        enemy.AddComponent<EnemyAI>();
        // Create three waypoints far away
        GameObject wp1 = new GameObject("Waypoint1");
        wp1.transform.position = new Vector3(-16f, 1.2f, -16f);
        GameObject wp2 = new GameObject("Waypoint2");
        wp2.transform.position = new Vector3(16f, 1.2f, 16f);
        GameObject wp3 = new GameObject("Waypoint3");
        wp3.transform.position = new Vector3(-16f, 1.2f, 16f);
        // Assign waypoints to EnemyAI
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        enemyAI.waypoints = new Transform[] { wp1.transform, wp2.transform, wp3.transform };

        // Distribute 10 coins in open zones
        for (int i = 0; i < 10; i++)
        {
            GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coin.name = "Coin_" + i;
            // Place coin in open areas
            float x = Random.Range(-15f, 15f);
            float z = Random.Range(-15f, 15f);
            coin.transform.position = new Vector3(x, 0.5f, z);
            coin.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            // Make it lie flat like a coin
            coin.transform.Rotate(90f, 0f, 0f);
            // Add Coin script
            coin.AddComponent<Coin>();
            // Set collider as trigger
            Collider coinCollider = coin.GetComponent<Collider>();
            coinCollider.isTrigger = true;
        }

        // Instantiate GameManager, SimpleUI, and ObjectPooler
        GameObject gameManagerGO = new GameObject("GameManager");
        gameManagerGO.AddComponent<GameManager>();

        GameObject simpleUIGO = new GameObject("SimpleUI");
        simpleUIGO.AddComponent<SimpleUI>();

        GameObject objectPoolerGO = new GameObject("ObjectPooler");
        objectPoolerGO.AddComponent<ObjectPooler>();

        // Set up Main Camera: add SmoothCameraFollow and assign target
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        if (mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
        if (mainCamera != null)
        {
            var cameraFollow = mainCamera.AddComponent<SmoothCameraFollow>();
            // The SmoothCameraFollow will find the player in Start, but we can set it now for editor convenience.
            // However, we leave it to Find in Start.
        }
        else
        {
            // Create a camera if none exists
            mainCamera = new GameObject("Main Camera");
            mainCamera.AddComponent<Camera>();
            mainCamera.tag = "MainCamera";
            var cameraFollow = mainCamera.AddComponent<SmoothCameraFollow>();
            // Position it roughly
            mainCamera.transform.position = new Vector3(0, 6, -8);
            mainCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
        }

        // Save the scene
        string savePath = "Assets/Scenes/NivelPrincipal.unity";
        // Ensure the Scenes directory exists
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
        EditorSceneManager.SaveScene(newScene, savePath);

        Debug.Log("Nivel generado y guardado en: " + savePath);
    }
}