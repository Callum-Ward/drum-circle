using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBeat : MonoBehaviour
{
    // Start is called before the first frame update
    private float moveSpeed = 2;
    private float removeHeight = 1;
    //private float tapArea = 1.3f;
    public ScoreManager scoreManager;
    public BeatManager beatManager;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        //Debug.Log("remove height: " + removeHeight);
        //Debug.Log("tap height: " + tapArea);

        if (transform.position.y < removeHeight)
        {
            scoreManager.Miss();
            beatManager.BeatDelete();
        }

       // if (transform.position.y < tapArea && Input.GetKeyDown(KeyCode.LeftArrow))
        //{
            //Destroy(gameObject);
        //}
    }

}
