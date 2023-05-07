using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawning : MonoBehaviour
{

    //setPlayers function needs to be called to setup spawning or script won't spawn any trees
    //cunrrently implementation spawns trees within circle regions depending on player count
    //list containing central tree spawn locations is indexed based on player number
    //trees are stored in 2d list, first index specifies which player tree belongs to


    public GameObject staticTree;
    public GameObject growingTree;
    [SerializeField] private Waypoints treeSpawns; //refernece to waypoints list
    public Transform platform;
    public camera_front cameraFront;
    public float scale;
    public Vector3 closestSpawn;
    public bool GrowingTrees = false;
    public float minGap = 1.5f;
    private float waterLevel = 106;
    public int scene = 1;
    private List<GameObject> treeObjs;
    private int[] playerTreeCount = {0,0,0};
    private List<Vector3> beachIslandSpawns;
    private float radius;


    //last tree to spawn
    private Transform currentTree;
    private int lastTreeIndex; 


    int validPlayerNo(int noPlayers)
    {
        if (noPlayers > 3) return 3;
        if (noPlayers < 1) return 1;
        return noPlayers;
    }

    void Start()
    {
       
        treeObjs = new List<GameObject>();
        if (scene == 3)
        beachIslandSpawns = new List<Vector3>();
        beachIslandSpawns.Add(new Vector3(382, 50,307));  //spawn points encoded (x,radius,z)

        beachIslandSpawns.Add(new Vector3(349,12, 432 ));  

        beachIslandSpawns.Add(new Vector3(255, 43,224));  

        beachIslandSpawns.Add(new Vector3(436,80, 120));  

        beachIslandSpawns.Add(new Vector3(639,43, 127));  

        beachIslandSpawns.Add(new Vector3(544,29,250));  

        beachIslandSpawns.Add(new Vector3(217,70, 58));  
        beachIslandSpawns.Add(new Vector3(117, 90, 86));  
        beachIslandSpawns.Add(new Vector3(75,90, 250));  

        beachIslandSpawns.Add(new Vector3(147, 103,547));  

        beachIslandSpawns.Add(new Vector3(350,101,609));  
        beachIslandSpawns.Add(new Vector3(595,101,609));  
        beachIslandSpawns.Add(new Vector3(650,101,503));  
        beachIslandSpawns.Add(new Vector3(650,110,385));

        closestSpawn = new Vector3(0, 0, 0);
        if (scene == 3)
        {
            getSpawnLocation(); //update the closest spawn so when game starts cameras face closest island spawn
            cameraFront.centre = new Vector3(closestSpawn.x, waterLevel, closestSpawn.z);
        }

    }

    public Vector3 getClosestSpawn()
    {
        return closestSpawn;
    }

    public void setScene(int sceneNo)
    {
        if (sceneNo > 0 && sceneNo < 4)
        {
            scene = sceneNo;
        }
    }
    public int getPlayerTreeCount(int playerNo)
    {
        return playerTreeCount[playerNo - 1];
    }

    public Vector3 getLastTreeLocation()
    {
        return currentTree.position;
    }

    public void spawnAtLocation(int playerNo, Vector3 location, bool growing)
    {
        GameObject newTree;
        if (growing)
        {
            newTree = Instantiate(growingTree, location, transform.rotation);
        }
        else
        {
            newTree = Instantiate(staticTree, location, transform.rotation);
        }
        playerTreeCount[playerNo - 1] += 1;
        treeObjs.Add(newTree);
        currentTree = newTree.transform;
        Debug.Log("spawned tree at " + location);
    }
    private float distanceSqrdFrom(Vector3 start, Vector3 end)
    {
        return Mathf.Pow(start.x - end.x, 2) + Mathf.Pow(start.z - end.z, 2); //distances are kept squared to reduce computation
    }
     
    private bool validTreeProximity(Vector3 newTree)
    {
        foreach (GameObject existingTree in treeObjs) //loop through players existing trees
        {
            if (distanceSqrdFrom(existingTree.transform.position,newTree)  < (minGap * minGap))
            {
                return false;
            }
        }
        return true;
    }
    private Vector3 getRandomSpawn(Vector3 spawnCentre, float radius, float minDisFromCentre) //finds random locaition within radius from spawn centre 
    {
        if (minDisFromCentre < 0 || minDisFromCentre > radius) minDisFromCentre = 0;
        float randCircum = Random.Range(-1.0f, 1.0f); //specify point on circumference of circle
        float randDis = Random.Range(minDisFromCentre, radius); //scale point on circumference within specified range
        Vector2 pointInCircle = new Vector2(spawnCentre.x + randDis * Mathf.Sin(randCircum), spawnCentre.z + randDis * Mathf.Cos(randCircum)); //random point within specified range from spawn centre
        return new Vector3(pointInCircle.x, Terrain.activeTerrain.SampleHeight(new Vector3(pointInCircle.x, 0, pointInCircle.y)) + Terrain.activeTerrain.transform.position.y, pointInCircle.y);
        //return new Vector3(pointInCircle.x, 0, pointInCircle.y);
    }
    public Vector3 getSpawnLocation() //specifies tree spawning strategy for each scene
    {
        Vector3 treePos = new Vector3();
        bool validLocation = false;
        int attemps = 0;
        int attempLim = 10000;
        Vector3 oldClosestSpawn = closestSpawn;
        while (!validLocation && attemps<attempLim)
        {
            switch (scene)
            {
                case 1: //forest
                    float forestSpawnRadius = 30f;
                    treePos = getRandomSpawn(platform.transform.position, forestSpawnRadius, 4); //get spawn locaiton within spawn radius of platform min distance from platform set to 4
                    if (treePos.y < 24) validLocation = true; //prevents spawning tress on cliff faces and mountain tops
                    break;
                case 3://beach spawning will choose the cloest island to the platform to spawn trees

                    float minDistance = 20000; //squared distance to spawn location 
                    
                    foreach (Vector3 spawnLoc in beachIslandSpawns)
                    {
                        float distance = distanceSqrdFrom(platform.position, spawnLoc);
                        if (minDistance > distance)
                        {
                            minDistance = distance;
                            closestSpawn = spawnLoc;
                        }
                    }
                    treePos = getRandomSpawn(closestSpawn, closestSpawn.y,0);
                    //Debug.Log(treePos);
                    if (treePos.y > waterLevel) validLocation = true; //prevent trees spawning under water
                    break;
            }
            if (validLocation) validLocation = validTreeProximity(treePos); //tree proximity validation applies to all scenes
            
            attemps++;
        }
        if (attemps == attempLim)
        {
            Debug.Log("Unable to find valid spawn");
            //Debug.Log(Terrain.activeTerrain.SampleHeight(new Vector3(110f, 0f, 109f)));

            return new Vector3(0, 0, 0);
        }
        if (scene == 3 && oldClosestSpawn != closestSpawn)
        {
            cameraFront.centre = new Vector3(closestSpawn.x, waterLevel, closestSpawn.z);
            Debug.Log("Camera center updated");

        }


        return treePos;

    }

    //platform transform used to generate spawns
    //playerNo used to assign trees under player numbers
    //treeCount specifies number of trees to spawn
    //treeColour used to instantiate trees with specific player colours
    //scene number used to differentiate between spawning approaches
    public bool spawnTree(int playerNo, int treeCount, Color treeColour, bool growing) 
    {
        //Debug.Log("pre spawn");
        playerNo = validPlayerNo(playerNo);
        if (treeCount < 1) treeCount = 1;
        Vector3 treeLocation;
        if (scene == 1 || scene ==3)
        {
            for (int treeNo = 1; treeNo < treeCount; treeNo++)
            {
                treeLocation = getSpawnLocation();
                if (treeLocation.y != 0) spawnAtLocation(playerNo, treeLocation, growing);//if height is set to 0 function was unable to find valid tree spawn location
            }
            return true;
        } else //mountain map uses preset spawning strategy
        {
            treeLocation = treeSpawns.GetNextWaypoint(currentTree).position;
            treeLocation = new Vector3(treeLocation.x, Terrain.activeTerrain.SampleHeight(new Vector3(treeLocation.x, 0, treeLocation.y)) + Terrain.activeTerrain.transform.position.y, treeLocation.z);
            spawnAtLocation(playerNo, treeLocation, growing);
            return true;
        }

    }

     


}
