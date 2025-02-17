using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        if (orePrefab == null)
        {
            Debug.LogError($"Missing ore prefab, please add it to the {gameObject.name}");
        }
        else
        {
            InstantiateGrid(orePrefab);
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
    public void InstantiateGrid(GameObject prefab)
    {
        // Get the grid positions
        Vector3[,] gridPositions = GenerateGridPositions();

        Ore[,] oreGrid = new Ore[oreCols, oreRows];

        // Instantiate cubes at each position
        for (int i = 0; i < oreCols; i++)
        {
            for (int j = 0; j < oreRows; j++)
            {
                GameObject currentOre = Instantiate(prefab, gridPositions[i, j], Quaternion.identity);
                currentOre.name = $"Ore {i}x{j}";
                Ore oreRef = currentOre.GetComponent<Ore>();
                oreRef.Init(currentFloor, oreSize, currentFloor);
                remainingOre.Add(oreRef);

                oreGrid[i,j] = oreRef;
            }
        }

        // Assign neighbors
        //for (int i = 0; i < oreRows; i++)
        //{
        //    for (int j = 0; j < oreCols; j++)
        //    {
        //        Ore currentOre = oreGrid[i, j];
        //        currentOre.Neighbours = GetNeighbors(oreGrid, i, j);
        //    }
        //}
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
