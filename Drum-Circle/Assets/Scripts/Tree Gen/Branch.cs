using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public Branch parent = null;
    public Branch childA  = null;
    public Branch childB  = null;
    public Vector3 growth;
    public Vector3 position;

    public bool isLeaf = true;
    public bool isFullyGrown = false;

    private float maxLength = 1000;
    private float length = 0.00001f;

    private LineRenderer lr;
    private ScoreManager sm;
    private float lastScore;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        sm = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

        lastScore = sm.Score;
    }

    private void Update()
    {
        //grow while space is pressed
        //if (Input.GetKey(KeyCode.Space)) Grow();

        if (sm.Score > lastScore)
        {
            Grow();
            lastScore = sm.Score;
        }
    }

    /*Set up branch parameters*/
    public void SetBranch(GameObject tree, Vector3 growth, Vector3 position)
    {
        this.growth = growth;
        this.position = position;

        maxLength = growth.magnitude;

        isLeaf = true;
        isFullyGrown = false;

        Place(tree);
    }

    /*Set up branch parameters*/
    public void SetBranch(GameObject tree, Branch parent, Vector3 growth, Vector3 position)
    {
        this.parent = parent;

        SetBranch(tree, growth, position);
    }

    /*Places branch in place*/
    public void Place(GameObject tree)
    {
        this.transform.parent = tree.transform;
        this.transform.position = position;

        length = 0;

        lr.positionCount = 2;

        lr.SetPosition(0, position);
        lr.SetPosition(1, position);
    }

    /*Set the children of a brnach*/
    public void SetChildren(Branch a, Branch b)
    {
        childA = a;
        childB = b;
    }

    void Grow()
    {
        if (!isLeaf) return;

        if(!isFullyGrown)
        {
            length += maxLength / 100 * sm.ScoreMultiplier;


            if (length >= maxLength) isFullyGrown = true;

            var endpoint = position + length / maxLength * growth;

            lr.SetPosition(1, endpoint );
        }
    }

}
