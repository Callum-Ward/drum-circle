using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmSpawner : MonoBehaviour

{
    public GameObject beatTarget;
    public GameObject leftBeat;
    public GameObject rightBeat;
    private Vector3 startSpawn;
    private BeatManager beatManager;
    //spawn range x= -3.5 to 3.5

    // Start is called before the first frame update
    void Start()
    {
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        //set beat spawner location with respect to camera position
        transform.position = Camera.main.transform.position + new Vector3(0f,2.5f,4f);
        //set left most spawn location with respect to spawner postion
        startSpawn = transform.position + new Vector3(-3.5f, 0f, 0f);
        
        //Instantiate(beatTarget, startSpawn + new Vector3(-0.1f, -4.0f, 0.1f), transform.rotation);

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void spawn(int pos, int left)
    {
        //pos: 1,2,3,4,5
        //if single player central position will be used to spawn 3
        //if two players then 2 and 4 
        //if three players then 1,3,5 used
        //left: each player has 2 beat lines, if 1 then left beat spawn else right
        Vector3 spawnLoc = startSpawn + new Vector3(pos*1.5f-(left*1.5f), 0f, 0f);
   
        if (left==1)
        {
            beatManager.AddToQueue(Instantiate(leftBeat, spawnLoc, transform.rotation));
        }
        else
        {
            beatManager.AddToQueue(Instantiate(rightBeat, spawnLoc, transform.rotation));
        }


    }
}