using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;

    public int mapIndex = 0;

    public Transform tilePrefab;
    public Transform wallPrefab;
    public Transform navMeshSurfaceTransform;
    private Transform mapHolder;
    public Queue<Coord> shuffledEmptyTiles;

    Map currentMap;
    Transform[,] tileMap;

    void Start()
    {
        GenerateMap();
        // FindObjectOfType<Spawner>().OnNextWaveStart += GenerateNewLevel;
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.sizeX, currentMap.sizeY];

        System.Random prng = new System.Random(currentMap.seed);

        string holderName = "mapHolder";

        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        List<Coord> coordList = new();
        for (int i = 0; i < currentMap.sizeX; i++)
        {
            for (int j = 0; j < currentMap.sizeY; j++)
            {
                coordList.Add(new Coord(i, j));
            }
        }

        // Place tiles to fill map floor
        for (int i = 0; i < coordList.Count; i++)
        {
            Coord coord = coordList[i];
            Vector3 tilePosition = CoordToPosition(coord);
            Transform tile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(90, 0, 0));
            tile.localScale = new Vector3(currentMap.tileSize, currentMap.tileSize, 1);
            tile.name = "Tile (" + coord.x + ", " + coord.y + ")";
            tile.parent = mapHolder;

            tileMap[coord.x, coord.y] = tile;
        }

        int numberOfWalls = (int)(
            (
                currentMap.sizeX * (currentMap.sizeY - 1)
                + // horizontal walls
                currentMap.sizeY * (currentMap.sizeX - 1)
            ) // vertical walls
            * currentMap.wallDensity
        );

        // Will need this to check if map is fully accessible
        WallOcupation[,] occupiedWalls = new WallOcupation[currentMap.sizeX, currentMap.sizeY];
        for (int i = 0; i < currentMap.sizeX; i++)
        {
            for (int j = 0; j < currentMap.sizeY; j++)
            {
                bool north = j == 0;
                bool east = i == currentMap.sizeX - 1;
                bool south = j == currentMap.sizeY - 1;
                bool west = i == 0;
                occupiedWalls[i, j] = new WallOcupation(i, j, north, east, south, west);
            }
        }

        List<WallLocation> wallLocations = new();
        for (int i = 0; i < currentMap.sizeX; i++)
        {
            for (int j = 0; j < currentMap.sizeY; j++)
            {
                if (i < currentMap.sizeX - 1)
                {
                    wallLocations.Add(new WallLocation(i, j, false));
                }
                if (j < currentMap.sizeY - 1)
                {
                    wallLocations.Add(new WallLocation(i, j, true));
                }
            }
        }
        Assert.AreEqual(
            currentMap.sizeX * (currentMap.sizeY - 1) + currentMap.sizeY * (currentMap.sizeX - 1),
            wallLocations.Count
        );

        Queue<WallLocation> shuffledWallLocations = new Queue<WallLocation>(
            Utility.ShuffleArray(wallLocations.ToArray(), currentMap.seed)
        );

        // Place walls
        int placedWalls = 0;
        while (shuffledWallLocations.Count > 0 && placedWalls < numberOfWalls)
        {
            WallLocation wallPosition = shuffledWallLocations.Dequeue();

            placedWalls++;

            if (wallPosition.isHorizontal)
            {
                occupiedWalls[wallPosition.x, wallPosition.y].south = true;
                if (wallPosition.y < currentMap.sizeY - 1)
                {
                    occupiedWalls[wallPosition.x, wallPosition.y + 1].north = true;
                }
            }
            else
            {
                occupiedWalls[wallPosition.x, wallPosition.y].east = true;
                if (wallPosition.x < currentMap.sizeX - 1)
                {
                    occupiedWalls[wallPosition.x + 1, wallPosition.y].west = true;
                }
            }

            if (MapIsFullyAccesible(occupiedWalls))
            {
                PlaceWallPiece(wallPosition);
            }
            else
            {
                placedWalls--;
                if (wallPosition.isHorizontal)
                {
                    occupiedWalls[wallPosition.x, wallPosition.y].south = true;
                    if (wallPosition.y < currentMap.sizeY - 1)
                    {
                        occupiedWalls[wallPosition.x, wallPosition.y + 1].north = true;
                    }
                }
                else
                {
                    occupiedWalls[wallPosition.x, wallPosition.y].east = true;
                    if (wallPosition.x < currentMap.sizeX - 1)
                    {
                        occupiedWalls[wallPosition.x + 1, wallPosition.y].west = true;
                    }
                }
            }
        }

        // Place peripheral horizontal walls
        for (int i = 0; i < currentMap.sizeX; i++) {
            PlaceWallPiece(new WallLocation(i, -1, true));
            PlaceWallPiece(new WallLocation(i, currentMap.sizeY - 1, true));
        }

        // Place peripheral vertical walls
        for (int i = 0; i < currentMap.sizeY; i++) {
            PlaceWallPiece(new WallLocation(-1, i, false));
            PlaceWallPiece(new WallLocation(currentMap.sizeX - 1, i, false));
        }

        navMeshSurfaceTransform.position = Vector3.zero - 0.01f * Vector3.up;
        navMeshSurfaceTransform.localScale = new Vector3(currentMap.mapSizeX, currentMap.mapSizeY, 1);

    }

    bool MapIsFullyAccesible(WallOcupation[,] occupiedWalls)
    {
        return true;
        // bool[,] checkedTiles = new bool[currentMap.sizeX, currentMap.sizeY];
        // for (int i = 0; i < currentMap.sizeX; i++)
        // {
        //     for (int j = 0; j < currentMap.sizeY; j++)
        //     {
        //         checkedTiles[i, j] = false;
        //     }
        // }
        // // occupiedTiles.CopyTo(checkedTiles, currentMap.sizeX * currentMap.sizeY);
        // Queue<Coord> queue = new Queue<Coord>();
        // int reacheableTiles = 0;

        // queue.Enqueue(currentMap.mapCenter);
        // checkedTiles[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        // reacheableTiles++;

        // Coord[] neighbours =
        // {
        //     new Coord(0, -1),
        //     new Coord(-1, 0),
        //     new Coord(0, 1),
        //     new Coord(1, 0)
        // };
        // while (queue.Count > 0)
        // {
        //     Coord coord = queue.Dequeue();
        //     foreach (Coord n in neighbours)
        //     {
        //         Coord neighbourCoord = new Coord(coord.x + n.x, coord.y + n.y);
        //         // if neighbour in map
        //         if (
        //             neighbourCoord.x >= 0
        //             && neighbourCoord.x < currentMap.sizeX
        //             && neighbourCoord.y >= 0
        //             && neighbourCoord.y < currentMap.sizeY
        //         )
        //         {
        //             // if neighbour not already checked
        //             if (!checkedTiles[neighbourCoord.x, neighbourCoord.y])
        //             {
        //                 // if tile has no obstacle
        //                 if (!occupiedTiles[neighbourCoord.x, neighbourCoord.y])
        //                 {
        //                     queue.Enqueue(neighbourCoord);
        //                     checkedTiles[neighbourCoord.x, neighbourCoord.y] = true;
        //                     reacheableTiles++;
        //                 }
        //             }
        //         }
        //     }
        // }
        // return reacheableTiles == targetTiles;
    }

    public Vector3 WallPositionToMapPosition(WallLocation pos)
    {
        // Center tile coords
        float xCoord =
            pos.x * currentMap.tileSize - (currentMap.mapSizeX - currentMap.tileSize) / 2f;
        float yCoord =
            pos.y * currentMap.tileSize - (currentMap.mapSizeY - currentMap.tileSize) / 2f;
        if (pos.isHorizontal)
        {
            yCoord += currentMap.tileSize / 2f;
        }
        if (!pos.isHorizontal)
        {
            xCoord += currentMap.tileSize / 2f;
        }
        return new Vector3(xCoord, 0, yCoord);
    }

    public void PlaceWallPiece(WallLocation pos)
    {
        Transform wallPiece = Instantiate(
            wallPrefab,
            WallPositionToMapPosition(pos),
            Quaternion.identity
        );
        wallPiece.localScale = GetScaleFromWallPosition(pos);
        wallPiece.name =
            "WallPiece" + "(" + pos.x + ", " + pos.y + ", " + (pos.isHorizontal ? "H" : "V") + ")";
        wallPiece.parent = mapHolder;

        Renderer obstacleRenderer = wallPiece.GetComponent<Renderer>();
        Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
        float colorPercent = pos.y / (float)currentMap.sizeY;
        obstacleMaterial.color = Color.Lerp(
            currentMap.foregroundColor,
            currentMap.backgroundColor,
            colorPercent
        );
        obstacleRenderer.sharedMaterial = obstacleMaterial;
    }

    public Vector3 CoordToPosition(Coord coord)
    {
        float xCoord =
            coord.x * currentMap.tileSize - (currentMap.mapSizeX - currentMap.tileSize) / 2f;
        float yCoord =
            coord.y * currentMap.tileSize - (currentMap.mapSizeY - currentMap.tileSize) / 2f;
        return new Vector3(xCoord, 0, yCoord);
    }

    public Vector3 GetScaleFromWallPosition(WallLocation pos)
    {
        if (pos.isHorizontal)
        {
            return new Vector3(
                currentMap.tileSize,
                currentMap.wallHeight,
                currentMap.wallThicknessPercent
            );
        }
        else
        {
            return new Vector3(
                currentMap.wallThicknessPercent,
                currentMap.wallHeight,
                currentMap.tileSize
            );
        }
    }

    Coord GetRandomCoord(Queue<Coord> coordQueue)
    {
        Coord coord = coordQueue.Dequeue();
        coordQueue.Enqueue(coord);
        return coord;
    }

    public Transform GetRandomTile(Queue<Coord> coordQueue)
    {
        Coord coord = GetRandomCoord(coordQueue);
        return tileMap[coord.x, coord.y];
    }

    public Transform TileFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x + currentMap.mapSizeX / 2f) / currentMap.tileSize);
        int y = Mathf.FloorToInt((position.z + currentMap.mapSizeY / 2f) / currentMap.tileSize);
        if (0 <= x && x < currentMap.sizeX && 0 <= y && y < currentMap.sizeY)
        {
            return tileMap[x, y];
        }
        else
        {
            return null;
        }
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public struct WallLocation
    {
        public int x;
        public int y;
        public bool isHorizontal;

        public WallLocation(int _x, int _y, bool _isHorizontal)
        {
            x = _x;
            y = _y;
            isHorizontal = _isHorizontal;
        }
    }

    public struct WallOcupation
    {
        public int x;
        public int y;
        public bool north;
        public bool east;
        public bool south;
        public bool west;

        public WallOcupation(int _x, int _y, bool _north, bool _east, bool _south, bool _west)
        {
            x = _x;
            y = _y;
            north = _north;
            east = _east;
            south = _south;
            west = _west;
        }
    }

    public void GenerateNewLevel(int i)
    {
        mapIndex = i;
        GenerateMap();
    }

    [System.Serializable]
    public class Map
    {
        [Min(2)]
        public int sizeX = 10;

        [Min(2)]
        public int sizeY = 10;

        [Range(0, 1)]
        public float wallDensity;
        public int seed = 1234;

        public float wallHeight;

        public Color foregroundColor;
        public Color backgroundColor;

        [Min(1)]
        public float tileSize;
        public Coord mapCenter
        {
            get { return new Coord(sizeX / 2, sizeY / 2); }
        }

        [Range(0, 1)]
        public float wallThicknessPercent;

        public float mapSizeX
        {
            get { return tileSize * sizeX; }
        }
        public float mapSizeY
        {
            get { return tileSize * sizeY; }
        }
    }
}
