using System;
using UnityEngine;

public class Branch {
    int id = 0;
    bool leaf = true;

    Branch a, b, parent;

    float ratio, spread, splitSize;
    int depth = 0;
    float splitDecay = 1;

    Vector3 dir = new Vector3(0, 1, 0); //up
    float length = 0, area = 0.1;

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

    public int GetId() {
        return this.id;
    }

    public void Grow(float feed) {
        if (leaf) {
            length += Math.Cbrt(feed);
            feed -= Math.Cbrt(feed);
            area += feed / length;

            if (length > splitSize * Math.Exp(-splitDecay * depth)) {
                //Split();
            } 
        }

        else {
            pass = (a.area + b.area) / (a.area + b.area + this.area);

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

        Vector3 density = leafDensity();
    }

    Vector3 leafAverage(Branch branch) {
        if (branch.leaf) return branch.length * branch.dir;

        return branch.length * branch.dir +
            branch.ratio * leafAverage(branch.a) +
            (1 - branch.ratio) * leafAverage(branch.b);

    }

    Vector3 leafDensity(int searchDepth) {
        System.Random r = new System.Random();
        Vector3 rand = new Vector3(float(r.NextDouble()), r.NextDouble(), r.NextDouble());

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