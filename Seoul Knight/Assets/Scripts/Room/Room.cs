using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class Room : MonoBehaviour
{
    public string type;
    public Vector2 roomCoordinates;

    //  Door Tiles
    public Tile wallRight;
    public Tile wallTopRight;
    public Tile wallLeft;
    public Tile wallTopLeft;
    public Tile wallSideFrontLeft;
    public Tile wallSideFrontRight;

    // Obstacle
    public Tilemap rbTileMap;
    public Tilemap coverTileMap;

    public GameObject columnPrefab;
    public GameObject enemyPrefab;
    public GameObject stairsPrefab;
    public GameObject treasuryChestPrefab;

    public int noOfObstacles;
    public int noOfEnemies;

    private int width = RoomController.instance.width;
    private int height = RoomController.instance.height;


    private void Awake()
    {
        RoomController.instance.RegisterRoom(this);
    }



    void Start()
    {
        if (type == "Combat")
        {
            StartCoroutine(PrepareCombatRoom());
        }
        else if (type == "Exit")
        {
            GameObject stairs = Instantiate(stairsPrefab, this.transform, false) as GameObject;

            stairs.transform.localPosition = new Vector3(0.5f, 1.5f, 0);

<<<<<<< Updated upstream
            GameObject treasuryChest  = Instantiate(treasuryChestPrefab, this.transform, false) as GameObject;

            treasuryChest.transform.localPosition = new Vector3(0.5f, 0.5f, 0);




=======
            GameObject treasuryChest = Instantiate(treasuryChestPrefab, this.transform, false) as GameObject;
            treasuryChest.transform.localPosition = new Vector3(0.5f, 0.5f, 0);
>>>>>>> Stashed changes
        }

        List<Vector2> doorPositions = RoomController.instance.doorLocations[roomCoordinates];
        
        foreach (Vector2 doorPosition in doorPositions)
        {
            Vector2 direction = doorPosition - roomCoordinates;

            if (direction == Vector2.up)
            {
                Vector3Int obstaclesVector = new Vector3Int(-2, height / 2 - 1, 0);
                Vector3Int obstaclesSize = new Vector3Int(5, 2, 1);
                TileBase[] tiles = { wallRight, null, null, null, wallLeft, wallTopRight, null, null, null, wallTopLeft };
                rbTileMap.SetTilesBlock(new BoundsInt(obstaclesVector, obstaclesSize), tiles);

            }
            else if (direction == Vector2.down)
            {
                Vector3Int doorPositionVector = new Vector3Int(-1, -height / 2 + 1, 0);
                Vector3Int doorSize = new Vector3Int(3, 1, 1);
                TileBase[] wallTiles = { null, null, null };

                rbTileMap.SetTilesBlock(new BoundsInt(doorPositionVector, doorSize), wallTiles);

                Vector3Int aboveObstaclesVector = new Vector3Int(-2, -height / 2 + 1, 0);
                Vector3Int aboveObstaclesSize = new Vector3Int(5, 2, 1);
                TileBase[] wallTopTiles = { wallRight, null, null, null, wallLeft, wallTopRight, null, null, null, wallTopLeft };

                coverTileMap.SetTilesBlock(new BoundsInt(aboveObstaclesVector, aboveObstaclesSize), wallTopTiles);

            }
            else if (direction == Vector2.right)
            {
                Vector3Int doorPositionVector = new Vector3Int((width - 1) / 2, -1, 0);
                Vector3Int doorSize = new Vector3Int(1, 4, 1);
                TileBase[] tiles = { null, null, null, wallSideFrontRight };

                rbTileMap.SetTilesBlock(new BoundsInt(doorPositionVector, doorSize), tiles);
                coverTileMap.SetTilesBlock(new BoundsInt(doorPositionVector, doorSize), tiles);

            }
            else if (direction == Vector2.left)
            {
                Vector3Int doorPositionVector = new Vector3Int(-(width - 1) / 2, -1, 0);
                Vector3Int doorSize = new Vector3Int(1, 4, 1);
                TileBase[] tiles = { null, null, null, wallSideFrontLeft };

                rbTileMap.SetTilesBlock(new BoundsInt(doorPositionVector, doorSize), tiles);
                coverTileMap.SetTilesBlock(new BoundsInt(doorPositionVector, doorSize), tiles);

            }
            
        }
    }


    IEnumerator PrepareCombatRoom()
    {
        AstarData data = AstarPath.active.data;
        GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;
        gg.center = new Vector3(roomCoordinates.x * width * 2 + 0.5f, roomCoordinates.y * height * 2 + 0.5f, 0);
        gg.SetDimensions(width - 1, height - 3, 1);
        gg.rotation = new Vector3(-90, 0, 0);
        gg.showMeshSurface = true;
        gg.showMeshOutline = true;
        GraphCollision graphCollision = new GraphCollision();
        graphCollision.use2D = true;
        graphCollision.mask = LayerMask.GetMask("Obstacle");
        gg.collision = graphCollision;

        SpawnObjects(noOfObstacles, columnPrefab);
        

        yield return null;

        AstarPath.active.Scan(gg);
        SpawnObjects(noOfEnemies, enemyPrefab);
    }
    


    private void SpawnObjects(int noOfObjects, GameObject prefab)
    {
        List<Vector2> objectCoordinates = new List<Vector2>(noOfObjects);

        while (objectCoordinates.Count < 5)
        {
            int x = Random.Range(-(width - 3) / 2, (width - 3) / 2);
            int y = Random.Range(-height / 2 + 3, height / 2 - 1);
            Vector2 coordinates = new Vector2(x + 0.5f, y);

            if (!objectCoordinates.Contains(coordinates))
            {
                objectCoordinates.Add(coordinates);
            }
        }

        for (int i = 0; i < noOfObjects; i++)
        {
            GameObject gameObject = Instantiate(prefab, this.transform, false) as GameObject;

            gameObject.transform.localPosition = objectCoordinates[i];
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(-gameObject.transform.position.y * 100);
        }
    }
}