using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OreManager : MonoBehaviour
{
    public GameObject orePrefab = null;
    public int oreRows = 10;
    public int oreCols = 10;
    public float oreSize = 1;
    public float oreSpacing = 1.25f;

    public float currentFloor = 1;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3[,] GenerateGridPositions()
    {
        // Create a 2D array to hold the positions
        Vector3[,] gridPositions = new Vector3[oreRows, oreCols];

        for (int i = 0; i < oreRows; i++)
        {
            for (int j = 0; j < oreCols; j++)
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

        // Instantiate cubes at each position
        for (int i = 0; i < oreCols; i++)
        {
            for (int j = 0; j < oreRows; j++)
            {
                GameObject currentOre = Instantiate(prefab, gridPositions[i, j], Quaternion.identity);
                Ore oreRef = currentOre.GetComponent<Ore>();
                oreRef.Init(currentFloor);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Get the grid positions
        Vector3[,] gridPositions = GenerateGridPositions();

        // Draw the cubes using Gizmos
        for (int i = 0; i < oreRows; i++)
        {
            for (int j = 0; j < oreCols; j++)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawCube(gridPositions[i, j], Vector3.one * oreSize);
            }
        }
    }
}
