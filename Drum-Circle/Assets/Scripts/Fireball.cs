using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [HideInInspector] public TreeSpawning treeSpawner;

    public GameObject explosionVFX;

    public float speed = 12f;
    public float rotationSpeed = 60f;

    private Vector3 heading;
    private int playerIndex;

    public void StartUp(int playerIndex)
    {
        this.playerIndex = playerIndex;
        this.heading = treeSpawner.getSpawnLocation();
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
        Destroy(gameObject);
        treeSpawner.spawnAtLocation(playerIndex+1, heading, true);
    }
}
