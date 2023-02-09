using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour

{
    private Queue<GameObject> beatQueue = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
        {
            Queue<GameObject> beatQueue;
        }
    // Update is called once per frame
    void Update()
        {
        
        }
    public void AddToQueue(GameObject beat)
        {
            beatQueue.Enqueue(beat);
        }
    public void BeatDelete()
        {
            GameObject lastelem = beatQueue.Dequeue();
            Destroy(lastelem);
        }
}
