using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class Branch : MonoBehaviour
{
    public GameObject leaves;

    public Branch parent = null;
    public Branch childA  = null;
    public Branch childB  = null;
    public Vector3 growth;
    public Vector3 basis;
    public Vector3 position;

    public bool isLeaf = true;
    public bool isFullyGrown = false;

    public float maxLength;
    private float length;
    public float maxWidth;
    private float width;

    private LineRenderer lr;

    public Mesh mesh;
    public int faces;
    public int segments;

    protected Vector3[] vertices = new Vector3[] { };
    private int[] triangles = new int[] { };

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Update()
    {
        if ( !isLeaf ) Object.Destroy( leaves );
    }

    /*Set up branch parameters*/
    public void SetBranch(GameObject tree, Vector3 growth, Vector3 basis, Vector3 position, float width)
    {
        this.growth = growth;
        this.basis = basis;
        this.position = position;

        maxLength = growth.magnitude;
        maxWidth = width;

        isLeaf = true;
        isFullyGrown = false;

        Place(tree);
    }

    /*Set up branch parameters*/
    public void SetBranch(GameObject tree, Branch parent, Vector3 growth, Vector3 basis, Vector3 position, float width)
    {
        this.parent = parent;

        SetBranch(tree, growth, basis, position, width);
    }

    /*Places branch in place*/
    public void Place(GameObject tree)
    {
        this.transform.parent = tree.transform;
        this.transform.position = position;

        length = 0;

        lr.positionCount = 2;

        lr.SetPosition(0, position);
        lr.SetPosition(1, position);
    }

    /*Set the children of a brnach*/
    public void SetChildren(Branch a, Branch b)
    {
        childA = a;
        childB = b;
    }

    /*Grow the branch*/
    public void Grow(float scoreMul)
    {
        GetComponent<MeshRenderer>().enabled = true;

        if (!isLeaf) return;

        AddLeaves();

        if(!isFullyGrown)
        {
            length += maxLength / 100 * scoreMul;
            width = maxLength * maxWidth;


            if (length >= maxLength) isFullyGrown = true;

            var endpoint = position + length / maxLength * growth;

            lr.SetPosition(1, endpoint );

            GenerateMesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
    }

    void GenerateMesh()
    {
        vertices = new Vector3[] { };
        triangles = new int[] { };

        var orth = Vector3.Normalize(growth - basis);
        orth *= width;

        var endpoint = length / maxLength * growth;

        for (int i = 0; i < segments + 1;  i++)
        {
            for (int j = 0; j < faces; j++)
            {
                var ang = (2 * Mathf.PI * j) / (faces - 1);
                ang *= Mathf.Rad2Deg;

                var rotOrth = Quaternion.AngleAxis(ang, growth) * orth;
                
                var vertex = (endpoint / segments) * i + rotOrth;

                vertices = vertices.Concat(new Vector3[] {vertex}).ToArray();
            }
        }

        if (parent != null && parent.childA == this)
        {
            var lastPreviousLayer = new Vector3[faces];

            Array.Copy(parent.vertices, parent.vertices.Length - faces, lastPreviousLayer, 0, faces);
            lastPreviousLayer = lastPreviousLayer.Select(x => x - parent.growth).ToArray();

            var vertexMinD = 0;
            float minD = Vector3.Distance(lastPreviousLayer[0], vertices[0]);
            for (int i = 0; i < faces; i++)
            {
                if (Vector3.Distance(lastPreviousLayer[i], vertices[0]) < minD)
                {
                    minD = Vector3.Distance(lastPreviousLayer[i], vertices[0]);
                    vertexMinD = i;
                }
            }

            for (int i = 0; i < faces; i++)
            {
                vertices[i] = lastPreviousLayer[(i + vertexMinD) % faces];
            }
        }

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < faces; j++)
            {
                var quad = new int[]
                {
                    i * faces + j,
                    (j + 1) % faces + i * faces,
                    (i + 1) * faces + j,

                    (i + 1) * faces + j,
                    (j + 1) % faces + i * faces,
                    (j + 1) % faces + (i + 1) * faces
                };

                triangles = triangles.Concat(quad).ToArray();
            }
        }


    }

    void AddLeaves()
    {
        leaves.transform.parent = this.transform;
        leaves.transform.position = position + length / maxLength * growth;
        leaves.transform.rotation = Quaternion.LookRotation(growth);
    }
}
