using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float Score = 0f;
    public float ScoreMultiplier = 1f;
    public int ComboCounter = 0;
    public int ComboCount = 0;

    public void Miss()
    {
        ComboCounter = 0;
        ComboCount = 0;
        ScoreMultiplier = 1f;
    }

    public void Hit(float proximity)
    {
        ComboCounter++;
        ComboCount++;
        Score += proximity * 100;

        if (ScoreMultiplier < 5 && ComboCounter >= 20)
        {
            ComboCounter = 0;
            ScoreMultiplier++;
        }
    }
}
