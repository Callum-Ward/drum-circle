using System;
using UnityEngine;

public class Branch : MonoBehaviour {
    public int id = 0;
    public int depth = 0;
    public bool leaf = true;

    public Branch a, b, parent;

    public float ratio, spread, splitSize;
    public float splitDecay = 1;

    public Vector3 dir = Vector3.up;
    public float length = 0.0f, radius = 0.0f, area = 0.1f;

    Branch(float ratio, float spread, float splitSize) {
        this.ratio = ratio;
        this.spread = spread;
        this.splitSize = splitSize;
    }

    Branch(Branch b, int id) {
        this.id = id;

        this.ratio = b.ratio;
        this.spread = b.spread;
        this.splitSize = b.splitSize;

        this.depth = b.depth + 1;
        parent = b;
    }

    void Start() {
    }

    void Update() {
    }

    void Grow(float feed) {
        radius = (float)Math.Sqrt(area/Math.PI);

        if (leaf) {
            length += (float)Math.Cbrt(feed);
            feed -= (float)Math.Cbrt(feed);
            area += feed / length;

            if (length > splitSize * Math.Exp(-splitDecay * depth)) {
                Split();
            } 
        }

        else {
            float pass = (a.area + b.area) / (a.area + b.area + this.area);

            area += pass * feed / length;
            feed *= (1 - pass); 

            a.Grow(ratio * feed);
            b.Grow((1 - ratio) * feed);
        }
    }

    public void Split() {
        this.leaf = false;

        a = new Branch(this, this.id * 2);
        b = new Branch(this, this.id * 2 + 1);

        Vector3 density = leafDensity(this.depth);
        Vector3 norm = Vector3.Cross(density, this.dir).normalized;
        Vector3 reflect = -1.0f * norm;

        System.Random r = new System.Random();
        float randFlip = (float)r.NextDouble();

        a.dir = Vector3.Normalize(randFlip * this.spread * norm * (1 - ratio) +
            dir * ratio);
        b.dir = Vector3.Normalize(randFlip * this.spread * reflect * ratio +
            dir * (1 - ratio));

    }

    Vector3 leafAverage(Branch branch) {
        if (branch.leaf) return branch.length * branch.dir;

        return branch.length * branch.dir +
            branch.ratio * leafAverage(branch.a) +
            (1 - branch.ratio) * leafAverage(branch.b);

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