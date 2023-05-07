using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [HideInInspector] public TreeSpawning treeSpawner;

    public GameObject explosionVFX;

    public float speed = 120f;
    public float rotationSpeed = 90f;

    private Vector3 heading;
    private bool spawn;
    private int playerIndex;

    public void StartUp(int playerIndex, bool spawn)
    {
        this.spawn = spawn;
        this.playerIndex = playerIndex;
        Debug.Log("FST " + spawn);
        if(spawn)
        {
            this.heading = treeSpawner.getSpawnLocation();
            treeSpawner.spawnAtLocation(playerIndex+1, heading, true);
        }
        else
        {
            Debug.Log("getLatTreeLocation: " + this.heading);

            this.heading = treeSpawner.getLastTreeLocation();
        }
    }

    void Awake()
    {
        treeSpawner = GameObject.Find("TreeSpawner").GetComponent<TreeSpawning>();
    }

    void Update()
    {
        if(heading != null)
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.velocity = transform.forward * speed;
            var rotation = Quaternion.LookRotation(heading - transform.position);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colliding at " + transform.position + " -> " + heading);
        if(spawn)
        {
            treeSpawner.spawnAtLocation(playerIndex+1, heading, true);
        }
        Destroy(gameObject);
    }
}
