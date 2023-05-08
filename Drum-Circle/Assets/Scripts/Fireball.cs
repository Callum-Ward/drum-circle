using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [HideInInspector] public TreeSpawning treeSpawner;

    public GameObject explosionVFX;

    public float speed = 1200f;
    public float rotationSpeed = 10f;

    private Vector3 heading;
    private bool willSpawn;
    private int playerIndex;

    public void StartUp(int playerIndex)
    {
        this.playerIndex = playerIndex;


        Tuple<Vector3, bool> closestSpawn = treeSpawner.getClosestSpawn();
        heading = closestSpawn.Item1;
        willSpawn = closestSpawn.Item2;
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
        if(willSpawn)
        {
            treeSpawner.spawnAtLocation(playerIndex+1, heading, true);
        }
        Destroy(gameObject);
    }
}
