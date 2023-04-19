using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.GraphicsBuffer;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Branch : MonoBehaviour
{
    private Tree tree;
    public GameObject leafObj;

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
    new Renderer renderer;

    protected Vector3[] vertices = new Vector3[] { };
    private Vector2[] uv = new Vector2[] { };
    private int[] triangles = new int[] { };
    

    //[SerializeField] int leavesNo = 5;
    [HideInInspector] public struct leaf
    {
        public GameObject leafObj;
        public Vector3 position;
    }
    [HideInInspector] public leaf[] leaves = new leaf[] { };

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!isLeaf)
        {
            /*foreach(leaf leaf in leaves)
            {
                leaf.leafObj.SetActive(false);
            }*/
        }
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

        SetLeaves();

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

        //SetLeaves();

        if(!isFullyGrown)
        {
            length += maxLength / 1000 * scoreMul;
            
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
            default:
                segments = 2;
                faces = 4;
                break;
        }

        GenerateMesh(segments, faces);
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        if (isLeaf)
        {
            UpdateLeavesPosition();
        }
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

    /* Generate the branch's mesh */
    void GenerateMesh(int segments, int faces)
    {
        vertices = new Vector3[] { };
        uv = new Vector2[] { };

        pointsAlongCurve = curve.GetPointsAlongCurve(segments);

        //the minimum widtht of the graph is the width of its children 
        float minWidth = 0.001f;
        if (!isLeaf)
        {
            minWidth = childA.width;
        }

        //Debug.Log(segments.ToString() + ", " + faces.ToString());

        //for each sampled point of the curve
        for (int i = 0; i < pointsAlongCurve.Length; i++)
        {
            var newVertices = new Vector3[faces];

            //the tangent vector conects 2 points aling the curve
            Vector3 tangent;

            //special case for the last point since it has no points ahead to
            //calculate the tangent
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

            //the width of the segment falls beween the minimum width and
            //the width of the branch
            float segmentWidth = width - i * (width - minWidth) / segments;

            //the normal vector is perpendicular to the tangent vector
            Vector3 norm = Vector3.Normalize(tangent - growth.normalized);

            //if it is the first layer of the mesh, then it is the same as the last layer 
            //of the parent
            if (i == 0 && parent != null && parent.childA == this)
            {
                Array.Copy(parent.vertices, parent.vertices.Length - faces, newVertices, 0, faces);
                newVertices = newVertices.Select(x => x - parent.growth).ToArray();

                for (int j = 0; j < faces; j++)
                {
                    var newUv = new Vector2((float)j / faces, 0);
                    uv = uv.Append(newUv).ToArray();
                }
            }

            else
            {
                //for each face
                for (int j = 0; j < faces; j++)
                {
                    //calculate the angle around the cebter
                    var angle = (2 * Mathf.PI * j) / faces;
                    angle *= Mathf.Rad2Deg;

                    //the vertex is the normal vector rotated by an angle around the
                    //tangent vector with a magnitude equal to the width of the segment
                    var vertex = Quaternion.AngleAxis(angle, tangent) * norm;
                    vertex *= segmentWidth;
                    vertex += (length/maxLength) *  pointsAlongCurve[i];

                    newVertices[j] = vertex;

                    var newUv = new Vector2((float)j / faces, (float)i / pointsAlongCurve.Length);
                    uv = uv.Append(newUv).ToArray();
                }
            }

            // new layers need to be rotated so that the don't 'pinch' the mesh

            //the first layer does not need to be rotated
            if ( i == 0 )
            {
                vertices = newVertices;
            }

            else
            {
                var rotatedNewVertices = new Vector3[faces];
                
                var lastLayer = new Vector3[faces];
                Array.Copy(vertices, vertices.Length - faces, lastLayer, 0, faces);
                

                //we need to find the 2 vertecies that are closest between the new layer
                //and the last layer of the mesh

                //the index of the vertex that has the minimum distance
                int vertexMinD = 0;
                //the minimum distance between a vertex of the last layer
                float minD = Vector3.Distance(lastLayer[0], newVertices[0]);
                for (int j = 1; j < faces; j++)
                {
                    if (Vector3.Distance(lastLayer[j], newVertices[0]) < minD)
                    {
                        minD = Vector3.Distance(lastLayer[j], newVertices[0]);
                        vertexMinD = j;
                    }
                }

                //displace the new layer by the index of the vertex that is closest
                //to the first vertex of the last layer
                for (int j = 0; j < faces; j++)
                {
                    rotatedNewVertices[(j + vertexMinD) % faces] = newVertices[j];
                }

                vertices = vertices.Concat(rotatedNewVertices).ToArray();
            }
        }

        triangles = new int[] { };
        
        //each segment has a number of faces
        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < faces; j++)
            {
                //the side of a segment is a quadrilateral polygon
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


    public virtual void SetLeaves()
    {
        int leavesNo = 30;

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

            var r = Random.Range(-40, 30);
            leaves[i].leafObj.transform.Rotate(r, i * 360 / leavesNo, 0);
        }
    }

    public virtual void UpdateLeavesPosition()
    {
        foreach( var leaf in leaves )
        {
            leaf.leafObj.transform.position = position + leaf.position * length / maxLength;
            leaf.leafObj.transform.localScale = Vector3.one * length / maxLength * 7;
        }
    }
}
