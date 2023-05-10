using UnityEngine;
using System.Linq;

public class OakBranch : Branch
{
    // Overite this funtion for oak trees
    public override void SetLeaves()
    {
        //Exclude the root branch
        if (parent == null) return;

        var leaf = Instantiate(leafObj);
        var l = new leaf();
        l.leafObj = leaf;

        var leafPoints = curve.GetPointsAlongCurve(2);

        // The leaf cluster is placed at tge middle of the branch and is rotated 
        // so that it is in line with the cluster is in line with the branch.
        l.position = leafPoints[1];
        l.leafObj.transform.parent = this.gameObject.transform;
        l.leafObj.transform.localScale = Vector3.one * 5;
        l.leafObj.transform.position = position + leafPoints[1] * length / maxLength;
        l.leafObj.transform.rotation = Quaternion.LookRotation(growth, growth - basis);
        l.leafObj.transform.Rotate(0, 90, 0);

        // Rotate th cluster so that the canopy appears fuller
        var r = Random.Range(-45, 45);
        l.leafObj.transform.Rotate(0, 0, r);

        leaves = leaves.Concat(new leaf[] { l }).ToArray();
    }

    // Overite this funtion for oak trees
    public override void UpdateLeaves()
    {
        foreach (var leaf in leaves)
        {
            // scale the position of the leaves with the size of the branch and increase
            // the size to the appropriate size for oak trees
            leaf.leafObj.transform.position = position + leaf.position * length / maxLength;
            leaf.leafObj.transform.localScale = Vector3.one * length / maxLength * 15;
        }
    }
}