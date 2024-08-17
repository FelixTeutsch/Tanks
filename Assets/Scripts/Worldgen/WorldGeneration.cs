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
            GenerateWorld();
        }

        private void GenerateWorld()
        {
            // Generate the height map
            var heightMap = GenerateHeightMap();
            var worldCenterOffset = numberOfTiles / 2;

            for (var x = 0; x < numberOfTiles; x++)
            {
                var height = heightMap[x] + minBottomDistance;
                var position = new Vector3((_worldWidth - _tileWidth) / 2 - x * _tileWidth, _worldBottom + height / 2,
                    0);
                var tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.transform.localScale = new Vector3(_tileWidth, height, 1);
                _generatedTiles[x] = tile;

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