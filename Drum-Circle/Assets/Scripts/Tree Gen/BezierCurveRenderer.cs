using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class BezierCurveRenderer : MonoBehaviour
{
    LineRenderer lr;
    float directionNoise = 1f;

    Mesh mesh;
    int[] triangles;
    Vector3[] vertices;

    /*Returns random value between the min and max according to normal distribution*/
    float RandomNormal(float min = -1, float max = 1)
    {
        float u, v, s;

        do
        {
            u = 2 * Random.value - 1;
            v = 2 * Random.value - 1;

            s = u * u + v * v;
        } while (s >= 1);

        float std = u * Mathf.Sqrt(-2 * Mathf.Log(s) / s);

        float mean = (min + max) / 2;
        float sigma = (max - mean) / 3;

        return Mathf.Clamp(std * sigma + mean, min, max);
    }

    /* Wrapper for RandomNormal(min, max)*/
    float RandomNormal(float range)
    {
        return RandomNormal(-range, range);
    }

    BezierCurve GetBranchCurve(Vector3 position, Vector3 growth)
    {
        var cp1_xangle = RandomNormal(directionNoise) * 90;
        var cp1_zangle = RandomNormal(directionNoise) * 90;
        var cp1 = (position + growth) * 0.5f;
        cp1 = Quaternion.Euler(cp1_xangle, 0, cp1_zangle) * cp1;

        var cp2_xangle = RandomNormal(directionNoise) * 90;
        var cp2_zangle = RandomNormal(directionNoise) * 90;
        var cp2 = (growth - position) * 0.5f;
        cp2 = Quaternion.Euler(cp2_xangle, 0, cp2_zangle) * cp2;

        return new BezierCurve(position, cp1, cp2, growth);
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

    Vector3[] GetPointsAlongCurve(BezierCurve curve, int n)
    {
        float spacing = GetCurveLength(curve, n) / n;

        var points = new Vector3[] {};
        points = points.Concat(new Vector3[] {curve.P0}).ToArray();

        Vector3 lastPoint = points[0];

        float dSinceLastPoint = 0;

        float t = 0;
        while ( t <= 1)
        {
            t += (float)1 / n;
            var pointOnCurve = EvaluateCurve(curve, t);
            dSinceLastPoint += Vector3.Distance(lastPoint, pointOnCurve);
                
            while ( dSinceLastPoint > spacing)
            {
                float overshootD = dSinceLastPoint - spacing;
                Vector3 newEvenlySpacedPoint = pointOnCurve + (lastPoint - pointOnCurve).normalized * overshootD;
                points = points.Concat(new Vector3[] { newEvenlySpacedPoint }).ToArray();
                dSinceLastPoint = overshootD;
                lastPoint = newEvenlySpacedPoint;
            }

            lastPoint = pointOnCurve;
        }

        return points;
    }


    Vector3 GetPointAlongCurve(BezierCurve curve, float t)
    {
        var points = new Vector3[]
        {
            curve.P0,
            curve.P1,
            curve.P2,
            curve.P3,
        };
        var degree = points.Length - 1;

        for (int i = 0; i < degree; i++)
        {
            for (int j = 0; j < degree - i; j++)
            {
                points[j] = (1 - t) * points[j] + t * points[j + 1];
            }
        }

        return points[0];
    }

    void GenerateMesh(Vector3[] points)
    {
        vertices = new Vector3[] { };
        triangles = new int[] { };

        int faces = 100;
        float width = 0.2f;

        for (int i = 0; i < points.Length; i++)
        {
            var newVertices = new Vector3[faces];

            Vector3 tangent;
            Vector3 norm;

            if (i == points.Length - 1)
            {
                tangent = points[i] - points[i - 1];
                tangent = tangent.normalized;
            }

            else
            {
                tangent = points[i + 1] - points[i];
                tangent = tangent.normalized;
            }

            norm = Vector3.Normalize(tangent - Vector3.up);

            for (int j = 0; j < faces; j++)
            {
                var angle = (2 * Mathf.PI * j) / faces;
                angle *= Mathf.Rad2Deg;

                var vertex = Quaternion.AngleAxis(angle, tangent) * norm;
                vertex *= width;
                Debug.Log(vertex.magnitude);
                vertex += points[i];

                newVertices[j] = vertex;
            }

            if (i > 0)
            {
                var rotatedNewVertices = new Vector3[faces];
                var lastLayer = new Vector3[faces];
                Array.Copy(vertices, vertices.Length - faces, lastLayer, 0, faces);

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
                for (int j = 0; j < faces; j++)
                {
                    rotatedNewVertices[(j + vertexMinD) % faces] = newVertices[j];
                }
                vertices = vertices.Concat(rotatedNewVertices).ToArray();
            }
            else vertices = newVertices;
        }



        for (int i = 0; i< points.Length - 1; i++)
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

        // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 5;

        mesh = GetComponent<MeshFilter>().mesh;

        var position = Vector3.zero;
        var growth = Vector3.up * 3;
        var curve = GetBranchCurve(position, growth);

        var points = GetPointsAlongCurve(curve, 100);

        GenerateMesh(points);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        for (int i = 0; i < 4; i++)
        {
            var p = points[i];
            lr.SetPosition(i, p);
        }

        lr.SetPosition(4, curve.P3);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
