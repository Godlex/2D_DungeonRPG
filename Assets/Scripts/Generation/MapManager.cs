using Generation.Classes;
using Generation.Scriptables;
using UnityEngine;

namespace Generation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Controller;
    using Random = UnityEngine.Random;

    public class MapManager : MonoBehaviour
    {
        public GameObject Camera; 
        public bool randomizeSeed = true;
        public int chunkSize = 16;
        public MapConfig config;
        public Transform[,,] TransformsMap;
        public byte[,] Map;
        public GameObject Player;
        public GameObject Enemy;
        public Transform EartCloneTile;
        public int SpawnRate = 20;
        private Dictionary<GameObject, int> chunks = new Dictionary<GameObject, int>();
        private int leastFillChunk = Int32.MaxValue;
        private CustomCameraController CameraController;


        private void Start()
        {
            InitGenerator();
            SpawnMap();
            SpawnPlayerAndEnemy();
            CreateEarthClone();
            CameraController.SearchPlayerForCum();
        }

        private void InitGenerator()
        {
            if (randomizeSeed)
                config.seed = Random.Range(int.MinValue, int.MaxValue);

            MapGenerator generator = new MapGenerator(config);
            CameraController = Camera.GetComponent<CustomCameraController>();
            Map = generator.Generate();
        }


        private void SpawnMap()
        {
            int chunksMapSizeX = Map.GetLength(0) / chunkSize + 1;
            int chunksMapSizeY = Map.GetLength(1) / chunkSize + 1;

            TransformsMap = new Transform[config.maxMapWidth, config.maxMapHeight, 2];

            Transform[,] chunksMap = new Transform[chunksMapSizeX, chunksMapSizeY];

            Transform[,] chunksWallsMap = new Transform[chunksMapSizeX, chunksMapSizeY];

            for (int x = 0; x < chunksMapSizeX; x++)
            {
                for (int y = 0; y < chunksMapSizeY; y++)
                {
                    GameObject chunk = new GameObject($"Chunk {x} {y}");
                    chunk.transform.parent = transform;
                    chunksMap[x, y] = chunk.transform;


                    GameObject chunkWalls = new GameObject($"ChunkWalls {x} {y}");
                    chunkWalls.transform.parent = transform;
                    chunksWallsMap[x, y] = chunkWalls.transform;
                }
            }

            for (int x = 0; x < config.maxMapWidth; x++)
            {
                for (int y = 0; y < config.maxMapWidth; y++)
                {
                    byte tileId = Map[x, y];

                    if (tileId == MapGenerator.EarthId || tileId == MapGenerator.WallId)
                    {
                        int chunkX = x / chunkSize;
                        int chunkY = y / chunkSize;
                        Transform chunk = chunksMap[chunkX, chunkY];
                        Transform chunkWall = chunksWallsMap[chunkX, chunkY];
                        GameObject tile;

                        if (tileId == MapGenerator.WallId)
                        {
                            tile = Instantiate(config.tileWallPrefab, chunkWall, true);
                        }
                        else
                        {
                            tile = Instantiate(config.tileEarthPrefab, chunk, true);
                        }

                        tile.transform.position = new Vector3(x, y, 0);
                        MeshRenderer meshRenderer = tile.GetComponent<MeshRenderer>();
                        meshRenderer.material.color = config.colorsByIds[tileId];
                        TransformsMap[x, y, 0] = tile.transform;
                    }
                }
            }

            for (int x = 0; x < chunksMapSizeX; x++)
            {
                for (int y = 0; y < chunksMapSizeY; y++)
                {
                    Transform chunkWalls = chunksWallsMap[x, y];
                    if (chunkWalls.childCount <= 0)
                    {
                        Destroy(chunkWalls.gameObject);
                    }

                    Transform chunk = chunksMap[x, y];
                    if (chunk.childCount <= 0)
                    {
                        Destroy(chunk.gameObject);
                    }
                    else
                    {
                        chunks.Add(chunk.gameObject, chunk.childCount);
                        if (chunk.childCount < leastFillChunk)
                        {
                            leastFillChunk = chunk.childCount;
                        }
                    }
                }
            }
        }

        private void SpawnPlayerAndEnemy()
        {
            bool playerSpawn = false;
            GameObject Enemys = new GameObject("Enemys");
            foreach (var chunk in chunks)
            {
                if (chunk.Value == leastFillChunk)
                {
                    if (!playerSpawn)
                    {
                        PlayerSpawn(chunk, out playerSpawn);
                    }
                }
                else
                {
                    SpawnEnemy(chunk, Enemys);
                }
            }
        }

        private void SpawnEnemy(KeyValuePair<GameObject, int> chunk, GameObject enemys)
        {
            for (int i = 0; i <= chunk.Value; i += 20)
            {
                Transform quad = chunk.Key.transform.GetChild(Random.Range(0, chunk.Value));
                var position1 = quad.position;
                Vector3 position = new Vector3(position1.x, position1.y, position1.z - 1);
                Instantiate(Enemy, position, Quaternion.identity, enemys.transform);
            }
        }

        private void PlayerSpawn(KeyValuePair<GameObject, int> chunk, out bool playerSpawn)
        {
            Transform quad = chunk.Key.transform.GetChild(0);
            var position1 = quad.position;
            Vector3 position = new Vector3(position1.x, position1.y, position1.z - 1);
            GameObject sPlayer = Instantiate(Player, position, Quaternion.identity);
            sPlayer.name = "Player";
            playerSpawn = true;
        }

        private Transform MakeChunk(int x, int y)
        {
            GameObject chunkGameObject = new GameObject();

            int startX = x - x % 16;
            int startY = y - y % 16;
            chunkGameObject.name = $"Chunk({startX}, {startY})";

            Transform chunk = chunkGameObject.transform;
            chunk.parent = transform;
            return chunk;
        }

        private void CreateEarthClone()
        {
            var tile = Instantiate(config.tileEarthPrefab);
            MeshRenderer meshRenderer = tile.GetComponent<MeshRenderer>();
            meshRenderer.material.color = config.colorsByIds[1];
            tile.transform.position = new Vector3(-10, -10, -1);
            EartCloneTile = tile.transform;
        }
    }
}