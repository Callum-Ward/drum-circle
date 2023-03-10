using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Numerics;
using UnityEngine.Rendering;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Tree : MonoBehaviour
{
    [SerializeField] GameObject branchObj;

    [SerializeField] float length = 0;

    [Range(0.0f, 90.0f)] public float orientation = 45.0f;
    [Range(0.0f, 180.0f)] public float rotation = 137.5f;

    [Range (0.0f, 1.0f)] public float lengthDecay = 0.33f;
    [Range (0.0f, 1.0f)] public float directionNoise;

    [SerializeField] float maxDepth = 10;

    List<GameObject> branches = new();

    int depth = 1;
    float currentRotation = 0;

    private void Start()
    {
        InitialiseTree();
    }

    private void Update()
    {
       
    }

    /*Returns random value between the min and max according to normal distribution*/
    float RandomNormal( float min=-1, float max=1 )
    {
        float u, v, s;

        do
        {
            u = 2 * Random.value - 1;
            v = 2 * Random.value - 1;

            s = u * u + v * v;
        } while (s >= 1);

        float std = u * Mathf.Sqrt(-2 * Mathf.Log(s) / s);

        float mean = (min + max) / 2;
        float sigma = (max - mean) / 3;
        
        return Mathf.Clamp(std * sigma + mean, min, max);
    }

    /* Wrapper for RandomNormal(min, max)*/
    float RandomNormal( float range )
    {
        return RandomNormal(-range, range);
    }

    /*Initialise new tree and set the first branch*/
    void InitialiseTree()
        {
        currentRotation = Random.Range(0, 360);

        //The startitng position of the branch is the position of the tree
        var pos = this.transform.position;

        //The basis vector is up
        var basis = Vector3.up * length; 

        //Genarate the rotation vector by sampling a normal distribution
        var xangle = RandomNormal(directionNoise) * 90;
        var zangle = RandomNormal(directionNoise) * 90;
        var rotation = new Vector3(xangle, 0, zangle);

        var growth = Quaternion.Euler(rotation) * basis;

        //Instantiate new branch and add it to branch list
        var root = Instantiate(branchObj);
        var rootBranch = root.GetComponent<Branch>();

        rootBranch.SetBranch(this.transform.gameObject, growth, pos);
        
        branches.Add(root);
    }

    public void Grow(float scoreMul)
    {
        foreach( var br in branches )
        {
            var branch = br.GetComponent<Branch>();
            branch.Grow(scoreMul);
        }
    }
    
    /*Adds a new set of branches to the tree*/
    public void AddBranches()
    {
        //cotinue if we max depth hasn't been reached yet
        if (depth > maxDepth) return;

        //the length of the branches decay baesed on the depth
        var bLength = length * Mathf.Exp(-lengthDecay * depth);

        var newBranches = new List<GameObject>();

        foreach (var br in branches)
            {
            var branch = br.GetComponent<Branch>();

            //only fully grown leaf branches can grow new branches
            if ( !(branch.isLeaf && branch.isFullyGrown) ) continue;

            branch.isLeaf = false;


            //Set up branch A

            //the position of the A branch is the top of the parent branch
            var posA = branch.position + branch.growth;

            //the basis vector of the A branch is the same as the parent branch
            var basisA = Vector3.Normalize(branch.growth) * bLength;

            //set up a noise vector 
            var xangleA = RandomNormal(directionNoise) * 90;
            var zangleA = RandomNormal(directionNoise) * 90;
            var noiseA = new Vector3(xangleA, 0, zangleA);

            var growthA = Quaternion.Euler(noiseA) * basisA;


            //Setup branch B

            //the starting position of branch B is somewhere along the parent brnach
            var posB = branch.position + branch.growth * RandomNormal(0, 1);

            //the basis vector of branch B is at an angle from the parent specified by this.orientation
            //and is rotated around it by an angle specified by this.rotation
            var basisB = Quaternion.Euler(this.orientation, currentRotation, 0) * 
                Vector3.Normalize(branch.growth) * bLength;

            //set up a noise vector
            var xangleB = RandomNormal(directionNoise) * 90;
            var zangleB = RandomNormal(directionNoise) * 90;
            var noiseB = new Vector3(xangleB, 0, zangleB);

            var growthB = Quaternion.Euler(noiseB) * basisB;


            //Instantiate the branches
            var a = Instantiate(branchObj);
            var branchA = a.GetComponent<Branch>();

            var b = Instantiate(branchObj);
            var branchB = b.GetComponent<Branch>();

            //Set them as the current branch's children
            branch.SetChildren(branchA, branchB);

            branchA.SetBranch(this.transform.gameObject ,branch, growthA, posA);
            branchB.SetBranch(this.transform.gameObject, branch, growthB, posB);

            newBranches.Add(a);
            newBranches.Add(b);

            //Increment the current rotation by this.rotation
            currentRotation = (currentRotation + this.rotation) % 360;

            //increment the depth
            depth++;
        }

        branches = branches.Concat(newBranches).ToList();
    }
}
