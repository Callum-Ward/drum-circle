using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBranch : MonoBehaviour
{
    public NewBranch parent { get; private set; } = null;
    public NewBranch childA { get; private set; } = null;
    public NewBranch childB { get; private set; } = null;
    public Vector3 growth { get; private set; }
    public Vector3 position { get; private set; }

    public bool isLeaf { get; set; } = true;
    public bool isFullyGrown { get; private set; } = false;

    private float maxLength;
    private float length;

    private void Update()
    {
        if (Input.GetKey("space")) Grow();
    }

    public void SetBranch(GameObject tree, NewBranch parent, Vector3 growth, Vector3 position)
    {
        this.parent = parent;
        this.growth = growth;
        this.position = position;

        maxLength = growth.magnitude;

        Place(tree);
    }

    public void SetBranch(GameObject tree, Vector3 growth, Vector3 position)
    {
        this.growth = growth;
        this.position = position;

        maxLength = growth.magnitude;

        Place(tree);
    }


    public void Place(GameObject tree)
    {
        this.transform.parent = tree.transform;
        this.transform.position = position;
        this.transform.rotation = Quaternion.LookRotation(growth);

        length = 0;

        this.transform.localScale = new Vector3(1, length, 1);
    }

    void Grow()
    {
        if (!isLeaf) return;

        if(length <= maxLength)
        {
            length += maxLength / 100;

            this.transform.localScale = new Vector3(1, length, 1);
        }
    }

}
