using System.Collections.Generic;
using Generation.Scriptables;
using UnityEngine;
using Random = System.Random;

namespace Generation.Classes
{
    using UnityEngine.WSA;

    public class MapGenerator
    {

        private Random random;
        private MapConfig config;

        private byte[,] map;
        private int filledTiles;

        private List<Walker> walkers;

        private (int, int)[] directions =
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0)
        };

        public const byte AirId = 0;
        public const byte EarthId = 1;
        public const byte WallId = 2;

        public MapGenerator(MapConfig config)
        {
            this.config = config;
            random = new Random(config.seed);
        }

        public byte[,] Generate()
        {
            walkers = new List<Walker>();
            map = new byte[config.maxMapWidth, config.maxMapHeight];
            (int, int) initWalkerDirection = GetRandomDirection();
            Walker initWalker = new Walker(config.maxMapWidth / 2, config.maxMapHeight / 2, initWalkerDirection.Item1,
                initWalkerDirection.Item2);
            walkers.Add(initWalker);
            int iteration;
            for (iteration = 0; iteration < config.maxIterations; iteration++)
            {
                if (walkers.Count <= 0)
                    break;

                for (int i = 0; i < walkers.Count; i++)
                {
                    Walker currentWalker = walkers[i];
                    // 1 = earth
                    map[currentWalker.position.Item1, currentWalker.position.Item2] = EarthId;
                    filledTiles++;
                    if (random.NextDouble() < config.chanceWalkerDestroy &&
                        (!config.keepLastWalker || walkers.Count > 1))
                    {
                        //walkers.Remove(currentWalker);
                        walkers.RemoveAt(i);

                        /*
                         i--;
                        continue;
                        */
                    }

                    if (random.NextDouble() < config.chanceWalkerChangeDirection)
                    {
                        int previousDirectionIndex = -1;
                        for (int k = 0; k < directions.Length; k++)
                        {
                            if (directions[k] == currentWalker.direction)
                            {
                                previousDirectionIndex = k;
                                break;
                            }
                        }
                        currentWalker.direction = GetRandomDirection(previousDirectionIndex);
                    }

                    if (walkers.Count < config.maxWalkers)
                    {
                        if (random.NextDouble() < config.chanceWalkerSpawn)
                        {
                            (int, int) direction = GetRandomDirection();
                            Walker newWalker = new Walker(currentWalker.position.Item1, currentWalker.position.Item2,
                                direction.Item1, direction.Item2);
                            walkers.Add(newWalker);
                        }
                    }

                    currentWalker.position.Item1 += currentWalker.direction.Item1;
                    currentWalker.position.Item2 += currentWalker.direction.Item2;

                    currentWalker.position.Item1 = IntClamp(currentWalker.position.Item1, 1, config.maxMapWidth - 2);
                    currentWalker.position.Item2 = IntClamp(currentWalker.position.Item2, 1, config.maxMapHeight - 2);
                }

                if ((float) filledTiles / (config.maxMapWidth * config.maxMapHeight) > config.percentToFill)
                    break;
            }

            Debug.Log($"Earth Tiles: {filledTiles}, Iterations: {iteration}");
            DetectWalls();

            return map;
        }

        private void DetectWalls()
        {
            int walls = 0;
            for (int x = 0; x < config.maxMapWidth; x++)
            {
                for (int y = 0; y < config.maxMapHeight; y++)
                {
                    if (map[x, y] == EarthId)
                    {
                        if (map[x, y + 1] == AirId)
                        {
                            map[x, y + 1] = WallId;
                            walls++;
                        }

                        if (map[x, y - 1] == AirId)
                        {
                            map[x, y - 1] = WallId;
                            walls++;
                        }

                        if (map[x + 1, y] == AirId)
                        {
                            map[x + 1, y] = WallId;
                            walls++;
                        }

                        if (map[x - 1, y] == AirId)
                        {
                            map[x - 1, y] = WallId;
                            walls++;
                        }
                    }
                }
            }

            Debug.Log($"Walls: {walls}");
        }


        private int IntClamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        private (int, int) GetRandomDirection(int previous = -1)
        {
            int randomNumber = random.Next(4);
            if (randomNumber == previous)
                randomNumber = (randomNumber + random.Next(3)) % 4;
            return directions[randomNumber];
        }
    }
}