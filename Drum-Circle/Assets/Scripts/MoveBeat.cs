using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBeat : MonoBehaviour
{
    private float moveSpeed = 2f;
    private float timer = 0f;
    private float windowtime = 0f;
    public bool window = false;
    public bool delete = false;
     

    public ScoreManager scoreManager;
    public BeatManager beatManager;
    public BeatmapScript beatmapScript;
    public AudioManager audioManager;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatmapScript = GameObject.Find("Rhythm Logic").GetComponent<BeatmapScript>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        timer += Time.deltaTime;
        windowtime = beatmapScript.windowtime;

        if (timer > (beatmapScript.delay + (beatmapScript.windowtime / 2)))
        {
            delete = true;
        }

        else if (timer >= (beatmapScript.delay - (windowtime/2)))
        {
            window = true;
        }

        else if (timer < (beatmapScript.delay - (windowtime / 2))) 
        {
            window = false;
        }

        //if (transform.position.y < removeHeight - (windowTime))
        //{
        //    scoreManager.Miss();
        //    beatManager.BeatDelete();
        //}

       // if (transform.position.y < tapArea && Input.GetKeyDown(KeyCode.LeftArrow))
        //{
            //Destroy(gameObject);
        //}
    }

}
