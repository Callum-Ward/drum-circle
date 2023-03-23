using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [SerializeField] GameObject[] trees;
    [SerializeField] bool testing = false;

    ScoreManager scoreManager = null;
    float lastScore = 0;


    void Start()
    {
        trees = GameObject.FindGameObjectsWithTag("Procedural Tree");

        if (!testing)
        {
            scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        }
    }

    void Update()
    {
        if (testing)
        {
            foreach (var t in trees)
            {
                var tree = t.GetComponent<Tree>();

                if (Input.GetKeyDown(KeyCode.A)) tree.AddBranches();
                if (Input.GetKey(KeyCode.Space)) tree.Grow(1);
            }
        }

        else
        {
            if (scoreManager.Score > lastScore)
            {
                foreach (var t in trees)
                {
                    var tree = t.GetComponent<Tree>();

                    tree.Grow(scoreManager.ScoreMultiplier);
                    tree.AddBranches();

                    lastScore = scoreManager.Score;
                }
            }
        }
    }
}
