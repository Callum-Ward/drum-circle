using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [Range(0.1f, 15f)] public float growthRate = 1; 
    public GameObject[] trees;
    [SerializeField] bool testing = false;

    ScoreManager scoreManager = null;

    bool hitStatus = false;

    void Start()
    {
        //Get all the procedural trees
        trees = GameObject.FindGameObjectsWithTag("Procedural Tree");
        if (!testing)
        {
            scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        }
    }

    void Update()
    {
        //this is used for debugging/development
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
            //Get just the trees that are growing
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

            if (hitStatus == true) //this is always true (legacy)
            {
                
                foreach (var t in this.trees)
                {
                    var tree = t.GetComponent<Tree>();

                    //avrage the score multipliers
                    float scoreMul = 0;
                    foreach (var mul in scoreManager.ScoreMultiplier) {
                        scoreMul += mul;
                    }
                    if (scoreMul < 3) scoreMul = 3;
                    scoreMul = scoreMul / 3f;

                    //grow the tree based on the score multiplier average and growth rate
                    tree.Grow(scoreMul * growthRate);
                    //always try to add branches 
                    tree.AddBranches(0);
                }
            }
        }
    }

    //legacy, please ignore
    public void SetHitStatus(bool hit) 
    {
        hitStatus = hit;
    }
}
