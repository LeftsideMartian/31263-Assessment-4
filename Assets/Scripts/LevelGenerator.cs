using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Level map from assignment
    private int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    // Store a list version of the 2D array for horizontal and vertical mirroring purposes
    private List<List<int>> levelMapList;

    public bool willGenerateLevel;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite[] tileSprites;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private GameObject powerPelletPrefab;
    [SerializeField] private GameObject pacstudentPrefab;
    private Vector2 pacstudentSpawnPoint = new Vector2(1, 1);
    [SerializeField] private GameObject[] ghostPrefabs;
    private Vector2[] ghostSpawnPointArray = new []{ new Vector2(11, 13), new Vector2(16, 13), new Vector2(11, 15), new Vector2(16, 15) };

    // The basePoint is representative of the current top left corner of the grid
    private Vector3 basePoint = new Vector3(0.0f, 0.0f, 0.0f);
    private GameObject generatedLevel;
    private Transform parentTransform;
    int ghostIndex;
    
    private int firstRow;
    private int lastRow;
    private int firstCol;
    private int lastCol;
    
    // Start is called before the first frame update
    void Start()
    {
        if (willGenerateLevel)
        {
            // Delete existing level grid
            Destroy(GameObject.FindGameObjectWithTag("LevelGrid"));

            // Horizontally and vertically flip the map, storing the entire level layout in a single list
            ConvertLevelMapFromArrayToList();
            HorizontallyFlipLevelMap();
            VerticallyFlipLevelMap();

            // Build the whole level from the list
            BuildLevel();

            PlaceCharacters();
        }
    }

    private void ConvertLevelMapFromArrayToList()
    {
        levelMapList = new List<List<int>>();
        
        for (int j = 0; j < levelMap.GetLength(0); j++)
        {
            List<int> row = new List<int>();

            for (int i = 0; i < levelMap.GetLength(1); i++)
            {
                row.Add(levelMap[j, i]);
            }
            
            levelMapList.Add(row);
        }
    }

    private void HorizontallyFlipLevelMap()
    {
        for (int j = 0; j < levelMap.GetLength(0); j++)
        {
            // i represents the column of the grid
            for (int i = 0; i < levelMap.GetLength(1); i++)
            {
                levelMapList[j].Add(levelMap[j, levelMap.GetLength(1) - i - 1]);
            }
        }
    }

    private void VerticallyFlipLevelMap()
    {
        for (int j = levelMap.GetLength(0) - 2; j >= 0; j--)
        {
            levelMapList.Add(levelMapList[j]);
        }
    }
    
    private void BuildLevel()
    {
        firstRow = 0;
        lastRow = levelMapList.Count - 1;
        firstCol = 0;
        lastCol = levelMapList[0].Count - 1;

        // Instantiate an empty game object called GeneratedLevel
        generatedLevel = new GameObject();
        generatedLevel.name = "GeneratedLevel";

        parentTransform = generatedLevel.transform;
        parentTransform.transform.position = basePoint;
        // Insert tiles as children of this game object

        // j represents the row of the grid
        for (int j = 0; j < levelMapList.Count; j++)
        {
            // i represents the column of the grid
            for (int i = 0; i < levelMapList[j].Count; i++)
            {
                CreateNewTile(i, j);
            }
        }
        
        PlaceCharacters();
    }

    private void CreateNewTile(int i, int j)
    {
        // Create new tile
        GameObject tile = Instantiate(tilePrefab, parentTransform, true);
        
        // Set position on grid
        tile.transform.localPosition = new Vector3(i, -j, 0);
        
        // Set sprite of tile
        int spriteType = levelMapList[j][i];
        tile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteType];

        bool isPacStudentSpawnPoint = i == 1 && j == 1;
        
        if (spriteType == 5 && !isPacStudentSpawnPoint)
        {
            GameObject pellet = Instantiate(pelletPrefab, parentTransform, true);
            pellet.transform.localPosition = new Vector3(i, -j, -0.2f);
        } else if (spriteType == 6)
        {
            GameObject powerPellet = Instantiate(powerPelletPrefab, parentTransform, true);
            powerPellet.transform.localPosition = new Vector3(i, -j, -0.2f);
        }

        // Handle different rotation cases (excluding 0, 5 and 6 as they are always the same rotation) 
        switch (spriteType)
        {
            case 1:
                HandleOuterCornerRotation(i, j, tile);
                break;
            case 2:
                HandleOuterWallRotation(i, j, tile);
                break;
            case 3:
                HandleInnerCornerRotation(i, j, tile);
                break;
            case 4:
                HandleInnerWallRotation(i, j, tile);
                break;
            case 7:
                HandleTJunctionRotation(i, j, tile);
                break;
            case 8:
                HandleGhostGateRotation(i, j, tile);
                break;
        }
    }

    private void HandleOuterCornerRotation(int i, int j, GameObject tile)
    {
        int[] validSprites = { 1, 2, 7 };

        bool validTileUp = j > firstRow && IsValidSprite(levelMapList[j - 1][i], validSprites);
        bool validTileDown = j < lastRow && IsValidSprite(levelMapList[j + 1][i], validSprites);
        bool validTileLeft = i > firstCol && IsValidSprite(levelMapList[j][i - 1], validSprites);
        bool validTileRight = i < lastCol && IsValidSprite(levelMapList[j][i + 1], validSprites);

        if (validTileRight && validTileUp)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
        } else if (validTileLeft && validTileDown)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, -90);
        } else if (validTileLeft && validTileUp)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    private void HandleOuterWallRotation(int i, int j, GameObject tile)
    {
        int[] validSprites = { 1, 2, 7 };
        bool validTileUp = j > firstRow && IsValidSprite(levelMapList[j - 1][i], validSprites);
        bool validTileDown = j < lastRow && IsValidSprite(levelMapList[j + 1][i], validSprites);

        if (validTileUp || validTileDown)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    private void HandleInnerCornerRotation(int i, int j, GameObject tile)
    {
        int[] validSprites = { 3, 4, 7, 8 };
        
        bool validTileUp = j > firstRow && IsValidSprite(levelMapList[j - 1][i], validSprites);
        bool validTileDown = j < lastRow && IsValidSprite(levelMapList[j + 1][i], validSprites);
        bool validTileLeft = i > firstCol && IsValidSprite(levelMapList[j][i - 1], validSprites);
        bool validTileRight = i < lastCol && IsValidSprite(levelMapList[j][i + 1], validSprites);

        // Handle case where inner corner is surrounded
        if (validTileUp && validTileDown && validTileLeft && validTileRight)
        {
            bool upLeftEmpty  = IsValidSprite(levelMapList[j - 1][i - 1], new []{ 0, 5, 6 });
            bool upRightEmpty = IsValidSprite(levelMapList[j - 1][i + 1], new []{ 0, 5, 6 });
            bool downLeftEmpty  = IsValidSprite(levelMapList[j + 1][i - 1], new []{ 0, 5, 6 });

            if (upLeftEmpty)
            {
                tile.transform.rotation = Quaternion.Euler(0, 0, 180);
            } else if (upRightEmpty)
            {
                tile.transform.rotation = Quaternion.Euler(0, 0, 90);
            } else if (downLeftEmpty)
            {
                tile.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
        } else if (validTileRight && validTileUp)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
        } else if (validTileLeft && validTileDown)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, -90);
        } else if (validTileLeft && validTileUp)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 180);
        } else if ((validTileUp || validTileDown) && (i == firstCol || i == lastCol))
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    private void HandleInnerWallRotation(int i, int j, GameObject tile)
    {
        int[] validSprites = { 3, 4, 7, 8 };
        bool validTileUp = j > firstRow && IsValidSprite(levelMapList[j - 1][i], validSprites);
        bool validTileDown = j < lastRow && IsValidSprite(levelMapList[j + 1][i], validSprites);

        if (validTileUp && validTileDown)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
        } 
    }

    private void HandleTJunctionRotation(int i, int j, GameObject tile)
    {
        int[] validWalls = { 2, 7 };

        bool validWallUp = j > firstRow && IsValidSprite(levelMapList[j - 1][i], validWalls);
        bool validWallDown = j < lastRow && IsValidSprite(levelMapList[j + 1][i], validWalls);
        bool validWallLeft = i > firstCol && IsValidSprite(levelMapList[j][i - 1], validWalls);
        bool validWallRight = i < lastCol && IsValidSprite(levelMapList[j][i + 1], validWalls);
        
        bool validThinWallUp = j > firstRow && IsValidSprite(levelMapList[j - 1][i], new []{ 4 });
        bool validThinWallLeft = i > firstCol && IsValidSprite(levelMapList[j][i - 1], new []{ 4 });
        bool validThinWallRight = i < lastCol && IsValidSprite(levelMapList[j][i + 1], new []{ 4 });

        if (validWallUp && validWallDown && validThinWallLeft)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, -90);
        } else if (validWallUp && validWallDown && validThinWallRight)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
        } else if (validThinWallUp && validWallLeft && validWallRight)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    private void HandleGhostGateRotation(int i, int j, GameObject tile)
    {
        int[] validSprites = { 3, 4 };
        bool validTileUp = j > firstRow && IsValidSprite(levelMapList[j - 1][i], validSprites);
        bool validTileDown = j < lastRow && IsValidSprite(levelMapList[j + 1][i], validSprites);

        if (validTileUp && validTileDown)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
        } 
    }

    // Helper function to check if a sprite is in the array of valid sprites
    private bool IsValidSprite(int sprite, int[] validSprites)
    {
        return Array.Exists(validSprites, validSprite => validSprite == sprite);
    }
    
    private void PlaceCharacters()
    {
        GameObject pacstudent = Instantiate(pacstudentPrefab, parentTransform, true);
        pacstudent.transform.position = new Vector3(pacstudentSpawnPoint.x, -pacstudentSpawnPoint.y, -0.2f);

        for (int i = 0; i <= ghostPrefabs.Length; i++)
        {
            GameObject ghost = Instantiate(ghostPrefabs[i], parentTransform, true);
            Vector2 spawnPoint = ghostSpawnPointArray[i];
            ghost.transform.position = new Vector3(spawnPoint.x, -spawnPoint.y, -0.2f);
        }
    }
}
