using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    //Serialized vars
    [SerializeField] int n_latitudes, n_longitudes;
    [SerializeField] float radius;
    Mesh mesh;


    List<Vector3> vertices;
    List<int> triangles;


    // non-state vars
    float PI = Mathf.PI;
    void Start()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new List<Vector3>();
        triangles = new List<int>();

        CreateMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        vertices.Clear();
        triangles.Clear();
        // uv sphere / disco ball
        // draw middle loops
        // each loop contains n_longitudes vertices
        // there are total n_latitudes loops

        // top point
        vertices.Add(new Vector3(0, radius, 0));

        // middle loops
        for (int h = 0; h < n_latitudes; h++)
        {
            float angle1 = (h + 1) * PI / (n_latitudes + 1);
            for (int v = 0; v < n_longitudes; v++)
            {
                float angle2 = v * PI * 2 / n_longitudes;

                float x = Mathf.Sin(angle1) * Mathf.Cos(angle2);
                float y = Mathf.Cos(angle1);
                float z = Mathf.Sin(angle1) * Mathf.Sin(angle2);
                Vector3 pointOnSphere = (new Vector3(x, y, z)) * radius;
                vertices.Add(pointOnSphere);
            }
        }

        // bottom point
        vertices.Add(new Vector3(0, -radius, 0));

        // add triangles

        // top cap
        for(int i = 0; i < n_longitudes; i++)
        {
            int index = 0 * n_latitudes + i + 1;
            if(i == n_longitudes - 1)
            {
                triangles.Add(index);
                triangles.Add(0);
                triangles.Add(1);
            }
            else
            {
                triangles.Add(index);
                triangles.Add(0);
                triangles.Add(index + 1);
            }
        }

        // add middle triangles
        for (int la = 0; la < n_latitudes - 1; la++)
        {
            for (int lo = 0; lo < n_longitudes; lo++)
            {
                int index = la * n_longitudes + lo + 1;
                if (lo == n_longitudes - 1)
                {
                    //final quad
                    triangles.Add(index);
                    triangles.Add(index - n_longitudes + 1);
                    triangles.Add(index + 1);

                    triangles.Add(index + n_longitudes);
                    triangles.Add(index);
                    triangles.Add(index + 1);
                }
                else
                {
                    //other quads
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + n_longitudes + 1);

                    triangles.Add(index + n_longitudes);
                    triangles.Add(index);
                    triangles.Add(index + n_longitudes + 1);
                }
            }
        }
    
        // bottom cap
        for(int i = 0; i < n_longitudes; i++)
        {
            int index = (n_latitudes - 1) * n_longitudes + i + 1;
            if(i == n_longitudes - 1)
            {
                //last triangle
                triangles.Add(index);
                triangles.Add(index - n_longitudes + 1);
                triangles.Add(0);
            }
            else
            {
                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(0);
            }
        }
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;
        for (int i = 0; i < vertices.Count; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.02f);
        }

        //if (triangles == null) return;
        //for (int i = 0; i < triangles.Count; i = i + 3)
        //{
        //    Gizmos.DrawLine(vertices[triangles[i]], vertices[triangles[i + 1]]);
        //    Gizmos.DrawLine(vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
        //    Gizmos.DrawLine(vertices[triangles[i]], vertices[triangles[i + 2]]);
        //}
    }

}
