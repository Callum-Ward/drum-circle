using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.GraphicsBuffer;
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
    public Curve curve;

    public Vector3[] controllPoints;
    public Vector3[] pointsAlongCurve;

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
        if ( !isLeaf ) leaves.SetActive(false);
    }

    /*Set up branch parameters*/
    public void SetBranch(GameObject tree, Vector3 growth, Vector3 basis, Vector3 position, 
        float width, Curve curve)
    {
        this.growth = growth;
        this.basis = basis;
        this.position = position;

        maxLength = curve.GetCurveLength(segments);
        maxWidth = width;

        this.curve = curve;
        controllPoints = new Vector3[]
        {
            curve.P0,
            curve.P1,
            curve.P2,
            curve.P3
        };
        pointsAlongCurve = curve.GetPointsAlongCurve(segments);

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

        SetLeaves();

        if(!isFullyGrown)
        {
            length += maxLength / 1000 * scoreMul;
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

            Vector3 norm = Vector3.Normalize(tangent - growth.normalized);

            for (int j = 0; j < faces; j++)
            {
                var angle = (2 * Mathf.PI * j) / faces;
                angle *= Mathf.Rad2Deg;

                var vertex = Quaternion.AngleAxis(angle, tangent) * norm;
                vertex *= width;
                vertex += pointsAlongCurve[i];

                newVertices[j] = vertex;
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
                    Array.Copy(parent.vertices, parent.vertices.Length - faces, newVertices, 0, faces);
                    newVertices = newVertices.Select(x => x - parent.growth).ToArray();
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

        /*if (parent != null && parent.childA == this)
        {
            var lastPreviousLayer = new Vector3[faces];

            Array.Copy(parent.vertices, parent.vertices.Length - faces, lastPreviousLayer, 0, faces);
            lastPreviousLayer = lastPreviousLayer.Select(x => x - parent.growth).ToArray();

            var vertexMinD = 0;
            float minD = Vector3.Distance(lastPreviousLayer[0], vertices[0]);
            for (int j = 0; j < faces; j++)
            {
                if (Vector3.Distance(lastPreviousLayer[j], vertices[0]) < minD)
                {
                    minD = Vector3.Distance(lastPreviousLayer[j], vertices[0]);
                    vertexMinD = j;
                }
            }

            for (int j = 0; j < faces; j++)
            {
                vertices[j] = lastPreviousLayer[(j + vertexMinD) % faces];
            }
        }*/


        for(int i = 0; i < segments; i++)
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
