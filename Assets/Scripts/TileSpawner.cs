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

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
            // SpawnTile(turnTiles[0].GetComponent<Tile>());
            //AddNewDirection(Vector3.left);
        }

        private void SpawnTile(Tile tile, bool spawnObstacle = false)
        {
            Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);

            if (tile.type == TileType.STRAIGHT)
            {
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);

            }
            if (spawnObstacle) { SpawnCollider(collider.GetComponent<ColliderManager>(), player); }
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
                //Obstacle spawner
                SpawnTile(startingTile.GetComponent<Tile>(), i != 0);
            }

            SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>(), false);
        }

        private GameObject SelectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;
            return list[Random.Range(0, list.Count)];
        }

        public void SpawnObstacle(LightColor light, GameObject player)
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

        public void SpawnCollider(ColliderManager collider, GameObject player)
        {

            if (Random.value > 0.2f) return;

            collider.player = player;
            var playerColorMaterials = collider.player.GetComponent<PlayerController>().playerColorMaterials;

            GameObject colliderPrefab = collider.gameObject;
            var colliderRenderer = colliderPrefab.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();

            int colorIndex = Random.Range(0, (colorDictionary.Count - 1));
            var colliderColor = colorDictionary.ElementAt(colorIndex).Key;
            collider.color = colliderColor;

            var newMaterial = playerColorMaterials[colorIndex];
            colliderRenderer.material = newMaterial;


            Quaternion newObjectRotation = colliderPrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
            Vector3 newColliderLocation = new Vector3(currentTileLocation.x, currentTileLocation.y + 1.3f, currentTileLocation.z);
            GameObject obstacle = Instantiate(colliderPrefab, newColliderLocation, newObjectRotation);

            currentObstacles.Add(obstacle);

        }

    }

}