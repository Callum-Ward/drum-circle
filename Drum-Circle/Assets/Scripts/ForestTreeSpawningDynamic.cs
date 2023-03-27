using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawnerForest : MonoBehaviour
{

    //setPlayers function needs to be called to setup spawning or script won't spawn any trees
    //cunrrently implementation spawns trees within circle regions depending on player count
    //list containing central tree spawn locations is indexed based on player number
    //trees are stored in 2d list, first index specifies which player tree belongs to


    public GameObject staticTree;
    public GameObject growingTree;
    public float scale;
    public bool GrowingTrees = false;
    public float minGap = 3;
    public int scene = 1;
    private List<GameObject> treeObjs;
    private int[] playerTreeCount = {0,0,0};
    private List<Vector2> treeLocations;
    private float radius;

    int validPlayerCount(int noPlayers)
    {
        if (noPlayers > 3) return 3;
        if (noPlayers < 0) return 1;
        return noPlayers;
    }

    void Start()
    {
        treeLocations = new List<Vector2>();
        treeObjs = new List<GameObject>();
    }

    public void setScene(int sceneNo)
    {
        if (sceneNo > 0 && sceneNo < 4)
        {
            scene = sceneNo;
        }
    }


    private void spawnAtLocation(int playerNo, Vector2 location, bool growing)
    {
        Vector3 treePos = new Vector3(location.x, Terrain.activeTerrain.SampleHeight(new Vector3(location.x, 0, location.y)), location.y);
        GameObject newTree;
        if (growing)
        {
            newTree = Instantiate(growingTreeObject, treePos, transform.rotation);
        }
        else
        {
            newTree = Instantiate(staticTree, treePos, transform.rotation);
            newTree.transform.localScale *= scale;
        }

        trees[playerNo - 1].Add(newTree);
        Debug.Log("spawned tree at " + treePos);
    }
    private bool checkSpawn(Vector3 tree, Vector3 newTree)
    {
        if (Mathf.Pow(tree.x - newTree.x, 2) + Mathf.Pow(tree.z - newTree.z, 2) > (minGap * minGap))
        {
            return true;
        }
        else return false;
    }
    private Vector3 getSpawnLocation(Transform platform) //specifies tree spawning strategy for each scene
    {
        Vector3 treePos = new Vector3();
        bool validLocation = false;
        int attemps = 0;
        while (!validLocation && attemps<10000)
        {
            switch (scene)
            {
            case 1: //forest
                float random = Random.Range(-1.0f, 1.0f);
                Vector2 randomOffset = new Vector2(radius * Mathf.Sin(random), radius * Mathf.Cos(random));
                Vector2 treeLocation = new Vector2(platform.transform.position.x + randomOffset.x, platform.transform.position.z + randomOffset.y);
                treePos = new Vector3(treeLocation.x, Terrain.activeTerrain.SampleHeight(new Vector3(treeLocation.x, 0, treeLocation.y)), treeLocation.y);

                if (treePos.y < 24) validLocation = true; //prevents spawning tress on cliff faces and mountain tops
                break;
            case 2: //mountain
                break;
            case 3: //beach

                break;

            }
            if (validLocation) validLocation = validTreeProximity(treePos); //tree proximity validation applies to all scenes
            
            attemps++;
        }


        return treePos;

    }

    private bool validTreeProximity(Vector3 newTree)
    {
        foreach (GameObject existingTree in treeObjs) //loop through players existing trees
        {
            if (Mathf.Pow(existingTree.transform.position.x - newTree.x, 2) + Mathf.Pow(existingTree.transform.position.z - newTree.z, 2) < (minGap * minGap))
            {
                return false;
            }
        }
        return true;
    }



    //platform transform used to generate spawns
    //playerNo used to assign trees under player numbers
    //treeCount specifies number of trees to spawn
    //treeColour used to instantiate trees with specific player colours
    //scene number used to differentiate between spawning approaches
    public bool spawnTree(Transform platform, int playerNo, int treeCount, Color treeColour) 
    {
        
        playerNo = validPlayerCount(playerNo);
        if (treeCount < 1) treeCount = 1;


        for (int treeNo = 1; treeNo < treeCount; treeNo++)
        {
            int attempts = 0;
            bool invalidLocation = true;

            while (invalidLocation && attempts < 100) //only place trees with a minSeperation distance
            {
                attempts++;
                Vector3 treeLocation = getSpawnLocation(platform);
                   
               
                if (!validTreeProximity(treeLocation))
                {
                    //take randomised vector2 tree position (x,z) and find terrain height at that coordinate to form full (x,y,z) coordinate

                    if (includeGrowingTrees)
                    {
                        spawnTreeAtLocation(playerNo, treeLocation, Random.Range(1, 10) > 5);
                        invalidLocation = false;
                    }
                    else
                    {
                        spawnTreeAtLocation(playerNo, treeLocation, false);
                        invalidLocation = false;
                    }
                }
            }
            if (attempts >= 100) return false;
  
        }
        return true;
    }

     


}
