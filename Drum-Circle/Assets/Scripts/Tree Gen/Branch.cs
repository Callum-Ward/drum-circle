using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
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
    private BezierCurve curve;

    private Vector3[] pointsAlongCurve;

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
        float width, BezierCurve curve)
    {
        this.growth = growth;
        this.basis = basis;
        this.position = position;

        maxLength = GetCurveLength(curve, segments);
        maxWidth = width;

        this.curve = curve;

        pointsAlongCurve = GetPointsAlongCurve(segments);

        isLeaf = true;
        isFullyGrown = false;

        //SetLeaves();
        leaves.transform.rotation = Quaternion.LookRotation(growth);

        Place(tree);
    }

    /*Set up branch parameters*/
    public void SetBranch(GameObject tree, Branch parent, Vector3 growth, Vector3 basis, Vector3 position,
        float width, BezierCurve curve)
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

    Vector3 EvaluateCubicCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        var p0 = Vector3.Lerp(a, b, t);
        var p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    Vector3 EvaluateCurve(BezierCurve curve, float t)
    {
        var p0 = EvaluateCubicCurve(curve.P0, curve.P1, curve.P2, t);
        var p1 = EvaluateCubicCurve(curve.P1, curve.P2, curve.P3, t);
        return Vector3.Lerp(p0, p1, t);
    }

    float GetCurveLength(BezierCurve curve, int resolution)
    {
        float length = 0;
        var lastPoint = curve.P0;
        for (int i = 1; i <= resolution; i++)
        {
            float step = (float)i / (float)resolution;
            var newPoint = EvaluateCurve(curve, step);
            length += Vector3.Distance(lastPoint, newPoint);
            lastPoint = newPoint;
        }
        return length;
    }

    Vector3[] GetPointsAlongCurve(int n)
    {
        float spacing = GetCurveLength(curve, n) / n;

        var points = new Vector3[] { };
        points = points.Concat(new Vector3[] { curve.P0 }).ToArray();

        Vector3 lastPoint = points[0];

        float dSinceLastPoint = 0;

        float t = 0;
        while (t <= 1)
        {
            var pointOnCurve = EvaluateCurve(curve, t);
            t += (float)1 / n;
            dSinceLastPoint += Vector3.Distance(lastPoint, pointOnCurve);

            while (dSinceLastPoint > spacing)
            {
                float overshootD = dSinceLastPoint - spacing;
                Vector3 newEvenlySpacedPoint = pointOnCurve + (lastPoint - pointOnCurve).normalized * overshootD;
                points = points.Concat(new Vector3[] { newEvenlySpacedPoint }).ToArray();
                dSinceLastPoint = overshootD;
                lastPoint = newEvenlySpacedPoint;
            }

            lastPoint = pointOnCurve;
        }
  
        if (points.Length < n + 1) points = points.Concat(new Vector3[] { curve.P3 }).ToArray();

        if (points.Last().x != curve.P3.x || points.Last().y != curve.P3.y
            || points.Last().z != curve.P3.z) points[points.Length - 1] = curve.P3;

        return points;
    }

    void GenerateMesh()
    {
        vertices = new Vector3[] { };
        triangles = new int[] { };

        float endpoint = 0;
        int meshSegments = 0;

        while ( endpoint < length )
        {
            var tangent = pointsAlongCurve[meshSegments + 1] - pointsAlongCurve[meshSegments];
            var orth = Vector3.Cross(tangent, basis).normalized;

            for (int j = 0; j < faces; j++)
            {
                var ang = (2 * Mathf.PI * j) / (faces - 1);
                ang *= Mathf.Rad2Deg;

                var vertex = Quaternion.AngleAxis(ang, growth) * orth * width + pointsAlongCurve[meshSegments];

                vertices = vertices.Concat(new Vector3[] {vertex}).ToArray();
            }

            endpoint += Vector3.Distance(pointsAlongCurve[meshSegments + 1], pointsAlongCurve[meshSegments]);
            meshSegments++;
        }

        if (endpoint > length)
        {
            var tangent = pointsAlongCurve[meshSegments] - pointsAlongCurve[meshSegments + 1];
            var orth = Vector3.Cross(tangent, basis).normalized;

            float scale = 1 - ( (endpoint - length) / 
                Vector3.Distance(pointsAlongCurve[meshSegments], pointsAlongCurve[meshSegments + 1]) );

            for (int j = 0; j < faces; j++)
            {
                var ang = (2 * Mathf.PI * j) / (faces - 1);
                ang *= Mathf.Rad2Deg;

                var vertex = Quaternion.AngleAxis(ang, pointsAlongCurve[meshSegments + 1]) * orth * width + 
                        pointsAlongCurve[meshSegments + 1] * scale;

                vertices = vertices.Concat(new Vector3[] { vertex }).ToArray();
            }
        }

        if (parent != null && parent.childA == this)
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
        }


        int i = 0;
        do
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
            i++;
        } while (endpoint < length);
    }

    void SetLeaves()
    {
        leaves.transform.parent = this.transform;
        leaves.transform.position = position + length / maxLength * growth;
        leaves.transform.rotation = Quaternion.LookRotation(growth);
    }
}
