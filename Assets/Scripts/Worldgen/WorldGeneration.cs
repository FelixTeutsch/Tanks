using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Utility;
using Random = System.Random;

namespace Worldgen
{
    public class WorldGeneration : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject treePrefab;
        [SerializeField] private GameObject rockPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private int numberOfTiles;

        [SerializeField] private float perlinScale = 0.1f;
        [SerializeField] private float perlinAmplitude = 5f;
        [SerializeField] private int perlinSeed = 42;
        [SerializeField] private float minBottomDistance = 0.5f;


        private GameObject[] _generatedRocks;
        private GameObject[] _generatedTiles;
        private GameObject[] _generatedTrees;

        private float _tileWidth;
        private float _worldBottom;
        private float _worldHeight;

        private float _worldWidth;

        private void Start()
        {
            InitialiseMap();
        }


        public void InitialiseMap()
        {
            Debug.Log("InitialiseMap START");
            // Get players from Game Manager
            var players = GameManager.instance.getPlayers;
            Debug.Log($"Number of Players: {players.Count}");
            Debug.Log($"Player 1: {players[0].name}");

            // randomize the seed
            perlinSeed = new Random().Next(0, 10000);
            perlinSeed = 7582; // This seed seems to kick a tank off the map

            // Get the width and height of the camera in world units
            _worldWidth = CameraUtility.GetCameraWidth(Camera.main);
            _worldHeight = CameraUtility.GetCameraHeight(Camera.main);

            // Calculate the width of each tile and the bottom of the world
            _tileWidth = _worldWidth / numberOfTiles;
            _worldBottom = -_worldHeight / 2;

            // Generate the world
            _generatedTiles = new GameObject[numberOfTiles];
            _generatedTrees = new GameObject[numberOfTiles];
            _generatedRocks = new GameObject[numberOfTiles];

            var heightMap = GenerateWorld();

            SpawnPlayers(heightMap, players);
            GameManager.instance.StartGame();
            Debug.Log("InitialiseMap END");
        }


        private void SpawnPlayers(float[] heightMap, List<PlayerObject> players)
        {
            Debug.Log("SpawnPlayers START");
            var numberOfPlayers = players.Count;

            // Calculate the spacing between players
            var spacing = _worldWidth / (numberOfPlayers + 1);

            for (var i = 0; i < numberOfPlayers; i++)
            {
                // Calculate the x position for the player
                var xPos = spacing * (i + 1) - _worldWidth / 2;

                // Find the closest tile index to the x position
                var tileIndex = Mathf.Clamp(Mathf.RoundToInt((xPos + _worldWidth / 2) / _tileWidth), 0,
                    numberOfTiles - 1);

                // Calculate the y position using the height map
                var yPos = _worldBottom + heightMap[tileIndex] + minBottomDistance;

                Debug.Log("Spawning player " + (i + 1));
                // Instantiate the player at the calculated position
                var tank = Instantiate(playerPrefab, new Vector3(xPos, yPos + 1, 0), Quaternion.identity);
                tank.name = players[i].name;
                var tankScript = tank.GetComponent<Tank>();
                tankScript.SetOwner(players[i]);

                Debug.Log("Assigning tank to player: " + players[i].name);
                players[i].SetTank(tank);
                Debug.Log("Tank assigned to player");
            }

            Debug.Log("SpawnPlayers END");
        }

        private float[] GenerateWorld()
        {
            // Generate the height map
            var heightMap = GenerateHeightMap();
            var worldCenterOffset = numberOfTiles / 2;

            for (var x = 0; x < numberOfTiles; x++)
            {
                var height = heightMap[x] + minBottomDistance;
                var position = new Vector3((_worldWidth - _tileWidth) / 2 - x * _tileWidth, _worldBottom + height / 2,
                    0);
                var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.transform.localScale = new Vector3(_tileWidth, height, 1);
                _generatedTiles[x] = tile;
                tile.transform.name = $"Tile {x}";

                var decorationSize = _tileWidth;
                var randomValue = UnityEngine.Random.value;
                // if (randomValue > 0.8f)
                // {
                //     var prefab = randomValue > 0.9f ? rockPrefab : treePrefab;
                //     var decoration = Instantiate(prefab, position + Vector3.up * (height / 2),
                //         Quaternion.identity);
                //     decoration.transform.localScale = new Vector3(decorationSize, decorationSize, 1);
                //
                //     if (prefab == treePrefab)
                //         _generatedTrees[x] = decoration;
                //     else
                //         _generatedRocks[x] = decoration;
                // }
            }

            return heightMap;
        }

        private float[] GenerateHeightMap()
        {
            var heightMap = new float[numberOfTiles];
            var prng = new Random(perlinSeed);

            // Generate initial height map using Perlin noise
            for (var x = 0; x < numberOfTiles; x++)
            {
                var xCoord = (float)x / numberOfTiles * perlinScale + prng.Next(-10000, 10000);
                heightMap[x] = Mathf.PerlinNoise(xCoord, 0) * perlinAmplitude;
            }

            // Apply multiple passes of smoothing
            for (var pass = 0; pass < 3; pass++)
            {
                var newHeightMap = new float[numberOfTiles];
                for (var i = 0; i < numberOfTiles; i++)
                    newHeightMap[i] = (heightMap[Math.Max(0, i - 2)] + heightMap[Math.Max(0, i - 1)] + heightMap[i] +
                                       heightMap[Math.Min(numberOfTiles - 1, i + 1)] +
                                       heightMap[Math.Min(numberOfTiles - 1, i + 2)]) / 5;
                heightMap = newHeightMap;
            }

            return heightMap;
        }
    }
}