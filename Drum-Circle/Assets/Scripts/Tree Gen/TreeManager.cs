using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [Range(0.1f, 15f)] public float growthRate = 1; 
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
            var growingTrees = new List<GameObject>( GameObject.FindGameObjectsWithTag("Procedural Tree") );
            foreach (var t in growingTrees.ToArray())
            {
                var tree = t.GetComponent<Tree>();
                if (tree.isFullyGrown)
                {
                    growingTrees.Remove(t);
                }
            }
            this.trees = growingTrees.ToArray();

            if (hitStatus == true)
            {
                
                foreach (var t in this.trees)
                {
                    var tree = t.GetComponent<Tree>();

                    float scoreMul = 0;
                    foreach (var mul in scoreManager.ScoreMultiplier) {
                        scoreMul += mul;
                    }
                    if (scoreMul < 3) scoreMul = 3;
                    scoreMul = scoreMul / 3f;

                    tree.Grow(scoreMul * growthRate);

                    tree.AddBranches(0);
                }
            }
        }
    }

    public void SetHitStatus(bool hit) 
    {
        hitStatus = hit;
    }
}
