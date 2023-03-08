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

    public NewBranch[] branches = null;

    int depth = 1;

    private void Start()
    {
        InitialiseTree();
    }

    private void Update()
    {
        if (Input.GetKeyDown("a")) AddBranch();
    }

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

    float RandomNormal( float range )
    {
        return RandomNormal(-range, range);
    }

    void InitialiseTree()
        {
        var pos = this.transform.position;
        var basis = Vector3.up * length; 

        var xangle = RandomNormal(directionNoise) * 90;
        var yangle = RandomNormal(directionNoise) * 90;

        var growth = Quaternion.Euler(xangle, yangle, 0) * basis;

        var root = Instantiate(branchObj);
        var rootBranch = root.GetComponent<NewBranch>();

        rootBranch.SetBranch(this.transform.gameObject, growth, pos);
        
        branches.Append(rootBranch);
    }
    
    void AddBranch()
    {
        var bLength = length * Mathf.Exp(-lengthDecay * depth);

       
        foreach (var branch in branches)
        {
            if ( !(branch.isLeaf && branch.isFullyGrown) ) continue;

            var posA = branch.position + branch.growth;
            var basisA = Vector3.Normalize(branch.growth) * bLength;

            var xangleA = RandomNormal(directionNoise);
            var yangleA = RandomNormal(directionNoise);

            var growthA = Quaternion.Euler(xangleA, yangleA, 0) * basisA;

            var posB = branch.growth * RandomNormal(0, branch.growth.magnitude);
            var basisB = Quaternion.Euler(orientation, 0, rotation) * 
                Vector3.Normalize(branch.growth) * bLength;

            var xangleB = RandomNormal(directionNoise);
            var yangleB = RandomNormal(directionNoise);

            var growthB = Quaternion.Euler(xangleB, yangleB, 0) * basisB;

            var a = Instantiate(branchObj);
            var branchA = a.GetComponent<NewBranch>();

            var b = Instantiate(branchObj);
            var branchB = b.GetComponent<NewBranch>();

            branchA.SetBranch(this.transform.gameObject ,branch, growthA, posA);
            branchB.SetBranch(this.transform.gameObject, branch, growthB, posB);

            branches.Append(branchA);
            branches.Append(branchB);
        }
        
        depth++;
    }
}
