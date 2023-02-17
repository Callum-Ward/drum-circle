using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour

{
    public Queue<GameObject> beatQueueL = new Queue<GameObject>();
    public Queue<GameObject> beatQueueR = new Queue<GameObject>();
    public float deleteDelay = 0.15f;


    // Start is called before the first frame update
    void Start()
        {
            //Queue<GameObject> beatQueue;
        }

    //Checks if beats need to be deleted every frame.
    void Update()
        {
        if (beatQueueL.Count > 0)
        {
            GameObject beatL = beatQueueL.Peek();
            if (beatL.GetComponent<MoveBeat>().delete == true)
            {
                BeatDelete("left", false);
            }

        }
        if (beatQueueR.Count > 0)
        {
            GameObject beatR = beatQueueR.Peek();
            if (beatR.GetComponent<MoveBeat>().delete == true)
            {
                BeatDelete("right", false);
            }
        }
    }

    //Queues for beats based on track.
    public void AddToQueueL(GameObject beat)
        {
            beatQueueL.Enqueue(beat);
        }
    public void AddToQueueR(GameObject beat)
        {
            beatQueueR.Enqueue(beat);
        }

    //Deletes beat from queue if queue isn't empty is available.
    public void BeatDelete(string side, bool highlight)
        {
        if (beatQueueL.Count > 0)
        {
            if (side == "left")
            {
                GameObject lastelem = beatQueueL.Dequeue();
                lastelem.GetComponent<MoveBeat>().fade = true;
                lastelem.GetComponent<MoveBeat>().highlight = highlight;
                StartCoroutine(WindowDelay(deleteDelay, lastelem));
            }
            else if (side == "right")
            {
                GameObject lastelem = beatQueueR.Dequeue();
                lastelem.GetComponent<MoveBeat>().fade = true;
                lastelem.GetComponent<MoveBeat>().highlight = highlight;
                StartCoroutine(WindowDelay(deleteDelay, lastelem));
            }
        }
    }

    //Coroutine to delay destruction of object after specified amount of time.
    IEnumerator WindowDelay(float time, GameObject o)
    {
        yield return new WaitForSeconds(time);

        Destroy(o);
    }
}
