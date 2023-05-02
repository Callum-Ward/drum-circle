using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [Range(0.1f, 3f)] public float growthRate = 1; 
    public GameObject[] trees;
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

                if (Input.GetKeyDown(KeyCode.A)) tree.AddBranches(0);
                if (Input.GetKey(KeyCode.Space)) tree.Grow(10 * growthRate);
            }
        }

        else
        {
            trees = GameObject.FindGameObjectsWithTag("Procedural Tree");
            var growingTrees = new GameObject[] { };
            foreach (var t in trees)
            {
                var tree = t.GetComponent<Tree>();
                if (!tree.isFullyGrown)
                {
                    growingTrees.Append(t);
                }
            }
            trees = growingTrees;

            if (hitStatus == true)
            {
                
                foreach (var t in trees)
                {
                    var tree = t.GetComponent<Tree>();

                    tree.Grow(scoreManager.ScoreMultiplier[0] + growthRate);

                    tree.AddBranches();
                }
            }
        }
    }

    public void SetHitStatus(bool hit) 
    {
        hitStatus = hit;
    }
}
