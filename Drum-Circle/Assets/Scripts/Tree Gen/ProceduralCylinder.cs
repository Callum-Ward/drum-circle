using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralCylinder : MonoBehaviour
{
    [Range(0.5f, 10.0f)] public float height;
    [Range(0.5f, 10f)] public float radius;
    [Range(5, 100)] public int faces;

    Mesh mesh;
    Vector3[] vertices = new Vector3[] { };
    int[] triangles = new int[] { };

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        GenerateShape();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateShape()
    {
        for (int n = 0; n < 2; n++)
        {
            for (int i = 0; i < faces; i++)
            {
                var angle = (2 * Mathf.PI * i) / (faces - 1);
                angle *= Mathf.Rad2Deg;

                Debug.Log(angle);

                var vertex = Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
                vertex += height * n * Vector3.up;

                vertices = vertices.Concat(new Vector3[] { vertex }).ToArray();
            }
        }

        for(int i = 0;i < faces; i++)
        {
            var quad = new int[]
            {
                i,
                (i + 1) % faces,
                i + faces,
                

                i + faces,
                (i + 1) % faces,
                (i + faces + 1) % faces + faces
            };

            triangles = triangles.Concat(quad).ToArray();
        }
    }
}
