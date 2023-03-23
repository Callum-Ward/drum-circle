using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.GraphicsBuffer;
using Object = UnityEngine.Object;

public class Branch : MonoBehaviour
{
    private Tree tree;
    public GameObject leaves;

    public Branch parent = null;
    public Branch childA  = null;
    public Branch childB  = null;

    [HideInInspector] public Vector3 growth;
    [HideInInspector] public Vector3 basis;
    [HideInInspector] public Vector3 position;

    [HideInInspector] public bool isLeaf = true;
    [HideInInspector] public bool isFullyGrown = false;

    [HideInInspector] public float maxLength;
    [HideInInspector] public float length;
    [HideInInspector] private float widthMul;
    [HideInInspector] public float maxWidth;
    [HideInInspector] public float width;
    [HideInInspector] public Curve curve;

    private Vector3[] pointsAlongCurve;

    public Mesh mesh;
    //public int faces;
    //public int segments;

    protected Vector3[] vertices = new Vector3[] { };
    private int[] triangles = new int[] { };

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        //lod = GetComponent<LOD>();
    }

    private void Update()
    {
        if ( !isLeaf ) leaves.SetActive(false);
    }
    
    /*Set up branch parameters*/
    public void SetBranch(GameObject tree, Vector3 growth, Vector3 basis, Vector3 position, 
        float width, Curve curve)
    {
        this.tree = tree.GetComponent<Tree>();

        this.growth = growth;
        this.basis = basis;
        this.position = position;

        maxLength = curve.GetCurveLength(100);
        widthMul = width;

        this.curve = curve;

        isLeaf = true;
        isFullyGrown = false;

        //SetLeaves();
        leaves.transform.rotation = Quaternion.LookRotation(growth);

        Place(tree);
    }

    /*Set up branch parameters*/
    public void SetBranch(GameObject tree, Branch parent, Vector3 growth, Vector3 basis, Vector3 position,
        float width, Curve curve)
    {
        this.parent = parent;

        SetBranch(tree, growth, basis, position, width, curve);
    }

    /*Places branch in place*/
    public void Place(GameObject tree)
    {
        this.transform.parent = tree.transform;
        this.transform.position = position;

        length = 0;
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

        maxWidth = widthMul * LengthToEnd();

        SetLeaves();

        if(!isFullyGrown)
        {
            length += maxLength / 100 * scoreMul;
            
            width = length * maxWidth;


            if (length >= maxLength) isFullyGrown = true;
        }

        int segments = 0;
        int faces = 0;

        switch (tree.lod)
        {
            case 0:
                segments = 10;
                faces = 16; 
                break;
            case 1:
                segments = 6;
                faces = 10;
                break;
            case 2:
                segments = 3;
                faces = 6;
                break;
            case 3:
                segments = 2;
                faces = 4;
                break;
        }

        GenerateMesh(segments, faces);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    float LengthToEnd()
    {
        float lengthToEnd = length;
        var branch = this;

        while (branch.childA != null)
        {
            branch = branch.childA;
            lengthToEnd += branch.length;
        }

        return lengthToEnd;
    }

    void GenerateMesh(int segments, int faces)
    {
        vertices = new Vector3[] { };

        pointsAlongCurve = curve.GetPointsAlongCurve(segments);

        float minWidth = 0;
        if (!isLeaf)
        {
            minWidth = childA.width;
        }

        if (pointsAlongCurve.Length != segments + 1) Debug.Log("not enough points");

        for (int i = 0; i < pointsAlongCurve.Length; i++)
        {
            var newVertices = new Vector3[faces];

            Vector3 tangent;

            if(i == pointsAlongCurve.Length - 1)
            {
                tangent = pointsAlongCurve[i] - pointsAlongCurve[i - 1];
                tangent = tangent.normalized;
            }
            else
            {
                tangent = pointsAlongCurve[i + 1] - pointsAlongCurve[i];
                tangent = tangent.normalized;
            }

            float segmentWidth = width - i * (width - minWidth) / segments;

            Vector3 norm = Vector3.Normalize(tangent - growth.normalized);

            if (i == 0 && parent != null && parent.childA == this)
            {
                Array.Copy(parent.vertices, parent.vertices.Length - faces, newVertices, 0, faces);
                newVertices = newVertices.Select(x => x - parent.growth).ToArray();
            }

            else
            {
                for (int j = 0; j < faces; j++)
                {
                    var angle = (2 * Mathf.PI * j) / faces;
                    angle *= Mathf.Rad2Deg;

                    var vertex = Quaternion.AngleAxis(angle, tangent) * norm;
                    vertex *= segmentWidth;
                    vertex += (length/maxLength) *  pointsAlongCurve[i];

                    newVertices[j] = vertex;
                }
            }

            if ( i == 0 && (parent == null || 
                (parent != null && parent.childB ==this) ) )
            {
                vertices = newVertices;
            }

            else
            {
                var rotatedNewVertices = new Vector3[faces];
                var lastLayer = new Vector3[faces];

                if (i == 0 && parent != null && parent.childA == this)
                {
                    Array.Copy(parent.vertices, parent.vertices.Length - faces, lastLayer, 0, faces);
                    lastLayer = lastLayer.Select(x => x - parent.growth).ToArray();
                }

                else Array.Copy(vertices, vertices.Length - faces, lastLayer, 0, faces);

                int vertexMinD = 0;
                float minD = Vector3.Distance(lastLayer[0], newVertices[0]);
                for (int j = 1; j < faces; j++)
                {
                    if (Vector3.Distance(lastLayer[j], newVertices[0]) < minD)
                    {
                        minD = Vector3.Distance(lastLayer[j], newVertices[0]);
                        vertexMinD = j;
                    }
                }
                for(int j =0; j < faces; j++)
                {
                    rotatedNewVertices[(j + vertexMinD) % faces] = newVertices[j];
                }
                vertices = vertices.Concat(rotatedNewVertices).ToArray();
            }
        }

        triangles = new int[] { };

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

    void SetLeaves()
    {
        leaves.transform.parent = this.transform;
        leaves.transform.position = position + length / maxLength * growth;
        leaves.transform.rotation = Quaternion.LookRotation(growth);
    }
}
