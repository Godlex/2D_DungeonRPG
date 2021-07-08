using System.Collections.Generic;
using UnityEngine;

namespace Generation.Scriptables
{
    [CreateAssetMenu(fileName = "Map Config", menuName = "Dungeon/Map Config")]
    public class MapConfig : ScriptableObject
    {
        public int seed;
        public int maxIterations = 1000;
        [Header("Map Settings")] public GameObject tileEarthPrefab;
        public GameObject tileWallPrefab;
        public int maxMapWidth = 500;
        public int maxMapHeight = 500;

        [Header("Walkers Settings")] public int maxWalkers = 3;
        public bool keepLastWalker = true;
        [Range(0, 1)] public float chanceWalkerChangeDirection = 0.5f;
        [Range(0, 1)] public float chanceWalkerSpawn = 0.01f;
        [Range(0, 1)] public float chanceWalkerDestroy = 0.01f;
        [Range(0, 1)] public float percentToFill = 0.025f;

        [Header("Tiles Settings")] public List<Color> colorsByIds;
    }
}