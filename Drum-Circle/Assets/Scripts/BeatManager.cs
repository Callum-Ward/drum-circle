using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour

{
    public Queue<GameObject> beatQueueL = new Queue<GameObject>();
    public Queue<GameObject> beatQueueR = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
        {
            //Queue<GameObject> beatQueue;
        }
    // Update is called once per frame
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
    public void AddToQueueL(GameObject beat)
        {
            beatQueueL.Enqueue(beat);
        }
    public void AddToQueueR(GameObject beat)
        {
            beatQueueR.Enqueue(beat);
        }
    public void BeatDelete(string side, bool highlight)
        {
        if (beatQueueL.Count > 0)
        {
            if (side == "left")
            {
                GameObject lastelem = beatQueueL.Dequeue();
                lastelem.GetComponent<MoveBeat>().fade = true;
                lastelem.GetComponent<MoveBeat>().highlight = highlight;
                StartCoroutine(WindowDelay(0.15f, lastelem));
            }
            else if (side == "right")
            {
                GameObject lastelem = beatQueueR.Dequeue();
                lastelem.GetComponent<MoveBeat>().fade = true;
                lastelem.GetComponent<MoveBeat>().highlight = highlight;
                StartCoroutine(WindowDelay(0.15f, lastelem));
            }
        }
    }
    IEnumerator WindowDelay(float time, GameObject o)
    {
        yield return new WaitForSeconds(time);

        Destroy(o);
    }
}
