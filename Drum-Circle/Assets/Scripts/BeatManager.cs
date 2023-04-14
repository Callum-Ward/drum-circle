using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Beat {
    public GameObject obj;
    public int oneShotIndex;
}

public class BeatManager : MonoBehaviour
{
    public Queue<Beat>[] beatQueues;
    public float deleteDelay = 0.15f;
    private int playerCount;

    // Start is called before the first frame update
    void Start()
        {
            beatQueues = new Queue<Beat>[playerCount * 2];
            for(int i = 0; i < playerCount * 2; i++)
            {
                beatQueues[i] = new Queue<Beat>();
            }
        }

    //Checks if beats need to be deleted every frame.
    void Update()
    {
        for(int i = 0; i < playerCount * 2; i++)
        {
            try{
                Beat beat = beatQueues[i].Peek();
                if (beat.obj.GetComponent<MoveBeat>().delete == true)
                {
                    BeatDelete(i, false);
                }
            } catch {
                
            }
        }
    }

    //Sets the total number of players from main scene script
    public void setPlayerCount(int playerCount)
    {
        this.playerCount = playerCount;
    }

    //Queues for beats based on track.
    public void AddToQueue(int queueIndex, GameObject obj, int oneShotIndex)
        {
            Beat beat = new Beat();
            beat.obj = obj;
            beat.oneShotIndex = oneShotIndex;
            beatQueues[queueIndex].Enqueue(beat);
        }

    //Deletes beat from queue if queue isn't empty is available.
    public void BeatDelete(int queueIndex, bool highlight)
        {
        if (beatQueues[queueIndex].Count > 0)
        {
            GameObject lastelem = beatQueues[queueIndex].Dequeue().obj;
            lastelem.GetComponent<MoveBeat>().fade = true;
            lastelem.GetComponent<MoveBeat>().highlight = highlight;
            StartCoroutine(WindowDelay(deleteDelay, lastelem));
        }
    }

    //Coroutine to delay destruction of object after specified amount of time.
    IEnumerator WindowDelay(float time, GameObject o)
    {
        yield return new WaitForSeconds(time);
        
        VisualElement beat = o.GetComponent<MoveBeatUI>().beatSpawnContainer;
        VisualElement parent = beat.parent;
        parent.Remove(beat);

        Destroy(o);
    }
}
