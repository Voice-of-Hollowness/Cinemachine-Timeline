using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;

    [SerializeField]
    public Transform _tilePrefab;

    public Transform obstaclePrefab;

    [SerializeField] 
    private Transform _navMeshObstacle; 

    [SerializeField] 
    private Transform _navMeshFloor;

    [SerializeField] 
    private Transform _navFloorWithColour; 

    [Range(0, 1)] public float outlinePercent;

    [SerializeField] private float _tileSize;

    Queue<Coord> shuffledTileCoords;

    Queue<Coord> shuffledOpenTileCoords;

    private List<Coord> allTileCoords;

    private Map currentMap;

    private Transform[,] tileMap;

    private Color _initColor;
    public Color InitColor => _initColor;

    [SerializeField] 
    private Spawner _spawner; 

    void Awake()
    {
        _spawner.OnNewWave+= OnNewWave;


        //todo:Add auto bake Inspector
    }

    void OnNewWave( int waveNum)
    {
        mapIndex = waveNum - 1;
        GenerateAMap();
    }
 
    public void GenerateAMap()
    {
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * _tileSize, .05f, currentMap.mapSize.y * _tileSize);
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                
                
                allTileCoords.Add(new Coord(x, y));
            }
        }

        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));


        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y =0; y < currentMap.mapSize.y; y++)
            {
                
                
                Vector3 tilePosition = CoordToPosition(x,y);
                Transform newTile = Instantiate(_tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent)*_tileSize;
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        var _renderer = tileMap[0, 0].GetComponent<Renderer>();
        _initColor = _renderer.materials[0].color;

        bool[,] OrbstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];


        int obstacleCount = (int) (currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;

        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);
        
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            OrbstacleMap[randomCoord.x, randomCoord.y] = true;

            currentObstacleCount++;

            var mapIsAccesible = MapAccessible(OrbstacleMap, currentObstacleCount);
            
            if (randomCoord!= currentMap.mapCentre && mapIsAccesible)
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight,(float)(prng.NextDouble()));

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
                newObstacle.localScale =new Vector3((1 - outlinePercent)*_tileSize, obstacleHeight, (1 - outlinePercent) * _tileSize);
                newObstacle.parent = mapHolder;

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.backgroundColor, currentMap.foregroundColor, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                OrbstacleMap[randomCoord.x, randomCoord.y] = false;

                 currentObstacleCount--;
            }
           
        }
        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));


        float f = _tileSize * 0.1f;
        Vector3 scale = new Vector3(currentMap.mapSize.x * f,1f, currentMap.mapSize.y * f);

        if(_navFloorWithColour!=null)
            _navFloorWithColour.localScale = scale;

         _navMeshFloor.localScale =scale;
        AddWalls(mapHolder);
        BakeNavMesh();

    }

    private void BakeNavMesh()
    {
//        NavMeshBuilder.ClearAllNavMeshes();
//        NavMeshBuilder.BuildNavMesh();
    }

    public void AddWalls(Transform mapHolder)
    {
        var h = currentMap.maxObstacleHeight;
        var hp = Vector3.up*h*.5f;
        //Left
        var lWallPos = Vector3.left*((currentMap.mapSize.x*.5f)+.5f)*_tileSize +hp; 
        var leftWall = Instantiate(_navMeshObstacle,lWallPos,Quaternion.identity) ;
        leftWall.localScale = new Vector3(_tileSize, h,currentMap.mapSize.y);
        leftWall.name = "Wall left";

        //Right
        var rWallPos = Vector3.right*((currentMap.mapSize.x*.5f)+.5f)*_tileSize +hp; 
        var rightWall = Instantiate(_navMeshObstacle,rWallPos,Quaternion.identity) ;
        rightWall.localScale = new Vector3(_tileSize, h,currentMap.mapSize.y);
        rightWall.name = "Wall right";

        //Up
        var uWallPos = Vector3.back*((currentMap.mapSize.y*.5f)+.5f)*_tileSize + hp; 
        var upWall = Instantiate(_navMeshObstacle,uWallPos,Quaternion.identity) ;
        upWall.localScale = new Vector3(currentMap.mapSize.x, h,_tileSize);
        upWall.name = "Wall up";

        //Down
        var dWallPos = Vector3.forward*((currentMap.mapSize.y*.5f)+.5f)*_tileSize + hp; 
        var downWall = Instantiate(_navMeshObstacle,dWallPos,Quaternion.identity) ;
        downWall.localScale = new Vector3(currentMap.mapSize.x, h,_tileSize);
        downWall.name = "Wall down";


        var walls = new [] {leftWall, rightWall, upWall, downWall};

        foreach (var wall in walls)
            wall.parent = mapHolder;
        


    }
    private bool MapAccessible(bool[,] ObstacleMap, int obstacleCount)
    {
        var mapFlags = new bool[ObstacleMap.GetLength(0), ObstacleMap.GetLength(1)];
        var queue = new Queue<Coord>();

        queue.Enqueue(currentMap.mapCentre);
        mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        var isXAccessible = neighbourX >= 0 && neighbourX < ObstacleMap.GetLength(0);
                        var isYAccessible = neighbourY >= 0 && neighbourY < ObstacleMap.GetLength(1);
                       
 
                        if (isXAccessible && isYAccessible)
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !ObstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int mapSquare = (int)(currentMap.mapSize.x * currentMap.mapSize.y);

        int targetAccessibleTileCount = mapSquare - obstacleCount;

        var accessible = targetAccessibleTileCount == accessibleTileCount;

        return accessible;
    }
    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) *_tileSize;
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {


        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x,randomCoord.y];
    }

    public Transform getTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt( position.x / _tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / _tileSize + (currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x,0,tileMap.GetLength(0));
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1));
        return tileMap[x,y];
    }


}
 
[Serializable]
public class Map
{
    public Coord mapSize;
    [Range(0, 1)] public float obstaclePercent;
    public int seed;
    public float minObstacleHeight;
    public float maxObstacleHeight;
    public Color foregroundColor;
    public Color backgroundColor;
 
    public Coord mapCentre {
        get {
            return new Coord(mapSize.x/2, mapSize.y/2);
        }
    }
}

[Serializable]
public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1,Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1==c2);
        }

}
