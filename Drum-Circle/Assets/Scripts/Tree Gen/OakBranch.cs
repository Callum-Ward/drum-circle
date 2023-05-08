using UnityEngine;
using System.Linq;

public class OakBranch : Branch
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
        l.leafObj.transform.Rotate(0, 90, 0);

        var r = Random.Range(-45, 45);
        l.leafObj.transform.Rotate(0, 0, r);

        leaves = leaves.Concat(new leaf[] { l }).ToArray();
    }

    public override void UpdateLeavesPosition()
    {
        foreach (var leaf in leaves)
        {
            leaf.leafObj.transform.position = position + leaf.position * length / maxLength;
            leaf.leafObj.transform.localScale = Vector3.one * length / maxLength * 15 /* * (maxLength / tree.length)*/;
        }
    }
}