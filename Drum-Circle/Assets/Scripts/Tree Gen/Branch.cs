using System;
using UnityEngine;
using Unity.Collections;
public class Branch : MonoBehaviour {
    public int id = 1;
    public int depth = 0;
    public bool leaf = true;

    public Branch branchA, branchB, parent;

    public float ratio, spread, splitSize;
    public float splitDecay = 1;

    public Vector3 dir = Vector3.up;
    public float length = 0.0f, radius = 0.0f, area = 0.1f;

    void Start() {
        

        transform.localScale = new Vector3(radius, length, radius);
    }

    void Update() {
        if (Input.GetKeyDown("space"))
            Grow(5);

    }

    void SetBranch(int id, Branch parent) {
        this.id = id;

        this.leaf = true;

        this.parent = parent;

        this.depth = parent.depth + 1;

        this.ratio = parent.ratio;
        this.spread = parent.spread;
        this.splitSize = parent.splitSize;
        this.splitDecay = parent.splitDecay;
    }

    public void Grow(float feed) {
        if (leaf) {
            length += (float)Math.Cbrt(feed);
            feed -= (float)Math.Cbrt(feed);
            area += feed / length; 

            if (length > splitSize * Math.Exp(-splitDecay * depth)) {
                Split();
            } 
        }

        else {
            float pass = (branchA.area + branchB.area) / (branchA.area + branchB.area + this.area);

            area += pass * feed / length;
            feed *= (1 - pass); 

            branchA.Grow(ratio * feed);
            branchB.Grow((1 - ratio) * feed);
        }

        radius = (float)Math.Sqrt((double)area/Math.PI);

        transform.localScale = Vector3.one * length / 1000;
    }

    public void Split() {
        this.leaf = false;

        var a = Instantiate(this.gameObject);
        var b = Instantiate(this.gameObject);

        var tupleA = Tuple.Create(a, this.id * 2, this);
        var tupleB = Tuple.Create(b, this.id * 2 + 1, this);

        this.branchA = a.GetComponent<Branch>();
        this.branchB = b.GetComponent<Branch>();

        branchA.SetBranch(id * 2, this);
        branchB.SetBranch(id * 2 + 1, this);

        Vector3 density = leafDensity(this.depth);
        Vector3 norm = Vector3.Cross(density, this.dir).normalized;
        Vector3 reflect = -1.0f * norm;

        System.Random r = new System.Random();
        float randFlip = (float)r.NextDouble();

        branchA.dir = Vector3.Normalize(randFlip * this.spread * norm * (1 - ratio) +
            dir * ratio);
        a.transform.parent = this.gameObject.transform;
        a.transform.position = transform.up * transform.localScale.y;
        a.transform.rotation = Quaternion.Euler(branchA.dir);

        branchB.dir = Vector3.Normalize(randFlip * this.spread * reflect * ratio +
            dir * (1 - ratio));
        b.transform.parent = this.gameObject.transform;
        b.transform.position = transform.up * transform.localScale.y;
        b.transform.rotation = Quaternion.Euler(branchB.dir);
    }

    Vector3 leafAverage(Branch branch) {
        if (branch.leaf) return branch.length * branch.dir;

        return branch.length * branch.dir +
            branch.ratio * leafAverage(branch.branchA) +
            (1 - branch.ratio) * leafAverage(branch.branchB);

    }

    Vector3 leafDensity(int searchDepth) {
        System.Random r = new System.Random();
        Vector3 rand = -new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()).normalized;

        Branch ancestor = this;
        Vector3 rel = new Vector3(0, 0, 0);

        while (ancestor.depth > 0 && searchDepth > 0) {
            rel += ancestor.dir * ancestor.length;
            ancestor = ancestor.parent;

            searchDepth--;
        }

        return Vector3.Normalize(leafAverage(ancestor) - rel) + rand; // <- REDO!
    }
}