using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject debugCube;
    public int mazeSize = 10;
    public float wallY = 1.5f;
    private HashSet<Vector3> visitedCells = new HashSet<Vector3>();

    private int[] xOffset = {0, 2, -2, 0};
    private int[] zOffset = {2, 0, 0, -2};

    private void Start()
    {
        generateGrid();
        //removeWallAt(new Vector3(6, 10, 5));
        testRandomUnivisitedNeighbor();
    }

    private void testRandomUnivisitedNeighbor()
    {
        for(int i = 0; i < 10; i++)
        {
            Vector3? randomNeighborLocation = getRandomUnvisitedNeighbor(new Vector3(9, wallY, 9));
            Instantiate(debugCube, (Vector3)randomNeighborLocation, Quaternion.identity, transform);
        }
    }

    private void testUnvisitedNeighbors()
    {
        foreach (Vector3 v in getUnvisitedNeighbors(new Vector3(1, wallY, 1)))
        {
            Instantiate(debugCube, v, Quaternion.identity, transform);
        }

        foreach (Vector3 v in getUnvisitedNeighbors(new Vector3(5, wallY, 9)))
        {
            Instantiate(debugCube, v, Quaternion.identity, transform);
        }

        foreach (Vector3 v in getUnvisitedNeighbors(new Vector3(5, wallY, 5)))
        {
            Instantiate(debugCube, v, Quaternion.identity, transform);
        }
    }

    private void testValidCellLocations()
    {
        Debug.Log("1, y, 1 valid? " + cellIsInMazeArea(new Vector3(1, wallY, 1)));
        Debug.Log("9, y, 9 valid? " + cellIsInMazeArea(new Vector3(9, wallY, 9)));
        Debug.Log("5, y, 5 valid? " + cellIsInMazeArea(new Vector3(5, wallY, 5)));
        Debug.Log("10, y, 10 valid? " + cellIsInMazeArea(new Vector3(10, wallY, 10)));
        Debug.Log("-1, y, -1 valid? " + cellIsInMazeArea(new Vector3(-1, wallY, -1)));
        Debug.Log("0, y, 0 valid? " + cellIsInMazeArea(new Vector3(0, wallY, 0)));
        Debug.Log("1, y, 10 valid? " + cellIsInMazeArea(new Vector3(1, wallY, 10)));
        Debug.Log("10, y, 1 valid? " + cellIsInMazeArea(new Vector3(10, wallY, 1)));
    }

    private void generateGrid()
    {
        for (int j = 0; j <= mazeSize; j += 2)
        {
            for (int i = 0; i <= mazeSize; i++)
            {
                Vector3 horizontalWallLocation = new Vector3(i, wallY, j);
                Instantiate(cellPrefab, horizontalWallLocation, Quaternion.identity, transform);
                if (i % 2 == 1)
                {
                    Vector3 verticalWallLocation = new Vector3(j, wallY, i);
                    Instantiate(cellPrefab, verticalWallLocation, Quaternion.identity, transform);
                }
            }
        }
    }

    private List<Vector3> getUnvisitedNeighbors(Vector3 cell)
    {
        //You are at (5, y, 5)
        //What are our 4 neighbor locations
        //(5, y, 7) (0, 2)
        //(7, y, 5) (2, 0)
        //(3, y, 5) (-2, 0)
        //(5, y, 3) (0, -2)
        // possible x offsets = {0, 2, -2, 0}
        // possible y offsets = {2, 0, 0, -2}

        List<Vector3> unvisitedNeighbors = new List<Vector3>();
        for(int i = 0; i < 4; i++)
        {
            Vector3 possibleNeighbor = new Vector3(cell.x + xOffset[i], cell.y, cell.z + zOffset[i]);
            if(cellIsInMazeArea(possibleNeighbor) && !visitedCells.Contains(possibleNeighbor))
            {
                //Now we have verified that this is a valid unvisited neighbor, we can add it to the list
                unvisitedNeighbors.Add(possibleNeighbor);
            }
        }

        return unvisitedNeighbors;
    }

    private Vector3? getRandomUnvisitedNeighbor(Vector3 cell)
    {
        List<Vector3> unvisitedNeighbors = getUnvisitedNeighbors(cell);

        if(unvisitedNeighbors.Count == 0)
        {
            return null;
        }

        Debug.Log("Picking a random neighbors from 0 to " + unvisitedNeighbors.Count);

        int randomIndex = Random.Range(0, unvisitedNeighbors.Count);

        Debug.Log("We chose " + randomIndex);

        return unvisitedNeighbors[randomIndex];
    }

    private bool cellIsInMazeArea(Vector3 cell)
    {
        bool validX = cell.x > 0 && cell.x < mazeSize;
        bool validZ = cell.z > 0 && cell.z < mazeSize;
        bool validY = cell.y == wallY;
        return validX && validY && validZ;
    }

    private void removeWallAt(Vector3 cell)
    {
        RaycastHit hit;

        if (Physics.Raycast(cell, Vector3.down, out hit))
        {
            print("found an object - distance: " + hit.distance);
            print("found an object - objectname: " + hit.collider.gameObject);
            print("found an object - transform position: " + hit.collider.gameObject.transform.position);
            print("found an object - tag: " + hit.collider.gameObject.tag);
            if (hit.collider.gameObject.tag == "Wall")
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
