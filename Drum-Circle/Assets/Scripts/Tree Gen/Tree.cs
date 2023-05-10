using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class Tree : MonoBehaviour
{
    [SerializeField] GameObject branchObj;

    public float length = 0;
    [Range(0.001f, 0.25f)] public float width = 1;

    [HideInInspector] public float totalLength;

    [Range(0.0f, 90.0f)] public float orientation = 45.0f;
    [Range(0.0f, 180.0f)] public float rotation = 137.5f;

    [Range(0.0f, 1.0f)] public float lengthDecay = 0.33f;
    [Range(0.0f, 1.0f)] public float directionNoise;

    [SerializeField] float maxDepth = 10;
    public bool isFullyGrown = false;

    public int lod;

    List<GameObject> branches = new();
    Branch root = null;

    int depth = 1;
    float currentRotation = 0;

    private void Awake()
    {
        // On instantiation get the lod level and randomise the rotation
        lod = GetComponent<LOD>().lod;
        currentRotation = Random.Range(0, 360);

        InitialiseTree();
    }

    private void Update()
    {
        //needed for the branches' mesh generation
        lod = GetComponent<LOD>().lod;
    }

    /*Returns random value between the min and max according to normal distribution*/
    float RandomNormal(float min = -1, float max = 1)
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
    float RandomNormal(float range)
    {
        return RandomNormal(-range, range);
    }

    /* Returns the curve of the branch*/
    Curve GetBranchCurve(Vector3 start, Vector3 end)
    {
        // The controll points of the branch are set along the growth vector when direction
        // noise is set to 0. The distance between control point 1 and start is somwhere 
        // between none and 2 thirds of the groth vector, and likewise between the second 
        // controll point and the end.

        var cp1_xangle = RandomNormal(directionNoise) * 45;
        var cp1_zangle = RandomNormal(directionNoise) * 45;
        var cp1 = start + (start + end) * RandomNormal(0f, 0.66f);
        cp1 = Quaternion.Euler(cp1_xangle, 0, cp1_zangle) * cp1;

        var cp2_xangle = RandomNormal(directionNoise) * 45;
        var cp2_zangle = RandomNormal(directionNoise) * 45;
        var cp2 = end + (start - end) * RandomNormal(0f, 0.66f);
        cp2 = Quaternion.Euler(cp2_xangle, 0, cp2_zangle) * cp2;

        return new Curve(start, cp1, cp2, end);
    }

    /*Initialise new tree and set the first branch*/
    void InitialiseTree()
    {
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

        var curve = GetBranchCurve(Vector3.zero, growth);


        // If the tree has branches i.e. not a palm tree
        if (maxDepth > 0)
        {
            // We want the trees to soawn in in the shape of a tree and not growning from nothing

            // Set the root branch to generate fully grown
            rootBranch.SetBranch(this.transform.gameObject, growth, basis, pos, width, curve, 1);
            rootBranch.isFullyGrown = false;

            this.root = rootBranch;
            branches.Add(root);

            // Grow to generate the root branch's mesh
            Grow(1);

            rootBranch.isFullyGrown = true;

            // Add branches to be half grown
            AddBranches(0.5f);
            // Grow to generate their mesh
            Grow(1);
        }
        else
        {
            //If it is a palm tree, we want to spawn it in half grown
            rootBranch.SetBranch(this.transform.gameObject, growth, basis, pos, width, curve, 0.5f);
            this.root = rootBranch;
            branches.Add(root);
        }
    }

    /* Grow the tree */
    public void Grow(float scoreMul)
    {
        //If the tree is fully grown return early,
        if (this.isFullyGrown)
        {
            return;
        }

        // Go through all the branches and ccheck if they are grown.
        // If one of them is not fully grown it means the tree is not fully grown
        bool fullyGrownCheck = true;
        foreach (var br in branches)
        {
            var branch = br.GetComponent<Branch>();

            branch.Grow(scoreMul);
            if (!branch.isFullyGrown) fullyGrownCheck = false;
        }

        // If all the branches are fully grown and the tree has reached max depth, regenerate
        // the branches' meshes at the highest level of detail and mark the tree as fully grown.
        if (depth > maxDepth && fullyGrownCheck)
        {
            lod = 0;
            foreach (var br in branches)
            {
                var branch = br.GetComponent<Branch>();
                branch.Grow(1);
            }
            this.isFullyGrown = true;
        }
    }

    /*Adds a new set of branches to the tree*/
    public void AddBranches(float growthPhase)
    {
        //cotinue if we max depth hasn't been reached yet
        if (depth > maxDepth) return;

        //the length of the branches decay baesed on the depth
        //var bLength = length * Mathf.Pow( (1 - lengthDecay), depth);
        var bLength = length * Mathf.Exp(-lengthDecay * depth);

        var newBranches = new List<GameObject>();

        var addedBranches = false;

        foreach (var br in branches)
        {
            var branch = br.GetComponent<Branch>();

            //only fully grown leaf branches can grow new branches
            if (!(branch.isLeaf && branch.isFullyGrown)) continue;


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

            var curveA = GetBranchCurve(Vector3.zero, growthA);
            var cp1 = (branch.growth - branch.curve.P2) * curveA.P1.magnitude;
            curveA.P1 = cp1;

            //Setup branch B

            //the starting position of branch B is somewhere along the parent brnach
            var posB = branch.position + branch.curve.GetPointAlongCurve(RandomNormal(0, 1));

            //the basis vector of branch B is at an angle from the parent specified by this.orientation
            //and is rotated around it by an angle specified by this.rotation
            var basisB = Quaternion.Euler(this.orientation, currentRotation, 0) *
                Vector3.Normalize(branch.growth) * bLength;

            //set up a noise vector
            var xangleB = RandomNormal(directionNoise) * 90;
            var zangleB = RandomNormal(directionNoise) * 90;
            var noiseB = new Vector3(xangleB, 0, zangleB);

            var growthB = Quaternion.Euler(noiseB) * basisB;

            var curveB = GetBranchCurve(Vector3.zero, growthB);


            //Instantiate the branches
            var a = Instantiate(branchObj);
            a.GetComponent<MeshRenderer>().enabled = false;
            var branchA = a.GetComponent<Branch>();

            var b = Instantiate(branchObj);
            b.GetComponent<MeshRenderer>().enabled = false;
            var branchB = b.GetComponent<Branch>();

            //Set them as the current branch's children
            branch.SetChildren(branchA, branchB);

            branchA.SetBranch(this.transform.gameObject, branch, growthA, basisA, posA, width, curveA, growthPhase);
            branchB.SetBranch(this.transform.gameObject, branch, growthB, basisB, posB, width, curveB, growthPhase);

            newBranches.Add(a);
            newBranches.Add(b);

            branch.isLeaf = false;

            //Increment the current rotation by this.rotation
            currentRotation = (currentRotation + this.rotation) % 360;

            addedBranches = true;
        }
        //increment the depth
        if (addedBranches) depth++;

        branches = branches.Concat(newBranches).ToList();
    }

}