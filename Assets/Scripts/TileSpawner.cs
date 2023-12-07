using System.Collections.Generic;
using UnityEngine;
using TempleRun;
using Unity.VisualScripting;
using System.Linq;
using TempleRun.Player;

namespace SwingTheRoad
{
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] private int tileStartCount = 10;
        [SerializeField] private int minStraightTiles = 3;
        [SerializeField] private int maxStraightTiles = 3;

        [SerializeField] private GameObject startingTile;
        [SerializeField] private List<GameObject> turnTiles;
        [SerializeField] private List<GameObject> obstacles;

        [SerializeField] private GameObject light;
        [SerializeField] private GameObject player;

        [SerializeField] private GameObject collider;

        private Vector3 currentTileLocation = Vector3.zero;
        private Vector3 currentTileDirection = Vector3.forward;
        private GameObject prevTile;

        private List<GameObject> currentTiles;
        private List<GameObject> currentObstacles;

        public Dictionary<string, Color> colorDictionary = new Dictionary<string, Color>()
        {
            { "red", Color.red },
            { "blue", Color.blue },
            { "yellow", Color.yellow },
            { "black", Color.black }
        };


        private void Start()
        {
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();

            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = 0; i < tileStartCount; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>(), true);
            }

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), true);

        }

        private void SpawnTile(Tile tile, bool spawnObstacle = true)
        {
            Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);

            if (tile.type == TileType.STRAIGHT)
            {
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);

            }
            if (spawnObstacle)
            {
                float randNum = Random.Range(1, 10);
                if (randNum <= 5)
                {
                    SpawnLight(light.GetComponent<LightColor>(), player);
                }
                else
                {
                    BatchSpawn(collider.GetComponent<ColliderManager>(), player);
                }
            }
        }

        public void DeletePreviousTiles()
        {
            // use object pooling
            while (currentTiles.Count != 1)
            {
                GameObject tile = currentTiles[0];
                currentTiles.RemoveAt(0);
                Destroy(tile);
            }
            while (currentObstacles.Count != 0)
            {
                GameObject obstacle = currentObstacles[0];
                currentObstacles.RemoveAt(0);
                Destroy(obstacle);
            }
        }

        public void AddNewDirection(Vector3 direction)
        {
            currentTileDirection = direction;
            DeletePreviousTiles();

            Vector3 tilePlacementScale;
            if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
            {
                tilePlacementScale = Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size / 2 +
                    (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);

            }
            else
            {
                tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size -
                 (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);

            }

            currentTileLocation += tilePlacementScale;
            int currentPathLenght = Random.Range(minStraightTiles, maxStraightTiles);

            for (int i = 0; i < currentPathLenght; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>());
            }

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), true);
        }

        private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;
            return list[Random.Range(0, list.Count)];
        }

        public void SpawnLight(LightColor light, GameObject player)
        {

            if (Random.value > 0.2f) return;

            light.player = player;

            GameObject obstaclePrefab = light.gameObject;
            GameObject pointLight = obstaclePrefab.transform.GetChild(0).gameObject;

            int colorIndex = Random.Range(0, (colorDictionary.Count - 1));
            var lightColor = colorDictionary.ElementAt(colorIndex).Value;
            var lightColorName = colorDictionary.ElementAt(colorIndex).Key;

            light.color = lightColorName;
            pointLight.GetComponent<Light>().color = lightColor;

            Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
            currentObstacles.Add(obstacle);

        }

        public void SpawnCollider(ColliderManager collider, GameObject player, int colorIndex, Vector3 newColliderLocation)
        {

            collider.player = player;
            var playerColorMaterials = collider.player.GetComponent<PlayerController>().playerColorMaterials;

            GameObject colliderPrefab = collider.gameObject;
            var colliderRenderer = colliderPrefab.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();

            var colliderColor = colorDictionary.ElementAt(colorIndex).Key;
            collider.color = colliderColor;

            var newMaterial = playerColorMaterials[colorIndex];
            colliderRenderer.material = newMaterial;


            Quaternion newObjectRotation = colliderPrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
            GameObject obstacle = Instantiate(colliderPrefab, newColliderLocation, newObjectRotation);

            currentObstacles.Add(obstacle);

        }

        public void BatchSpawn(ColliderManager collider, GameObject player)
        {
            int rows = (int)Random.Range(1, 5);
            for (int i = 0; i < rows; i++)
            {
                string[] rowColors = { "", "", "" };
                for (int j = 0; j < 3; j++)
                {
                    float posX = 5f; float posZ = 5f; float posY = 1.3f;
                    var playerColor = player.gameObject.GetComponent<PlayerController>().playerColor;

                    int colorIndex = Random.Range(0, (colorDictionary.Count - 1));
                    var colliderColor = colorDictionary.ElementAt(colorIndex).Key;
                    Vector3 colliderLocation = new Vector3(currentTileLocation.x + (float)(j - 1) * posX, currentTileLocation.y + posY, currentTileLocation.z + posZ);
                    rowColors[j] = colliderColor;
                    SpawnCollider(collider.GetComponent<ColliderManager>(), player, colorIndex, colliderLocation);
                }
            }

        }

    }

}