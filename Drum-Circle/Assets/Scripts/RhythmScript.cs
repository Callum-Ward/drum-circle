using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmScript : MonoBehaviour
{
    public RhythmSpawner spawner;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<RhythmSpawner>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 2)
        {
            spawner.spawn(1, 1, 1);
            spawner.spawn(1, 0, 1);
            spawner.spawn(3, 1, 1);
            spawner.spawn(3, 0, 1);
            spawner.spawn(5, 1, 1);
            spawner.spawn(5, 0, 1);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
        
        
    }
}
