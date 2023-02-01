using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmSpawner : MonoBehaviour
{
    public GameObject square;
    public float spawnRate = 2;
    private float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        //setup beat spawner location with respect to camera position
        transform.position = Camera.main.transform.position + new Vector3(0f,2.5f,4f);
        //setup square beat spawn location to the left
        square.transform.position = transform.position + new Vector3(-1f, 0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (timer<spawnRate)
        {
            timer += Time.deltaTime;
        } else
        {
            Instantiate(square, square.transform.position, square.transform.rotation);
            timer = 0;
        }

        
        
    }
}
