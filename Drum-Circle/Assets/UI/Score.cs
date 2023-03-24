using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public float ScoreValue = 0;
    public Text ScoreText;
    public ScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Camera.main.transform.position + new Vector3(4f, 1.75f, 4f);
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {

        ScoreValue = (int)scoreManager.Score;

        ScoreText.text = ScoreValue.ToString();
    }
}
