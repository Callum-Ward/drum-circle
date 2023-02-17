using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmSpawner : MonoBehaviour

{
    public GameObject leftTargetForeground;
    public GameObject leftTargetBase;
    public GameObject rightTargetForeground;
    public GameObject rightTargetBase;
    public GameObject leftBeat;
    public GameObject rightBeat;
    private Vector3 startSpawn;
    private BeatManager beatManager;
    private const float spawnScale = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        //set beat spawner location with respect to camera position
        transform.position = Camera.main.transform.position + new Vector3(0f,2.5f,4f);
        //transform.rotation = Camera.main.transform.rotation;
        //set left most spawn location with respect to spawner postion
        startSpawn = transform.position + new Vector3(-3.5f, 0.35f, 0f);
        
        Instantiate(leftTargetBase, startSpawn + new Vector3(-0.01f, -4.26f, 0.01f), transform.rotation);
        Instantiate(rightTargetBase, startSpawn + new Vector3(1.49f, -4.26f, 0.01f), transform.rotation);

        GameObject newLeftTarget = Instantiate(leftTargetForeground, startSpawn + new Vector3(0f, -4.25f, 0f), transform.rotation);
        GameObject newRightTarget = Instantiate(rightTargetForeground, startSpawn + new Vector3(1.5f, -4.25f, 0f), transform.rotation);
        newLeftTarget.transform.localScale *= spawnScale;
        newRightTarget.transform.localScale *= spawnScale;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void spawn(int pos, int left, int size)
    {
        //pos: 1,2,3,4,5
        //if single player central position will be used to spawn 3
        //if two players then 2 and 4 
        //if three players then 1,3,5 used
        //left: each player has 2 beat lines, if 1 then left beat spawn else right
        Vector3 spawnLoc = startSpawn + new Vector3(pos*1.5f-(left*1.5f), 0f, 0f);
        Vector3 spawnScaleAddition = new Vector3((size - 1)*0.1f, (size-1)*0.1f, 0f);
   
        if (left==1)
        {
            GameObject newBeat = Instantiate(leftBeat, spawnLoc, transform.rotation);
            newBeat.transform.localScale = spawnScale * newBeat.transform.localScale + spawnScaleAddition;
            beatManager.AddToQueueL(newBeat);
        }
        else
        {
            GameObject newBeat = Instantiate(rightBeat, spawnLoc, transform.rotation);
            newBeat.transform.localScale = spawnScale * newBeat.transform.localScale + spawnScaleAddition;
            beatManager.AddToQueueR(newBeat);
        }


    }
}