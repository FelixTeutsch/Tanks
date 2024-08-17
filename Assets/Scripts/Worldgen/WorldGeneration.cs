using System;
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

        [SerializeField] private int playerCount = 2;


        private GameObject[] _generatedRocks;
        private GameObject[] _generatedTiles;
        private GameObject[] _generatedTrees;

        private float _tileWidth;
        private float _worldBottom;
        private float _worldHeight;

        private float _worldWidth;

        private void Start()
        {
            // randomize the seed
            perlinSeed = new Random().Next(0, 10000);

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

            // Spawn the players
            SpawnPlayers(heightMap);
        }

        private void SpawnPlayers(float[] heightMap)
        {
            // Calculate the spacing between players
            var spacing = _worldWidth / (playerCount + 1);

            for (var i = 0; i < playerCount; i++)
            {
                // Calculate the x position for the player
                var xPos = spacing * (i + 1) - _worldWidth / 2;

                // Find the closest tile index to the x position
                var tileIndex = Mathf.Clamp(Mathf.RoundToInt((xPos + _worldWidth / 2) / _tileWidth), 0,
                    numberOfTiles - 1);

                // Calculate the y position using the height map
                var yPos = _worldBottom + heightMap[tileIndex] + minBottomDistance;

                // Instantiate the player at the calculated position
                var player = Instantiate(playerPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
                player.name = $"Player {i + 1}";
            }
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