using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawnerForest : MonoBehaviour
{

    //setPlayers function needs to be called to setup spawning or script won't spawn any trees
    //cunrrently implementation spawns trees within circle regions depending on player count
    //list containing central tree spawn locations is indexed based on player number
    //trees are stored in 2d list, first index specifies which player tree belongs to

    public GameObject treeObject;
    public float minSeperation =1;
    private List<List<GameObject>> trees;
    private int playerCount =0;
    private List<Vector2> spawnLocations;
    private float spawnRadius;


    // Start is called before the first frame update
    void Start()
    {
        spawnLocations = new List<Vector2>();
        trees = new List<List<GameObject>>();
    }

    int validatePlayerCount(int noPlayers)
    {
        if (noPlayers > 3) return 3;
        if (noPlayers < 0) return 1;
        return noPlayers;
    }

    void setPlayers(int noPlayers)
    {
        //coordinates vector2 (x,z) set then y will be pulled from terrain height at that point
        playerCount = validatePlayerCount(noPlayers);

        if (playerCount == 3)
        {
            spawnRadius = 76f;
            spawnLocations.Add(new Vector2(132, 243));
            spawnLocations.Add(new Vector2(286, 243));
            spawnLocations.Add(new Vector2(378, 120));

        }
        else if (playerCount == 2)
        {
            spawnRadius = 105f;
            spawnLocations.Add(new Vector2(203, 242));
            spawnLocations.Add(new Vector2(340, 224));
        }
        else
        {
            spawnRadius = 160f;
            spawnLocations.Add(new Vector2(287, 224));
        }
    }

    bool validSpawn(GameObject tree, Vector2 newTree)
    {
        if (Mathf.Sqrt(Mathf.Pow(tree.transform.position.x - newTree.x, 2) + Mathf.Pow(tree.transform.position.z - newTree.x, 2)) > minSeperation)
        {
            return true;
        }
        else return false;
    }


    void spawnTree(int playerNo, int treeCount)
    {
        if (playerCount > 0)
        {
            
            playerNo = validatePlayerCount(playerNo);
            if (treeCount < 1) treeCount = 1;
            if (playerNo > playerCount) playerNo = playerCount;
        
            for (int treeNo = 1; treeNo < treeCount; treeCount++)
            {
                bool invalidLocation = true;
                while (invalidLocation) //only place trees with a minSeperation distance
                {
                    float random = Random.Range(-1, 1);
                    Vector2 randomOffset = new Vector2(spawnRadius * Mathf.Sin(random), spawnRadius * Mathf.Cos(random));
                    Vector2 treeLocation = spawnLocations[playerCount - 1] + randomOffset;
                    bool treeTooClose = false;
                    foreach (GameObject tree in trees[playerNo-1]) //loop through players existing trees
                    {
                        if (!validSpawn(tree, treeLocation)) treeTooClose = true;
                    }
                    if (!treeTooClose)
                    {
                        //take randomised vector2 tree position (x,z) and find terrain height at that coordinate to form full (x,y,z) coordinate
                        Vector3 treePos = new Vector3(treeLocation.x, Terrain.activeTerrain.SampleHeight(new Vector3(treeLocation.x, 0, treeLocation.y)), treeLocation.y);
                        trees[playerNo-1].Add(Instantiate(treeObject, treePos, transform.rotation));
                        invalidLocation = false;
                    }
                }

            }
        }   
    }


}
