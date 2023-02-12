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
            if (beatL.GetComponent<MoveBeat>().window == true)
            {
                BeatHighlight(beatL);
            }
            if (beatL.GetComponent<MoveBeat>().delete == true)
            {
                BeatDelete("left");
            }

        }
        if (beatQueueR.Count > 0)
        {
            GameObject beatR = beatQueueR.Peek();
            if (beatR.GetComponent<MoveBeat>().window == true)
            {
                BeatHighlight(beatR);
            }
            if (beatR.GetComponent<MoveBeat>().delete == true)
            {
                BeatDelete("right");
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
    public void BeatDelete(string side)
        {
        if (beatQueueL.Count > 0)
        {
            if (side == "left")
            {
                GameObject lastelem = beatQueueL.Dequeue();
                Destroy(lastelem);
            }
            else if (side == "right")
            {
                GameObject lastelem = beatQueueR.Dequeue();
                Destroy(lastelem);
            }
        }
    }
    public void BeatHighlight(GameObject beat)
    {
        Color color = new Color(255,215,0,1);
        MeshRenderer beatRenderer = beat.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = color;
        beatRenderer.material = newMaterial;
    }
}
