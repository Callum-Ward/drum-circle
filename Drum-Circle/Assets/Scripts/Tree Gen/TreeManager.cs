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
    bool hitStatus = false;

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
                if (Input.GetKey(KeyCode.Space)) tree.Grow(10);
            }
        }

        else
        {
            trees = GameObject.FindGameObjectsWithTag("Procedural Tree");
            
            if (hitStatus == true)
            {
                var modifiedScore = false;
                if(scoreManager.Score > lastScore) 
                {
                    modifiedScore = true;
                    lastScore = scoreManager.Score;
                }
                
                foreach (var t in trees)
                {
                    var tree = t.GetComponent<Tree>();

                    tree.Grow(scoreManager.ScoreMultiplier);

                    if(modifiedScore) tree.AddBranches();
                }
            }
        }
    }

    public void SetHitStatus(bool hit) 
    {
        hitStatus = hit;
    }
}
