using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OreSprite
{
    public string oreName; // Name of the ore
    public Sprite sprite;  // Corresponding sprite
}

public class OreManager : MonoBehaviour
{
    #region Singleton Creation
    // Singleton instance
    private static OreManager instance;

    public static OreManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Look for an existing instance in the scene
                instance = FindObjectOfType<OreManager>();

                if (instance == null)
                {
                    // Create a new GameObject with OreManager if none exists
                    GameObject singletonObject = new GameObject("OreManager");
                    instance = singletonObject.AddComponent<OreManager>();
                }
            }
            return instance;
        }
    }

    // Prevent direct instantiation
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"Multiple instances of {nameof(OreManager)} found. Destroying the duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Makes the instance persist across scenes

        // Convert List to Dictionary for fast lookups
        oreTextureDict = new Dictionary<string, Sprite>();
        foreach (var oreSprite in oreSprites)
        {
            if (!oreTextureDict.ContainsKey(oreSprite.oreName))
            {
                oreTextureDict.Add(oreSprite.oreName, oreSprite.sprite);
            }
        }
    }
    #endregion

    public GameObject orePrefab = null;
    public int oreRows = 10;
    public int oreCols = 10;
    public float oreSize = 1;
    public float oreSpacing = 1.25f;

    public float currentFloor = 1;

    private List<Ore> remainingOre = new List<Ore>();
    public List<Ore> RemainingOre { get { return remainingOre; } }

    public List<OreSprite> oreSprites;

    private Dictionary<string, Sprite> oreTextureDict;

    // Start is called before the first frame update
    void Start()
    {
        if (orePrefab == null)
        {
            Debug.LogError($"Missing ore prefab, please add it to the {gameObject.name}");
        }
        else
        {
            InstantiateGrid();
        }
    }

    private Vector3[,] GenerateGridPositions()
    {
        // Create a 2D array to hold the positions
        Vector3[,] gridPositions = new Vector3[oreCols, oreRows];

        for (int i = 0; i < oreCols; i++)
        {
            for (int j = 0; j < oreRows; j++)
            {
                // Calculate the position for each cube
                Vector3 position = new Vector3(
                    transform.position.x + i * oreSpacing,
                    transform.position.y + j * oreSpacing,
                    0
                );

                // Store the position in the array
                gridPositions[i, j] = position;
            }
        }

        return gridPositions;
    }

    public void InstantiateGrid()
    {
        // Get the grid positions
        Vector3[,] gridPositions = GenerateGridPositions();

        Ore[,] oreGrid = new Ore[oreCols, oreRows];

        // Instantiate cubes at each position
        for (int i = 0; i < oreCols; i++)
        {
            for (int j = 0; j < oreRows; j++)
            {
                GameObject currentOre = Instantiate(orePrefab, gridPositions[i, j], Quaternion.identity);
                currentOre.name = $"Ore {i}x{j}";
                Ore oreRef = currentOre.GetComponent<Ore>();
                oreRef.Init(currentFloor, oreSize, currentFloor);
                remainingOre.Add(oreRef);

                oreGrid[i,j] = oreRef;
            }
        }

        for (int i = 0; i < oreCols; i++)
        {
            for (int j = 0; j < oreRows; j++)
            {
                Ore currentOre = oreGrid[i, j];
                currentOre.Neighbours = GetNeighbors(oreGrid, i, j);
            }
        }

        foreach (var currOre in remainingOre)
        {
            if (currOre != null)
            {
                currOre.UpdateTexture();
            }
        }
    }

    private Ore[] GetNeighbors(Ore[,] grid, int col, int row)
    {
        Ore[] neighbours = new Ore[4];

        if (col < oreCols -1) neighbours[0] = grid[col + 1, row];   // Right
        if (row > 0) neighbours[1] = grid[col, row - 1];            // Bottom
        if (col > 0) neighbours[2] = grid[col - 1, row];            // Left
        if (row < oreRows -1) neighbours[3] = grid[col, row + 1];   // Top

        return neighbours;
    }

    public string DeterminOrePosition(Ore ore)
    {
        var positionMap = new Dictionary<string, string>
        {
            { "0111", "TopLeft" }, // Top row with nothing below it
            { "0101", "TopMiddle" },
            { "1101", "TopRight" },
            { "0011", "TopLeft" }, // Top row with something below it
            { "0001", "TopMiddle" },
            { "1001", "TopRight" },
            { "0010", "MiddleLeft" },
            { "0000", "MiddleMiddle" },
            { "1000", "MiddleRight" },
            { "0110", "BottomLeft" },
            { "0100", "BottomMiddle" },
            { "1100", "BottomRight" },
            { "1011", "TopMiddle" } // Alone in a row with nothing below
        };


        // Generate a key based on neighbour conditions
        string key = $"{BoolToBit(ore.Neighbours[0] == null)}" + // Right
                     $"{BoolToBit(ore.Neighbours[1] == null)}" + // Bottom
                     $"{BoolToBit(ore.Neighbours[2] == null)}" + // Left
                     $"{BoolToBit(ore.Neighbours[3] == null)}";  // Top

        return positionMap.TryGetValue(key, out string position) ? position : "MiddleMiddle";
    }

    // Helper method to convert bool to bit (0 or 1)
    private int BoolToBit(bool value) => value ? 1 : 0;

    public Sprite GetTexture(string name)
    {
        oreTextureDict.TryGetValue(name, out Sprite texture);
        return texture;
    }

    private void OnDrawGizmos()
    {
        if (oreCols < 0 || oreRows < 0) return;
        // Get the grid positions
        Vector3[,] gridPositions = GenerateGridPositions();

        // Draw the cubes using Gizmos
        for (int i = 0; i < oreCols; i++)
        {
            for (int j = 0; j < oreRows; j++)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawCube(gridPositions[i, j], Vector3.one * oreSize);
            }
        }
    }
}
