using System.Linq;
using UnityEngine;

public class Curve
{
    public Vector3 P0 { get; set; }
    public Vector3 P1 { get; set; }
    public Vector3 P2 { get; set; }
    public Vector3 P3 { get; set; }
    
    public Curve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) 
    {
        P0 = p0;
        P1 = p1;
        P2 = p2;
        P3 = p3;
    }

    /*Retrun a point that at t distance from p0 along a cubic Bezier curv*/
    Vector3 EvaluateCubicCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        var p0 = Vector3.Lerp(a, b, t);
        var p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    /*Retrun a point that at t distance from p0 along the curve*/
    public Vector3 EvaluateCurve(float t)
    {
        var p0 = EvaluateCubicCurve(P0, P1, P2, t);
        var p1 = EvaluateCubicCurve(P1, P2, P3, t);
        return Vector3.Lerp(p0, p1, t);
    }

    /*Return an aproximation for the length of the curve given a resolution*/
    public float GetCurveLength(int resolution)
    {
        float length = 0;
        var lastPoint = P0;
        for (int i = 1; i <= resolution; i++)
        {
            float step = (float)i / (float)resolution;
            var newPoint = EvaluateCurve(step);
            length += Vector3.Distance(lastPoint, newPoint);
            lastPoint = newPoint;
        }
        return length;
    }

    /*Return n+1 equidistant points along hte curve*/ 
    public Vector3[] GetPointsAlongCurve(int n)
    {
        float spacing = GetCurveLength(2 * n) / n;

        var points = new Vector3[] { };
        points = points.Concat(new Vector3[] { P0 }).ToArray();

        Vector3 lastPoint = points[0];

        float dSinceLastPoint = 0;

        float t = 0;
        while (t <= 1)
        {
            var pointOnCurve = EvaluateCurve(t);
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

        while (points.Length < n + 1)
        {
            points = points.Concat(new Vector3[] { P3 }).ToArray();
        }
        if (points.Last().x != P3.x || points.Last().y != P3.y
        || points.Last().z != P3.z) points[points.Length - 1] = P3;
        return points;
    }

    /* Return a point that is at t% along the distance of the curve*/
    public Vector3 GetPointAlongCurve(float t)
    {
        int p = (int)(t * 100);
        return GetPointsAlongCurve(100)[p];
    }

}
