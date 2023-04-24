using UnityEngine;
using System.Linq;

public class PalmBranch : Branch
{
    public override void SetLeaves()
    {
        int leavesNo = 20;

        for (int i = 0; i < leavesNo; i++)
        {
            var leaf = Instantiate(leafObj);
            var l = new leaf();
            l.leafObj = leaf;

            leaves = leaves.Concat(new leaf[] { l }).ToArray();
        }

        for (int i = 0; i < leavesNo; i++)
        {
            leaves[i].position = growth;
            leaves[i].leafObj.transform.parent = this.gameObject.transform;
            leaves[i].leafObj.transform.localScale = Vector3.zero;
            leaves[i].leafObj.transform.position = position + growth * length / maxLength;
            leaves[i].leafObj.transform.rotation = Quaternion.LookRotation(growth - basis, growth);
            leaves[i].leafObj.transform.Rotate(0, i * 360 / leavesNo, 0);
        }
    }
}