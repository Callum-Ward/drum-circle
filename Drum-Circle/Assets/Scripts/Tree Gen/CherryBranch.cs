using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CherryBranch : Branch
{
    public override void SetLeaves()
    {
        if (parent == null) return;

        var leaf = Instantiate(leafObj);
        var l = new leaf();
        l.leafObj = leaf;

        var leafPoints = curve.GetPointsAlongCurve(2);

        l.position = leafPoints[1];
        l.leafObj.transform.parent = this.gameObject.transform;
        l.leafObj.transform.localScale = Vector3.one * 5;
        l.leafObj.transform.position = position + leafPoints[1] * length / maxLength;
        l.leafObj.transform.rotation = Quaternion.LookRotation(growth, growth - basis);
        l.leafObj.transform.Rotate(0, Random.Range(-45, 45), 0);

        leaves = leaves.Concat(new leaf[] { l }).ToArray();
    }

    public override void UpdateLeavesPosition()
    {
        foreach (var leaf in leaves)
        {
            leaf.leafObj.transform.position = position + leaf.position * length / maxLength;
            leaf.leafObj.transform.localScale = Vector3.one * length / maxLength * 20 /* * (maxLength / tree.length)*/;
        }
    }
}
