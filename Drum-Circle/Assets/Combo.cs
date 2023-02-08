using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    public int ComboValue = 0;
    public int ComboCounter = 0;
    public Text ComboText;
    public ScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Camera.main.transform.position + new Vector3(4f, 1f, 4f);
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {

        ComboValue = (int)scoreManager.ScoreMultiplier;
        ComboCounter = scoreManager.ComboCount;

        ComboText.text = "Combo: " + ComboCounter + " Multiplier: " + ComboValue.ToString() + "x";
    }
}