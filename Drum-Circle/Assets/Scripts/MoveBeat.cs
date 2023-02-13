using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBeat : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float timer = 0f;
    private float windowtime = 0f;
    public bool window = false;
    public bool delete = false;
    public float windowScore = 0f;
    private float alpha = 1.0f;
    public bool fade = false;
    public bool highlight = false;
     

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

        if (timer > (beatmapScript.delay + (beatmapScript.windowtime)))
        {
            delete = true;
        }

        else if (timer >= (beatmapScript.delay - windowtime/3))
        {
            window = true;
            windowScore = Mathf.Abs(timer - beatmapScript.delay);
        }

        else if (timer < (beatmapScript.delay - (windowtime/2))) 
        {
            window = false;
        }
        if (fade == true) {
            moveSpeed = 0.2f;
            alpha = 0.0f;
            BeatHighlight();
        }
    }

public void BeatHighlight()
    {
        Color color = new Color(50,50,50,alpha);
        if(highlight){
            color = new Color(255,215,0,alpha);
        }
        MeshRenderer beatRenderer = gameObject.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = color;
        beatRenderer.material = newMaterial;
    }
}
